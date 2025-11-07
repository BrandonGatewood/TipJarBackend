using System.Net;
using System.Text.Json;
using TipJar.Application.Exceptions;

namespace TipJar.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        int statusCode;
        string message;

        if (ex is ApiException apiEx)
        {
            statusCode = apiEx.StatusCode;
            message = apiEx.Message;
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            message = "An unexpected error occurred.";

            _logger.LogError(ex, "Unhandled exception occurred");
        }

        var response = new
        {
            StatusCode = statusCode,
            Message = message
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(payload); 
    }
}