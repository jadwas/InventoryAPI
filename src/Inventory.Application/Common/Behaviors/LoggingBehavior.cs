using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Inventory.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;

    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} with data: {@Request}", requestName);

        try
        {
            var response = await next(cancellationToken);

            sw.Stop();

            _logger.LogInformation("Handled {RequestName} in {Elapsed}ms with response: {@Response}", requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            _logger.LogError(ex, "Error handling {RequestName} after {Elapsed} ms Request: {@Request}", requestName, sw.ElapsedMilliseconds);

            throw;
        }
    }
}