using AuditLoggingApp.Middleware;
using AuditLoggingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddTransient<AuditLoggingMiddleware>();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Apply CORS policy

app.UseRouting();
app.UseAuthentication(); // If using auth
app.UseAuthorization();

app.UseAuditLogging();   // Add custom audit logging middleware
app.MapControllers();

app.Run();