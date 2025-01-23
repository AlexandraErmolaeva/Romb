using Newtonsoft.Json;
using Romb.Application.Exceptions;
using StackExchange.Redis;
using System.Net;

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
            EventCofinanceRateIncorrectValueException => HttpStatusCode.BadRequest,
            EventTotalBudgetIncorrectValueException => HttpStatusCode.BadRequest,
            EventCalculatingBudgetException => HttpStatusCode.BadRequest,
            RedisException => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.InternalServerError 
        };

        var errorResponse = new
        {
            message = exception.Message,
            statusCode = (int)statusCode
        };

        var errorJson = JsonConvert.SerializeObject(errorResponse);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(errorJson);
    }
}
