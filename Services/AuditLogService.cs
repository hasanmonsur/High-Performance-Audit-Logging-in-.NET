using AuditLoggingApp.Models;
using Dapper;
using Npgsql;

namespace AuditLoggingApp.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly string _connectionString;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IConfiguration config, ILogger<AuditLogService> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public async Task LogAsync(AuditLog auditLog)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                var sql = @"
                INSERT INTO AuditLogs (Timestamp, UserId, ClientIp, HttpMethod, Path, RequestBody, ResponseStatus, ExecutionDurationMs)
                VALUES (@Timestamp, @UserId, @ClientIp, @HttpMethod, @Path, @RequestBody, @ResponseStatus, @ExecutionDurationMs)";
                await connection.ExecuteAsync(sql, auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save audit log to database. Falling back to file log.");
                // Fallback: Log to file or queue for retry
                await File.AppendAllTextAsync("audit-fallback.log", $"{DateTime.UtcNow}: {auditLog.Path} - {ex.Message}\n");
            }
        }
    }
}
