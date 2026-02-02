using PFS.Application.Execeptions;
using System.Net;
using System.Text.Json;

namespace PFS.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    NotFoundException => HttpStatusCode.NotFound,
                    AlreadyExisitException => HttpStatusCode.BadRequest,
                    UnAuthorizedException => HttpStatusCode.Unauthorized,
                    
                };

                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    success = false,
                    message = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
