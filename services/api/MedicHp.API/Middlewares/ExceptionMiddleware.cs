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

        if (exception is ValidationException validationException && validationException.Errors.Any())
        {
            var firstError = validationException.Errors.First();
            message = firstError.Value.FirstOrDefault() ?? message;
        }

        context.Response.StatusCode = (int)statusCode;
        var result = Result<object>.Failure(message, errorCode);
        var correlationId = context.TraceIdentifier;

        var response = new 
        {
            success = result.IsSuccess,
            message = message,
            errorCode = result.ErrorCode,
            data = result.Value,
            correlationId = correlationId
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
