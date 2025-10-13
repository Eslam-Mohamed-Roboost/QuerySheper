namespace QuerySheper.Models
{
    public class QueryRequest
    {
        public string SqlQuery { get; set; } = string.Empty;
        public string DatabaseType { get; set; } = "both"; // "both", "sqlserver", "postgresql"
        public string? CustomConnectionString { get; set; }
        public string? MultipleConnections { get; set; }
        public string? MultipleConnectionsDbType { get; set; }
    }
}
