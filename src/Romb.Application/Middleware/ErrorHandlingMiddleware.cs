using Romb.Application.Exceptions;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace Romb.Application.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _nextDelegate;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _nextDelegate = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("Handling request: {Method}, {Path}", context.Request.Method, context.Request.Path);

            await _nextDelegate(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while processing the request: {Path}.", context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
        finally
        {
            _logger.LogInformation("Finished handling request: {Method}, {Path}.", context.Request.Method, context.Request.Path);
        }
    }

    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound, 
            ArgumentException => HttpStatusCode.BadRequest, 
            CofinanceRateIncorrectValueException => HttpStatusCode.BadRequest,
            TotalBudgetIncorrectValueException => HttpStatusCode.BadRequest,
            CalculatingBudgetException => HttpStatusCode.BadRequest,
            RedisException => HttpStatusCode.ServiceUnavailable,
            EntityNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError 
        };

        var errorResponse = new
        {
            message = exception.Message,
            statusCode = (int)statusCode
        };

        var errorJson = JsonSerializer.Serialize(errorResponse);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(errorJson);
    }
}
