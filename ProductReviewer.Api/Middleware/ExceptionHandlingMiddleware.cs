using FluentResults;
using System.Text.Json;

namespace ProductReviewer.Api.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResult = Result.Fail("An unexpected error occurred. Please try again later.");

                var errorResponse = JsonSerializer.Serialize(new
                {
                    message = errorResult.Errors.FirstOrDefault()?.Message
                });

                await context.Response.WriteAsync(errorResponse);
            }
        }
    }
}
