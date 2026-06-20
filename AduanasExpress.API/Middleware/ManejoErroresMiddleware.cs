using System.Net;
using System.Text.Json;
using AduanasExpress.API.Middleware;
using AduanasExpress.Infrastructure.Exceptions;
using ValidationException = AduanasExpress.Infrastructure.Exceptions.ValidationException;

namespace AduanasExpress.API.Middleware
{
    public class ManejoErroresMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejoErroresMiddleware> _logger;

        public ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
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
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        // Traduce excepciones del dominio a códigos HTTP y mensajes seguros para el cliente
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, mensajeUsuario, detalleSeguro) = ex switch
            {
                ValidationException => (HttpStatusCode.BadRequest, ex.Message, "Error de validación de datos."),

                KeyNotFoundException => (HttpStatusCode.NotFound, "El recurso solicitado no fue encontrado.", ex.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No tienes autorización para realizar esta acción.", ex.Message),
                ArgumentException => (HttpStatusCode.BadRequest, ex.Message, "Argumento inválido en la petición."),

                _ when ex.GetType().Name == "SqlException" || ex.InnerException?.GetType().Name == "SqlException"
                    => (HttpStatusCode.ServiceUnavailable, "El servicio de datos no está disponible temporalmente. Por favor, inténtelo más tarde.", "Fallo de infraestructura (Base de Datos)."),

                _ => (HttpStatusCode.InternalServerError, "Ha ocurrido un error inesperado en el servidor.", "Error interno no controlado.")
            };

            var response = new MensajeErrorDto
            {
                Estado = (int)statusCode,
                Mensaje = mensajeUsuario,
                Detalle = detalleSeguro
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
