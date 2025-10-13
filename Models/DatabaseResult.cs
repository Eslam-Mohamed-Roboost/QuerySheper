namespace QuerySheper.Models
{
    public class DatabaseResult
    {
        public string DatabaseName { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
        public object? Data { get; set; }
        public int RowCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
