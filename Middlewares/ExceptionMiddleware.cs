using System.Net;
using WebApi.Errors;

namespace WebApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        ILogger<ExceptionMiddleware> logger,
        RequestDelegate next,
        IHostEnvironment env)
    {
        _logger = logger;
        _next = next;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            ApiError response;
            var statusCode = HttpStatusCode.InternalServerError;
            var message = string.Empty;
            var exceptionType = ex.GetType();

            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                statusCode = HttpStatusCode.Forbidden;
                message = "You are not authorized";
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
                message = "Some unknown error occurred";
            }

            if (_env.IsDevelopment())
            {
                response = new ApiError((int)statusCode, ex.Message, ex.StackTrace.ToString());
            }
            else
            {
                response = new ApiError((int)statusCode, message);
            }

            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response.ToString());
        }
    }
}