using ProductAPI.Wrappers;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ProductAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred. RequestPath: {RequestPath}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>();

            switch (exception)
            {
                case ValidationException validationEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Validation failed",
                        new List<string> { validationEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentNullException argumentNullEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Invalid request",
                        new List<string> { argumentNullEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case ArgumentException argumentEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Invalid argument",
                        new List<string> { argumentEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Resource not found",
                        new List<string> { keyNotFoundEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case UnauthorizedAccessException unauthorizedEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Unauthorized access",
                        new List<string> { unauthorizedEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case InvalidOperationException invalidOpEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Invalid operation",
                        new List<string> { invalidOpEx.Message });
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case TimeoutException timeoutEx:
                    response = ApiResponse<object>.ErrorResponse(
                        "Request timeout",
                        new List<string> { "The operation has timed out. Please try again." });
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    break;

                default:
                   
                    var errorMessage = _environment.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred.";

                    var errorDetails = _environment.IsDevelopment()
                        ? new List<string> { exception.Message, exception.StackTrace ?? string.Empty }
                        : new List<string> { "Please contact support if the problem persists." };

                    response = ApiResponse<object>.ErrorResponse(errorMessage, errorDetails);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

           
            var correlationId = context.TraceIdentifier;
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            
            if (context.Response.StatusCode >= 500)
            {
                _logger.LogError(exception,
                    "Server error occurred. CorrelationId: {CorrelationId}, StatusCode: {StatusCode}, RequestPath: {RequestPath}",
                    correlationId,
                    context.Response.StatusCode,
                    context.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception,
                    "Client error occurred. CorrelationId: {CorrelationId}, StatusCode: {StatusCode}, RequestPath: {RequestPath}",
                    correlationId,
                    context.Response.StatusCode,
                    context.Request.Path);
            }

            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var result = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(result);
        }
    }

    
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }

   
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(int productId)
            : base($"Product with ID {productId} was not found.")
        {
        }

        public ProductNotFoundException(string message) : base(message)
        {
        }

        public ProductNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ProductValidationException : ValidationException
    {
        public ProductValidationException(string message) : base(message)
        {
        }

        public ProductValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
