namespace QuerySheper.Models
{
    public class DatabaseConfig
    {
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; } = DatabaseType.SqlServer;
        public string ConnectionString { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }

    public class DatabaseConnectionConfig
    {
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; } = DatabaseType.SqlServer;
        public Dictionary<string, string> ConnectionStrings { get; set; } = new();
        public bool IsEnabled { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }
}
