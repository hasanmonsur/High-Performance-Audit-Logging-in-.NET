using AuditLoggingApp.Models;

namespace AuditLoggingApp.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditLog auditLog);
    }
}
