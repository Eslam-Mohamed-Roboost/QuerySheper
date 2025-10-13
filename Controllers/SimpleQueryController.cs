using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuerySheper.Models;
using QuerySheper.Application;
using System.Data;
using System.Diagnostics;
using QuerySheper.Persistence;

namespace QuerySheper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleQueryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SimpleQueryController> _logger;
        private readonly ISimpleQueryService _service;

        public SimpleQueryController(IConfiguration configuration, ILogger<SimpleQueryController> logger, ISimpleQueryService service)
        {
            _configuration = configuration;
            _logger = logger;
            _service = service;
        }

        [HttpPost("execute")]
        public async Task<ActionResult> ExecuteQuery([FromBody] QueryRequest request)
        {
            _logger.LogInformation("ExecuteQuery endpoint called with SQL: {SqlQuery}, DatabaseType: {DatabaseType}", 
                request.SqlQuery, request.DatabaseType);
            
            if (string.IsNullOrWhiteSpace(request.SqlQuery))
            {
                _logger.LogWarning("ExecuteQuery called with empty SQL query");
                return BadRequest("SQL query is required");
            }

            // Handle backward compatibility - if request is just a string
            if (request == null || string.IsNullOrEmpty(request.SqlQuery))
            {
                // Try to parse as string for backward compatibility
                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    request = new QueryRequest { SqlQuery = body.Trim('"') };
                }
                else
                {
                    return BadRequest("SQL query is required");
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var results = new List<object>();

            try
            {
                _logger.LogInformation("Executing SQL query: {SqlQuery}", request.SqlQuery);

                // Get database configurations based on type filter and multiple connections
                var databases = GetDatabaseConfigurations(request.DatabaseType, request.CustomConnectionString, request.MultipleConnections, request.MultipleConnectionsDbType);

                foreach (var db in databases)
                {
                    try
                    {
                        var dbResult = await _service.ExecuteQueryOnDatabase(db, request.SqlQuery);
                        results.Add(new
                        {
                            DatabaseName = db.Name,
                            Success = true,
                            Data = dbResult.Data,
                            RowCount = dbResult.RowCount,
                            ExecutionTime = dbResult.ExecutionTime
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error executing query on database {DatabaseName}", db.Name);
                        results.Add(new
                        {
                            DatabaseName = db.Name,
                            Success = false,
                            Error = ex.Message,
                            ExecutionTime = TimeSpan.Zero
                        });
                    }
                }

                stopwatch.Stop();

                return Ok(new
                {
                    SqlQuery = request.SqlQuery,
                    DatabaseType = request.DatabaseType,
                    TotalExecutionTime = stopwatch.Elapsed,
                    TotalDatabases = databases.Count,
                    SuccessfulDatabases = results.Count(r => ((dynamic)r).Success),
                    FailedDatabases = results.Count(r => !((dynamic)r).Success),
                    Results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global error executing query");
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        

        private DbContext CreateDbContext(DatabaseConfig config)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            switch (config.Type)
            {
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(config.ConnectionString);
                    break;
                case DatabaseType.PostgreSQL:
                    optionsBuilder.UseNpgsql(config.ConnectionString);
                    break;
               
                default:
                    throw new NotSupportedException($"Database type '{config.Type}' is not supported");
            }

            optionsBuilder.EnableSensitiveDataLogging(false);
            optionsBuilder.EnableServiceProviderCaching();

            return new GenericDbContext(optionsBuilder.Options);
        }

        private List<DatabaseConfig> GetDatabaseConfigurations(string databaseType = "both", string? customConnectionString = null, string? multipleConnections = null, string? multipleConnectionsDbType = null)
        {
            var configs = new List<DatabaseConfig>();
            
            // If multiple connections are provided, parse them
            if (!string.IsNullOrEmpty(multipleConnections))
            {
                var connectionData = ParseMultipleConnectionsWithNames(multipleConnections);
                foreach (var (name, connString) in connectionData)
                {
                    if (!string.IsNullOrEmpty(connString))
                    {
                        var sanitized = SanitizeConnectionString(connString);
                        // Use the specified database type for multiple connections, or auto-detect if not specified
                        var dbType = !string.IsNullOrEmpty(multipleConnectionsDbType) 
                            ? ParseDatabaseType(multipleConnectionsDbType)
                            : DetectDatabaseType(sanitized);
                        
                        var config = new DatabaseConfig
                        {
                            Name = name,
                            Type = dbType,
                            ConnectionString = sanitized,
                            IsEnabled = true,
                            TimeoutSeconds = 30
                        };
                        configs.Add(config);
                    }
                }
                return configs;
            }
            
            // If custom connection string is provided, create a custom config
            if (!string.IsNullOrEmpty(customConnectionString))
            {
                var customDbType = ParseDatabaseType(databaseType);
                var customConfig = new DatabaseConfig
                {
                    Name = $"Custom_{databaseType}",
                    Type = customDbType,
                    ConnectionString = customConnectionString,
                    IsEnabled = true,
                    TimeoutSeconds = 30
                };
                configs.Add(customConfig);
                return configs;
            }

            var databasesSection = _configuration.GetSection("Databases");

            foreach (var dbSection in databasesSection.GetChildren())
            {
                var dbType = ParseDatabaseType(dbSection["Type"] ?? "SqlServer");
                var isEnabled = dbSection.GetValue<bool>("IsEnabled", true);
                var timeoutSeconds = dbSection.GetValue<int>("TimeoutSeconds", 30);

                // Filter by database type
                if (databaseType != "both")
                {
                    var requestedType = ParseDatabaseType(databaseType);
                    if (dbType != requestedType)
                        continue;
                }

                // Get all connection strings for this database type
                var connectionStrings = new Dictionary<string, string>();
                
                // Get the main connection string
                var mainConnectionString = dbSection["ConnectionString"];
                if (!string.IsNullOrEmpty(mainConnectionString))
                {
                    connectionStrings["Default"] = mainConnectionString;
                }

                // Get additional connection strings (ConnectionString1, ConnectionString2, etc.)
                foreach (var child in dbSection.GetChildren())
                {
                    if (child.Key.StartsWith("ConnectionString") && !string.IsNullOrEmpty(child.Value))
                    {
                        var connectionName = child.Key == "ConnectionString" ? "Default" : child.Key;
                        connectionStrings[connectionName] = child.Value;
                    }
                }

                // Create individual DatabaseConfig for each connection string
                foreach (var connString in connectionStrings)
                {
                    var config = new DatabaseConfig
                    {
                        Name = $"{dbSection.Key}_{connString.Key}",
                        Type = dbType,
                        ConnectionString = connString.Value,
                        IsEnabled = isEnabled,
                        TimeoutSeconds = timeoutSeconds
                    };

                    if (!string.IsNullOrEmpty(config.ConnectionString))
                    {
                        configs.Add(config);
                    }
                }
            }

            return configs;
        }

        private static DatabaseType ParseDatabaseType(string typeString)
        {
            return typeString.ToLowerInvariant() switch
            {
                "sqlserver" => DatabaseType.SqlServer,
                "postgresql" => DatabaseType.PostgreSQL,
                _ => throw new ArgumentException($"Unsupported database type: {typeString}. Only 'SqlServer' and 'PostgreSQL' are supported.")
            };
        }

        private static DatabaseType DetectDatabaseType(string connectionString)
        {
            var connStr = connectionString.ToLowerInvariant();
            
            if (connStr.Contains("host=") || connStr.Contains("server=") && connStr.Contains("port="))
            {
                return DatabaseType.PostgreSQL;
            }
            else if (connStr.Contains("server=") || connStr.Contains("data source="))
            {
                return DatabaseType.SqlServer;
            }
            else
            {
                // Default to SQL Server if we can't determine
                return DatabaseType.SqlServer;
            }
        }

        private static List<(string Name, string ConnectionString)> ParseMultipleConnectionsWithNames(string input)
        {
            var connections = new List<(string Name, string ConnectionString)>();
            
            try
            {
                // Try to parse as JSON first
                var trimmedInput = input.Trim();
                if (trimmedInput.StartsWith("{") && trimmedInput.EndsWith("}"))
                {
                    // Parse JSON format
                    using var doc = System.Text.Json.JsonDocument.Parse(trimmedInput);
                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        var connectionString = property.Value.GetString();
                        if (!string.IsNullOrEmpty(connectionString))
                        {
                            connections.Add((property.Name, connectionString));
                        }
                    }
                    return connections;
                }
            }
            catch
            {
                // If JSON parsing fails, fall back to line-by-line parsing
            }
            
            // Fall back to line-by-line parsing
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                var trimmedLine = lines[i].Trim();
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                // Support labeled format: "Label: connectionString"
                // Heuristic: treat text before first ':' as label only if it doesn't contain '='
                int colonIndex = trimmedLine.IndexOf(':');
                if (colonIndex > 0)
                {
                    var possibleLabel = trimmedLine.Substring(0, colonIndex).Trim();
                    var possibleConn = trimmedLine.Substring(colonIndex + 1).Trim();
                    possibleLabel = StripQuotes(possibleLabel);
                    possibleConn = StripQuotes(possibleConn);
                    if (!string.IsNullOrEmpty(possibleLabel) && !possibleLabel.Contains('=') && !string.IsNullOrEmpty(possibleConn))
                    {
                        connections.Add((possibleLabel, possibleConn));
                        continue;
                    }
                }

                connections.Add(($"DB_{i + 1}", StripQuotes(trimmedLine)));
            }
            
            return connections;
        }

        private static List<string> ParseMultipleConnections(string input)
        {
            var connections = new List<string>();
            
            try
            {
                // Try to parse as JSON first
                var trimmedInput = input.Trim();
                if (trimmedInput.StartsWith("{") && trimmedInput.EndsWith("}"))
                {
                    // Parse JSON format
                    using var doc = System.Text.Json.JsonDocument.Parse(trimmedInput);
                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        var connectionString = property.Value.GetString();
                        if (!string.IsNullOrEmpty(connectionString))
                        {
                            connections.Add(connectionString);
                        }
                    }
                    return connections;
                }
            }
            catch
            {
                // If JSON parsing fails, fall back to line-by-line parsing
            }
            
            // Fall back to line-by-line parsing
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmedLine = StripQuotes(line.Trim());
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    connections.Add(SanitizeConnectionString(trimmedLine));
                }
            }
            
            return connections;
        }

        private static string StripQuotes(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            string result = value.Trim();
            // Remove surrounding quotes repeatedly if nested
            while (result.Length >= 2 &&
                   ((result.StartsWith("\"") && result.EndsWith("\"")) ||
                    (result.StartsWith("'") && result.EndsWith("'"))))
            {
                result = result.Substring(1, result.Length - 2).Trim();
            }
            return result;
        }

        private static string SanitizeConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            string sanitized = StripQuotes(connectionString).Trim();
            sanitized = sanitized.TrimEnd(';', ',');

            // If input looks like a quoted label mapping without braces and survived here, try to extract RHS
            // e.g. "Default": "Host=..." -> extract after first ':'
            int colonIndex = sanitized.IndexOf(':');
            if (colonIndex > 0 && sanitized.Substring(0, colonIndex).IndexOf('=') < 0)
            {
                string rhs = sanitized.Substring(colonIndex + 1).Trim();
                if (!string.IsNullOrEmpty(rhs))
                {
                    sanitized = StripQuotes(rhs).Trim().TrimEnd(';', ',');
                }
            }

            // Normalize tokens and auto-fix common issues (like 'Keepalive' without value)
            var parts = sanitized.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(p => p.Trim())
                                 .Where(p => !string.IsNullOrEmpty(p))
                                 .ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                // If token has no '=' but is a known key, add default
                if (!part.Contains('='))
                {
                    var key = part.Trim();
                    if (key.Equals("Keepalive", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "Keepalive=30";
                        continue;
                    }
                    // Unknown bare token: drop it to avoid parser crash
                    parts[i] = string.Empty;
                }
                else
                {
                    var kv = part.Split('=', 2);
                    var k = StripQuotes(kv[0].Trim());
                    var v = kv.Length > 1 ? StripQuotes(kv[1].Trim()) : string.Empty;

                    // Normalize some common synonyms (optional)
                    if (k.Equals("User", StringComparison.OrdinalIgnoreCase)) k = "Username";
                    if (k.Equals("Uid", StringComparison.OrdinalIgnoreCase)) k = "Username";
                    if (k.Equals("Pwd", StringComparison.OrdinalIgnoreCase)) k = "Password";
                    if (k.Equals("Addr", StringComparison.OrdinalIgnoreCase)) k = "Host";

                    // Rebuild sanitized pair
                    parts[i] = $"{k}={v}";
                }
            }

            sanitized = string.Join(';', parts.Where(p => !string.IsNullOrEmpty(p)));
            return sanitized;
        }
    }

    
}
