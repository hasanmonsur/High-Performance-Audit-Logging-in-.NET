namespace AuditLoggingApp.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? ClientIp { get; set; }
        public string HttpMethod { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string? RequestBody { get; set; }
        public int ResponseStatus { get; set; }
        public int ExecutionDurationMs { get; set; }
    }
}
