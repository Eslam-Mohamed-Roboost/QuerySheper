# QuerySheper - Simple Multi-Database Query System

## Overview

QuerySheper is now simplified to have **ONE ENDPOINT** that executes SQL queries across **ALL** configured database connections from `appsettings.json`.

## Features

- ✅ **One Endpoint**: Single API endpoint for all operations
- ✅ **All Databases**: Automatically executes on all configured databases
- ✅ **Error Handling**: Shows success/failure for each database individually
- ✅ **Auto-launch**: Browser opens automatically with Swagger UI
- ✅ **Simple Setup**: Just configure connection strings and run

## Quick Start

### 1. Configure Your Databases

Edit `appsettings.json` with your actual connection strings:

```json
{
  "Databases": {
    "SqlServer": {
      "Type": "SqlServer",
      "IsEnabled": true,
      "TimeoutSeconds": 30,
      "ConnectionString": "YOUR_SQL_SERVER_CONNECTION_STRING_HERE",
      "ConnectionString2": "YOUR_SECOND_SQL_SERVER_CONNECTION_STRING_HERE",
      "ConnectionString3": "YOUR_THIRD_SQL_SERVER_CONNECTION_STRING_HERE"
    },
    "PostgreSQL": {
      "Type": "PostgreSQL",
      "IsEnabled": true,
      "TimeoutSeconds": 30,
      "ConnectionString": "YOUR_POSTGRESQL_CONNECTION_STRING_HERE",
      "ConnectionString2": "YOUR_SECOND_POSTGRESQL_CONNECTION_STRING_HERE",
      "ConnectionString3": "YOUR_THIRD_POSTGRESQL_CONNECTION_STRING_HERE"
    }
  }
}
```

### 2. Run the Application

```bash
dotnet run
```

The browser will automatically open to `http://localhost:5163` with Swagger UI.

### 3. Execute SQL Queries

**Single Endpoint**: `POST /api/simplequery/execute`

**Request Body**: Just the SQL query as a string:
```json
"SELECT 1 as TestValue, 'Hello QuerySheper!' as Message"
```

**Response**: Detailed results from all databases:
```json
{
  "sqlQuery": "SELECT 1 as TestValue, 'Hello QuerySheper!' as Message",
  "totalExecutionTime": "00:00:02.1234567",
  "totalDatabases": 7,
  "successfulDatabases": 5,
  "failedDatabases": 2,
  "results": [
    {
      "databaseName": "SqlServer_Default",
      "success": true,
      "data": [{"TestValue": 1, "Message": "Hello QuerySheper!"}],
      "rowCount": 1,
      "executionTime": "00:00:00.5000000"
    },
    {
      "databaseName": "PostgreSQL_Default",
      "success": false,
      "error": "Connection failed",
      "executionTime": "00:00:00"
    }
  ]
}
```

## Supported Database Types

- **SQL Server**: `Server=localhost;Database=YourDB;Trusted_Connection=true;TrustServerCertificate=true`
- **PostgreSQL**: `Host=localhost;Database=yourdb;Username=postgres;Password=password`

## Usage Examples

### Test Query
```json
"SELECT 1 as TestValue, 'Hello QuerySheper!' as Message"
```

### Database Information
```json
"SELECT @@VERSION as DatabaseVersion, DB_NAME() as DatabaseName"
```

### User Count (if Users table exists)
```json
"SELECT COUNT(*) as UserCount FROM Users"
```

### Error Test
```json
"SELECT * FROM NonExistentTable"
```

## How It Works

1. **Configure**: Set up connection strings in `appsettings.json`
2. **Execute**: Send SQL query to the single endpoint
3. **Results**: Get detailed results from all configured databases
4. **Error Handling**: See which databases succeeded and which failed

## API Endpoints

- `GET /` - Swagger UI (auto-launches)
- `POST /api/simplequery/execute` - Execute SQL on all databases

## Configuration

The system automatically:
- ✅ Loads all connection strings from `appsettings.json`
- ✅ Creates database contexts for each connection
- ✅ Executes the SQL query on all databases in parallel
- ✅ Returns detailed results with success/failure status
- ✅ Shows execution times and error messages

## Perfect For

- **Database Comparisons**: Run the same query across multiple environments
- **Health Checks**: Test connectivity to all databases
- **Data Validation**: Compare results across databases
- **Maintenance Tasks**: Execute scripts on all databases
- **Reporting**: Aggregate data from multiple sources

## Simple and Powerful! 🚀

Just configure your connection strings, run the app, and execute SQL queries across all your databases with a single API call!
