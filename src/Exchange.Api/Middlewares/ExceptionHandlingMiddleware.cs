using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Api.Middlewares;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = new ProblemDetails
            {
                Instance = context.Request.HttpContext.Request.Path,
                Extensions = { new KeyValuePair<string, object?>("traceId", context.TraceIdentifier) },
            };
            
            switch (exception)
            {
                default:
                    response.Title = "Internal server error";
                    response.Status = StatusCodes.Status500InternalServerError;
                    response.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                    break;
            }
            
            context.Response.StatusCode = response.Status.Value;
            context.Response.ContentType = "application/problem+json";
            
            _logger.LogError(exception, @"Exception thrown in method: {Method} with trace identifier: {TraceIdentifier} was successfully handled", context.Request.Method, context.TraceIdentifier);

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}