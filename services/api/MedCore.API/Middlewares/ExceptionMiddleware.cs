using System.Net;
using System.Text.Json;
using MedCore.Shared.Exceptions;
using MedCore.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedCore.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "INTERNAL_ERROR";
        var message = "An internal server error occurred.";

        if (exception is BaseException baseException)
        {
            errorCode = baseException.ErrorCode;
            message = baseException.Message;

            statusCode = exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                ValidationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.BadRequest
            };
        }

        context.Response.StatusCode = (int)statusCode;
        var result = Result<object>.Failure(message, errorCode);
        var correlationId = context.TraceIdentifier;

        var response = new 
        {
            result.IsSuccess,
            result.ErrorMessage,
            result.ErrorCode,
            result.Value,
            CorrelationId = correlationId
        };

        if (exception is ValidationException validationException)
        {
            // Future: Expand Result to carry validation errors if needed.
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
