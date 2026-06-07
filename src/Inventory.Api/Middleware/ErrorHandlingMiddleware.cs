using Inventory.Application.Common.Exceptions;
using Inventory.Application.Common.Interfaces;
using Inventory.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Inventory.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var cid = context.RequestServices.GetRequiredService<ICorrelationIdProvider>();
            if (ex is FluentValidation.ValidationException fv)
            {
                var errors = fv.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                var validationProblem = new ValidationProblemDetails(errors)
                {
                    Status = (int)HttpStatusCode.BadRequest, 
                    Title = "Validation error",
                    Type = "https://httpstatuses.com/400"
                };
                validationProblem.Extensions["CID"] = cid.CorrelationId;

                context.Response.StatusCode = validationProblem.Status!.Value;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(validationProblem);
                return;

            }
            ProblemDetails problem;
            if (ex is DomainException de)
            {
                problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.UnprocessableEntity,
                    Title = "Domain error",
                    Detail = de.Message,
                    Type = "https://httpstatuses.com/422"
                };
            }
            else if (ex is BadRequestException br)
            {
                problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Detail = br.Message,
                    Type = "https://httpstatuses.com/400"
                };
            }
            else if (ex is NotFoundException nf)
            {
                problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Not Found",
                    Detail = nf.Message,
                    Type = "https://httpstatuses.com/404"
                };
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception", new {CID = cid.FormattedCorrelationId});
                problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Unexpected error",
                    Detail = "An unexpected error occurred.",
                    Type = "https://httpstatuses.com/500"
                };
            }

            problem.Extensions["CID"] = cid.CorrelationId;

            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}

