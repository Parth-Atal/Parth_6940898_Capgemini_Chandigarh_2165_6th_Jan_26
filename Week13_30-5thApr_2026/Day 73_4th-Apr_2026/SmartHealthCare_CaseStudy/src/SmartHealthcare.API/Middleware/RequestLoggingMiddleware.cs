using System.Diagnostics;

namespace SmartHealthcare.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Incoming Request: {Method} {Path} at {Time}", context.Request.Method, context.Request.Path, DateTime.UtcNow);
        await _next(context);
        stopwatch.Stop();
        _logger.LogInformation("Outgoing Response: {StatusCode} for {Method} {Path} in {Elapsed}ms", context.Response.StatusCode, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
}