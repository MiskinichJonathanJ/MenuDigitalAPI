using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Application.Exceptions;
using FluentValidation;

namespace Infrastructure.Middleware
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
            // 1. Generar e inyectar Correlation ID
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            context.Response.Headers.Append("X-Correlation-ID", correlationId);

            // 2. Extraer User ID si está logueado
            var userId = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous";

            // 3. Enriquecer el contexto de Serilog para TODO lo que pase en esta request
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["UserId"] = userId
            }))
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex, correlationId);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message = "Ocurrió un error procesando la solicitud.";
            List<string> errors = new List<string>();

            switch (exception)
            {
                case FluentValidation.ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = "Error de validación.";
                    errors = validationEx.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validación fallida: {Errors}", JsonSerializer.Serialize(errors));
                    break;

                case NotFoundException notFoundEx:
                    statusCode = StatusCodes.Status404NotFound;
                    message = notFoundEx.Message;
                    _logger.LogWarning("Recurso no encontrado: {Message}", notFoundEx.Message);
                    break;

                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    _logger.LogError(exception, "Error interno no manejado capturado en el middleware.");
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                CorrelationId = correlationId,
                Message = message,
                Errors = errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}