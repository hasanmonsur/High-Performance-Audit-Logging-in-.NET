using AuditLoggingApp.Models;
using AuditLoggingApp.Services;
using System.Diagnostics;

namespace AuditLoggingApp.Middleware
{
    public class AuditLoggingMiddleware : IMiddleware
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(IAuditLogService auditLogService, ILogger<AuditLoggingMiddleware> logger)
        {
            _auditLogService = auditLogService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            request.EnableBuffering();
            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                UserId = context.User.Identity?.Name ?? "Anonymous",
                ClientIp = context.Connection.RemoteIpAddress?.ToString(),
                HttpMethod = request.Method,
                Path = request.Path,
                RequestBody = requestBody.Length > 0 ? requestBody : null
            };

            try
            {
                await next(context);

                auditLog.ResponseStatus = context.Response.StatusCode;
                auditLog.ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds;

                await _auditLogService.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in request pipeline for {Path}", auditLog.Path);
                auditLog.ResponseStatus = 500;
                auditLog.ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds;
                await _auditLogService.LogAsync(auditLog);
                throw;
            }
        }
    }

    public static class AuditLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLoggingMiddleware>();
        }
    }
}
