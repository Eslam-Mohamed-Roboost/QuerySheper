# QuerySheper Local Setup Guide

## Quick Start

### 1. Run the Application
```bash
dotnet run
```

The application will:
- ✅ Auto-launch your browser with Swagger UI
- ✅ Start on `http://localhost:5163`
- ✅ Show the beautiful homepage with interactive dashboard
- ✅ Provide both web interface and API endpoints

### 2. Multiple Database Connections

The system now supports multiple connection strings for each database type:

#### SQL Server (Multiple Databases)
- **Default**: `SqlServer_Default` - Main SQL Server database
- **Additional**: `SqlServer_ConnectionString2`, `SqlServer_ConnectionString3`, etc.
- **Requirements**: SQL Server or SQL Server LocalDB installed
- **Default**: Uses Windows Authentication

#### PostgreSQL (Multiple Databases)
- **Default**: `PostgreSQL_Default` - Main PostgreSQL database  
- **Additional**: `PostgreSQL_ConnectionString2`, `PostgreSQL_ConnectionString3`, etc.
- **Requirements**: PostgreSQL installed locally
- **Default**: Username `postgres`, Password `password`

### 3. Update Your Connection Strings

Edit `appsettings.json` and update the connection strings:

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

### 4. Test with SQL Files

1. **Create SQL files** in the `Scripts` folder
2. **Use Swagger UI** to test the endpoints:
   - `GET /api/databasequery/sql-files` - List your SQL files
   - `POST /api/databasequery/execute-from-file` - Execute SQL from files

### 5. Example SQL File

Create `Scripts/my-query.sql`:
```sql
-- Your SQL query here
SELECT 
    'Hello World' as Message,
    GETDATE() as CurrentTime;
```

Then execute it via API:
```json
POST /api/databasequery/execute-from-file
{
  "filePath": "Scripts/my-query.sql",
  "databaseNames": ["SqlServer_Default", "PostgreSQL_Default"],
  "continueOnError": true
}
```

### 6. Multiple Database Execution

Execute the same SQL on multiple databases:

```json
POST /api/databasequery/execute-from-file
{
  "filePath": "Scripts/my-query.sql",
  "databaseNames": [
    "SqlServer_Default", 
    "SqlServer_ConnectionString2", 
    "SqlServer_ConnectionString3",
    "PostgreSQL_Default",
    "PostgreSQL_ConnectionString2"
  ],
  "continueOnError": true
}
```

## Features

- 🚀 **Auto-launch**: Browser opens automatically with beautiful web interface
- 🌐 **Web Interface**: Interactive dashboard for easy query execution
- 📁 **File-based**: Execute SQL from local files
- 🗄️ **Multi-DB**: Run same query on multiple databases
- ⚡ **Error Handling**: Catch errors on each database individually
- 📊 **Results**: Detailed results from all databases
- 🎨 **Modern UI**: Beautiful, responsive web interface
- 📖 **API Documentation**: Complete Swagger documentation

## Troubleshooting

### Connection Issues
1. Check if your database server is running
2. Verify connection strings in `appsettings.json`
3. Test connections using `GET /api/databasequery/test-connections`

### File Issues
1. Ensure SQL files are in the `Scripts` folder
2. Use relative paths: `Scripts/my-query.sql`
3. Check file permissions

### Browser Launch Issues
If browser doesn't auto-launch:
1. Manually go to `http://localhost:5163`
2. The Swagger UI will be available there

## Web Interface & API Endpoints

### Web Interface
- `GET /` - Beautiful homepage with overview and quick links
- `GET /dashboard.html` - Interactive dashboard for query execution
- `GET /swagger` - API documentation (Swagger UI)

### Static Assets
- `GET /css/style.css` - Custom styling
- `GET /js/app.js` - Interactive JavaScript functionality
- `GET /config.json` - Application configuration

### API Endpoints
- `GET /api/databasequery/databases` - List available databases
- `GET /api/databasequery/sql-files` - List SQL files
- `POST /api/databasequery/execute-from-file` - Execute SQL from file
- `POST /api/databasequery/execute` - Execute SQL directly
- `GET /api/databasequery/test-connections` - Test database connections

## Usage Options

1. **Web Interface**: Use the beautiful dashboard at `http://localhost:5163/dashboard.html`
2. **API Only**: Use Swagger documentation at `http://localhost:5163/swagger`
3. **Direct API**: Make HTTP requests to the API endpoints
4. **HTTP Files**: Use the provided `QuerySheper.http` file for testing

Happy querying! 🎉
