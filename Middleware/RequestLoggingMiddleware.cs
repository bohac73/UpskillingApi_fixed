using System.Diagnostics;

namespace UpskillingApi.Middleware;

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
        var correlationId = Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Request {Method} {Path} started with CorrelationId={CorrelationId}",
            context.Request.Method, context.Request.Path, correlationId);

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation("Request {Method} {Path} finished with CorrelationId={CorrelationId} in {Elapsed} ms, StatusCode={StatusCode}",
            context.Request.Method,
            context.Request.Path,
            correlationId,
            stopwatch.ElapsedMilliseconds,
            context.Response.StatusCode);
    }
}