using Newtonsoft.Json;
using Romb.Application.Exceptions;
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
            // finally блок используется, чтобы зафиксировать завершение обработки запроса.
            _logger.LogInformation("Finished handling request: {Method}, {Path}.", context.Request.Method, context.Request.Path);
        }
    }

    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound, // 404.
            ArgumentException => HttpStatusCode.BadRequest, // 400.
            EventCofinanceRateIncorrectValueException => HttpStatusCode.BadRequest, // 400.
            EventTotalBudgetIncorrectValueException => HttpStatusCode.BadRequest, // 400.
            EventCalculatingBudgetException => HttpStatusCode.BadRequest, // 400.
            _ => HttpStatusCode.InternalServerError // 500.
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
