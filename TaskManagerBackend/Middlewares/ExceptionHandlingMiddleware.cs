using System.Text.Json;
using TaskManagerBackend.Exceptions;

namespace TaskManagerBackend.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";

                var statusCode = ex is AppException appEx
                    ? appEx.StatusCode
                    : StatusCodes.Status500InternalServerError;

                var response = new
                {
                    success = false,
                    error = ex.Message
                };

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

}
