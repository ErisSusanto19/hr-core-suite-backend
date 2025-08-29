using System.Net;
using System.Text.Json;

namespace HRCoreSuite.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred for request {Path}", context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            object errorResponse;

            if (_env.IsDevelopment())
            {
                errorResponse = new
                {
                    title = "An unexpected error occurred.",
                    status = context.Response.StatusCode,
                    detail = ex.Message,
                    stackTrace = ex.StackTrace
                };
            }
            else
            {
                errorResponse = new
                {
                    title = "An internal server error has occurred.",
                    status = context.Response.StatusCode
                };
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
