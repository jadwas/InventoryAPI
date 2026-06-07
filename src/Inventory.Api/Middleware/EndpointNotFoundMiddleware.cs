using Inventory.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Inventory.Api.Middleware
{
    public class EndpointNotFoundMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<EndpointNotFoundMiddleware> _logger;
        

        public EndpointNotFoundMiddleware(RequestDelegate next, ILogger<EndpointNotFoundMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
            var cid = context.RequestServices.GetRequiredService<ICorrelationIdProvider>();

            if (context.Response is { StatusCode: 404, HasStarted: false } && context.GetEndpoint() == null)
            {
                _logger.LogWarning("Endpoint not found: {Path}", context.Request.Path);

                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Endpoint not found",
                    Detail = $"The endpoint '{context.Request.Path}' does not exist.",
                    Type = "https://httpstatuses.com/404",
                    Extensions =
                    {
                        ["CID"] = cid.CorrelationId
                    }
                };

                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
