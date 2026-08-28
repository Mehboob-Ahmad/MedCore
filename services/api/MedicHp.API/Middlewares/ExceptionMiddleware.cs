using System.Net;
using System.Text.Json;
using MedicHp.Shared.Exceptions;
using MedicHp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MedicHp.API.Middlewares;

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
        else if (exception is UnauthorizedAccessException authEx)
        {
            statusCode = HttpStatusCode.Unauthorized;
            errorCode = "UNAUTHORIZED";
            message = authEx.Message; // Should be "Invalid credentials."
        }

        context.Response.StatusCode = (int)statusCode;
        var result = Result<object>.Failure(message, errorCode);
        var correlationId = context.TraceIdentifier;

        var response = new 
        {
            success = result.IsSuccess,
            message = result.ErrorMessage,
            errorCode = result.ErrorCode,
            data = result.Value,
            correlationId = correlationId
        };

        if (exception is ValidationException validationException)
        {
            // Future: Expand Result to carry validation errors if needed.
        }

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
