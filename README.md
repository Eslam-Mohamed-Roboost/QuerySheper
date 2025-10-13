# QuerySheper - Multi-Database Query System

QuerySheper is an ASP.NET Core Web API that allows you to execute SQL queries across multiple databases simultaneously with comprehensive error handling.

## Features

- **Multi-Database Support**: Execute queries on SQL Server, PostgreSQL, MySQL, and In-Memory databases
- **Comprehensive Error Handling**: Catch and report errors from each database individually
- **Parallel Execution**: Queries run in parallel across all databases for optimal performance
- **Flexible Configuration**: Configure database connections through appsettings.json
- **Timeout Management**: Set per-database and per-query timeouts
- **Detailed Results**: Get execution time, row counts, and error details for each database

## Database Configuration

Configure your databases in `appsettings.json`:

```json
{
  "Databases": {
    "SqlServerDb": {
      "Type": "SqlServer",
      "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=QuerySheperDb;Trusted_Connection=true;MultipleActiveResultSets=true",
      "IsEnabled": true,
      "TimeoutSeconds": 30
    },
    "PostgreSqlDb": {
      "Type": "PostgreSQL",
      "ConnectionString": "Host=localhost;Database=querysheper;Username=postgres;Password=password",
      "IsEnabled": true,
      "TimeoutSeconds": 30
    },
    "MySqlDb": {
      "Type": "MySQL",
      "ConnectionString": "Server=localhost;Database=querysheper;Uid=root;Pwd=password;",
      "IsEnabled": true,
      "TimeoutSeconds": 30
    },
    "InMemoryDb": {
      "Type": "InMemory",
      "ConnectionString": "",
      "IsEnabled": true,
      "TimeoutSeconds": 10
    }
  }
}
```

### Supported Database Types

The system supports the following database types (case-insensitive):

- **SqlServer** - Microsoft SQL Server
- **PostgreSQL** - PostgreSQL database
- **MySQL** - MySQL database  
- **InMemory** - In-memory database for testing

## API Endpoints

### 1. Execute Query (POST)
Execute a SQL query across multiple databases with full error handling.

**Endpoint**: `POST /api/databasequery/execute`

**Request Body**:
```json
{
  "sqlQuery": "SELECT 1 as Id, 'Test' as Name, GETDATE() as CurrentTime",
  "databaseNames": ["SqlServerDb", "InMemoryDb"],
  "timeoutSeconds": 30,
  "continueOnError": true
}
```

**Response**:
```json
{
  "overallSuccess": true,
  "totalDatabases": 2,
  "successfulDatabases": 2,
  "failedDatabases": 0,
  "totalExecutionTime": "00:00:00.1234567",
  "executedAt": "2024-01-15T10:30:00.000Z",
  "results": [
    {
      "databaseName": "SqlServerDb",
      "isSuccess": true,
      "data": [
        { "Id": 1, "Name": "Test", "CurrentTime": "2024-01-15T10:30:00.000Z" }
      ],
      "rowCount": 1,
      "executionTime": "00:00:00.0500000",
      "executedAt": "2024-01-15T10:30:00.000Z"
    },
    {
      "databaseName": "InMemoryDb",
      "isSuccess": true,
      "data": [
        { "Id": 1, "Name": "Test", "CurrentTime": "2024-01-15T10:30:00.000Z" }
      ],
      "rowCount": 1,
      "executionTime": "00:00:00.0300000",
      "executedAt": "2024-01-15T10:30:00.000Z"
    }
  ]
}
```

### 2. Execute SELECT Query (GET)
Quick way to execute SELECT queries via GET request.

**Endpoint**: `GET /api/databasequery/select`

**Query Parameters**:
- `sqlQuery` (required): The SQL SELECT query
- `databaseNames` (optional): Comma-separated list of database names
- `timeoutSeconds` (optional): Query timeout in seconds

**Example**: `/api/databasequery/select?sqlQuery=SELECT 1 as TestValue&databaseNames=SqlServerDb,InMemoryDb`

### 3. Get Available Databases
Get list of all configured and available databases.

**Endpoint**: `GET /api/databasequery/databases`

### 4. Test Connections
Test connectivity to all configured databases.

**Endpoint**: `GET /api/databasequery/test-connections`

### 5. Execute SQL from File (POST)
Execute SQL queries stored in files across multiple databases.

**Endpoint**: `POST /api/databasequery/execute-from-file`

