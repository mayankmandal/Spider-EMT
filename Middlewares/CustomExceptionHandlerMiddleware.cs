using Spider_EMT.Models;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

namespace Spider_EMT.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var errorLogRepository = scope.ServiceProvider.GetRequiredService<IErrorLogRepository>();
            
                var errorLog = new ErrorLog
                {
                    ErrorMessage = exception.Message,
                    StackTrace = exception.StackTrace,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                };
                var logId = await errorLogRepository.LogErrorAsync(errorLog);
                context.Response.Redirect($"/Error?logId={logId}");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
