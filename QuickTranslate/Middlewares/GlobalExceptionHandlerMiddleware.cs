using Microsoft.AspNetCore.Mvc;
using QuickTranslate.Exceptions;

namespace QuickTranslate.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception occurred.");

                var statusCode = MapExceptionToStatusCode(ex);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = ex.GetType().Name,
                    Detail = ex.Message,
                    Instance = context.Request.Path 

                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private int MapExceptionToStatusCode(Exception ex)
        {
            var exceptionMapping = new Dictionary<Type, int>
        {
            { typeof(ArgumentException), StatusCodes.Status400BadRequest },
            { typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized },
            { typeof(InvalidOperationException), StatusCodes.Status400BadRequest },
            { typeof(NotImplementedException), StatusCodes.Status501NotImplemented },
            { typeof(KeyNotFoundException), StatusCodes.Status404NotFound },
            { typeof(TimeoutException), StatusCodes.Status408RequestTimeout },
            { typeof(InvalidTranslationDataException), StatusCodes.Status400BadRequest },
            { typeof(NoDataException), StatusCodes.Status404NotFound }
        };

            if (exceptionMapping.ContainsKey(ex.GetType()))
            {
                return exceptionMapping[ex.GetType()];
            }
            return StatusCodes.Status500InternalServerError;
        }
    } 
}