**Request Body**:
```json
{
  "filePath": "C:\\Scripts\\query.sql",
  "databaseNames": ["SqlServerDb", "InMemoryDb"],
  "timeoutSeconds": 30,
  "continueOnError": true
}
```

### 6. Get SQL Files (GET)
List all available SQL files in a directory.

**Endpoint**: `GET /api/databasequery/sql-files`

**Query Parameters**:
- `directoryPath` (optional): Directory to scan for SQL files. Defaults to `Scripts` folder.

### 7. Validate SQL File (GET)
Validate a SQL file without executing it.

**Endpoint**: `GET /api/databasequery/validate-file`

**Query Parameters**:
- `filePath` (required): Path to the SQL file to validate

## Error Handling

The system provides comprehensive error handling for various scenarios:

### Database-Specific Errors
- **Connection Errors**: Database unavailable or connection string issues
- **SQL Errors**: Syntax errors, invalid queries, permission issues
- **Timeout Errors**: Queries exceeding the specified timeout
- **Data Type Errors**: Incompatible data types between databases

### Error Response Format
```json
{
  "overallSuccess": false,
  "totalDatabases": 2,
  "successfulDatabases": 1,
  "failedDatabases": 1,
  "results": [
    {
      "databaseName": "SqlServerDb",
      "isSuccess": true,
      "data": [...],
      "rowCount": 1,
      "executionTime": "00:00:00.0500000"
    },
    {
      "databaseName": "PostgreSqlDb",
      "isSuccess": false,
      "errorMessage": "SQL Error 208: Invalid object name 'NonExistentTable'",
      "executionTime": "00:00:00.0100000"
    }
  ]
}
```

## Running the Application

1. **Install Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Configure Databases**: Update connection strings in `appsettings.json`

3. **Run the Application**:
   ```bash
   dotnet run
   ```

4. **Test the API**: Use the provided `QuerySheper.http` file or any HTTP client

## Example Usage Scenarios

### 1. Execute SQL from File
```json
{
  "filePath": "C:\\Scripts\\user-report.sql",
  "databaseNames": ["SqlServerDb", "PostgreSqlDb"],
  "continueOnError": true
}
```

### 2. Cross-Database Data Comparison
```json
{
  "sqlQuery": "SELECT COUNT(*) as UserCount FROM Users WHERE CreatedDate > '2024-01-01'",
  "continueOnError": true
}
```

### 3. Database Health Check
```json
{
  "sqlQuery": "SELECT @@VERSION as Version, DB_NAME() as DatabaseName, GETDATE() as CurrentTime",
  "continueOnError": true
}
```

### 4. Data Migration Verification
```json
{
  "sqlQuery": "SELECT COUNT(*) as RecordCount FROM Orders WHERE Status = 'Completed'",
  "databaseNames": ["SourceDb", "TargetDb"],
  "continueOnError": true
}
```

## File-Based Operations

### SQL File Management
- **Supported Extensions**: `.sql`, `.txt`
- **Maximum File Size**: 10MB (configurable)
- **Default Directory**: `Scripts` folder in project root
- **File Validation**: Automatic SQL syntax checking

### Example SQL Files
The project includes sample SQL files in the `Scripts` directory:
- `example-select.sql` - Basic SELECT queries for testing
- `user-count.sql` - User counting with error handling
- `database-info.sql` - Database version and info queries

### File Execution Features
- **Error Isolation**: File read errors don't affect database execution
- **Content Preview**: See the SQL content that was executed
- **File Metadata**: Track file size, modification date, and validation status
- **Batch Processing**: Execute the same SQL across multiple databases

## Security Considerations

- **SQL Injection Prevention**: Use parameterized queries when possible
- **Connection String Security**: Store sensitive connection strings in secure configuration
- **Access Control**: Implement proper authentication and authorization
- **Query Validation**: Consider implementing query validation rules

## Performance Tips

- **Parallel Execution**: Queries run in parallel across databases
- **Timeout Management**: Set appropriate timeouts for different query types
- **Connection Pooling**: Entity Framework handles connection pooling automatically
- **Selective Database Execution**: Use `databaseNames` to limit execution to specific databases

## Troubleshooting

### Common Issues

1. **Database Connection Errors**: Verify connection strings and database availability
2. **Timeout Errors**: Increase timeout values or optimize queries
3. **SQL Compatibility**: Some SQL features may not work across all database types
4. **Permission Errors**: Ensure database user has necessary permissions

### Logging

The application provides detailed logging for debugging:
- Database connection attempts
- Query execution times
- Error details and stack traces
- Performance metrics

Check the console output or configured logging providers for detailed information.
