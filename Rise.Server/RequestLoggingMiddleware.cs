namespace Rise.Server
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log the request path and method
            _logger.LogInformation("Incoming Request: {Method} {Path}", context.Request.Method, context.Request.Path);

            // Log each header
            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation("Header: {Key}: {Value}", header.Key, header.Value.ToString());
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
