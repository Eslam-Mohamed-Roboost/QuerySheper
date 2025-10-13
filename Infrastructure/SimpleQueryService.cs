using Microsoft.EntityFrameworkCore;
using QuerySheper.Application;
using QuerySheper.Controllers;
using QuerySheper.Models;
using QuerySheper.Persistence;

namespace QuerySheper.Infrastructure
{
    public class SimpleQueryService : ISimpleQueryService
    {
        public async Task<(object Data, int RowCount, TimeSpan ExecutionTime)> ExecuteQueryOnDatabase(DatabaseConfig db, string sql)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            using var context = CreateDbContext(db);
            context.Database.SetCommandTimeout(db.TimeoutSeconds);

            var data = await ExecuteRawQueryAsync(context, sql);

            stopwatch.Stop();
            return (data, GetRowCount(data), stopwatch.Elapsed);
        }

        private async Task<object> ExecuteRawQueryAsync(DbContext context, string sql)
        {
            var trimmedSql = sql.Trim().ToUpperInvariant();
            if (trimmedSql.StartsWith("SELECT") || trimmedSql.StartsWith("WITH"))
            {
                using var command = context.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = context.Database.GetCommandTimeout() ?? 30;

                await context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i) ?? DBNull.Value;
                    }
                    results.Add(row);
                }

                return results;
            }
            else
            {
                var affectedRows = await context.Database.ExecuteSqlRawAsync(sql);
                return new { AffectedRows = affectedRows };
            }
        }

        private int GetRowCount(object data)
        {
            return data switch
            {
                List<Dictionary<string, object>> list => list.Count,
                { } obj when obj.GetType().GetProperty("AffectedRows") != null =>
                    (int)(obj.GetType().GetProperty("AffectedRows")?.GetValue(obj) ?? 0),
                _ => 0
            };
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
    }
}


