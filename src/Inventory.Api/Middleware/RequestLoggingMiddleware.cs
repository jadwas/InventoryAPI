using Inventory.Application.Common.Interfaces;
using Serilog.Context;

namespace Inventory.Api.Middleware;

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
        var cid = context.RequestServices.GetRequiredService<ICorrelationIdProvider>().FormattedCorrelationId;
        using (LogContext.PushProperty("CID", cid))
        {
            _logger.LogInformation("Starting {Method} {Path}", context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("Finished {StatusCode}", context.Response.StatusCode);

        }
    }
}