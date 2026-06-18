using AduanasExpress.API.Middleware;
using AduanasExpress.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace AduanasExpress.Tests.Middleware
{
    public class SqlException : Exception { }

    public class ManejoErroresMiddlewareTests
    {
        private readonly ILogger<AduanasExpress.API.Middleware.ManejoErroresMiddleware> _logger;

        public ManejoErroresMiddlewareTests()
        {
            _logger = NullLogger<AduanasExpress.API.Middleware.ManejoErroresMiddleware>.Instance;
        }

        [Fact]
        public async Task InvokeAsync_CuandoOcurreValidationException_DebeDevolverBadRequest()
        {
            var contexto = new DefaultHttpContext();
            contexto.Response.Body = new MemoryStream();
            string mensajeValidacion = "El email no tiene un formato valido.";

            RequestDelegate siguientePaso = (ctx) => throw new ValidationException(mensajeValidacion);
            var middleware = new AduanasExpress.API.Middleware.ManejoErroresMiddleware(siguientePaso, _logger);

            await middleware.InvokeAsync(contexto);

            Assert.Equal((int)HttpStatusCode.BadRequest, contexto.Response.StatusCode);
            contexto.Response.Body.Seek(0, SeekOrigin.Begin);
            var cuerpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
            Assert.Contains("El email no tiene un formato valido.", cuerpo);
            Assert.Contains("Error de validaci", cuerpo);
        }

        [Fact]
        public async Task InvokeAsync_CuandoOcurreSqlException_DebeDevolverServiceUnavailable()
        {
            var contexto = new DefaultHttpContext();
            contexto.Response.Body = new MemoryStream();

            RequestDelegate siguientePaso = (ctx) => throw new SqlException();
            var middleware = new AduanasExpress.API.Middleware.ManejoErroresMiddleware(siguientePaso, _logger);

            await middleware.InvokeAsync(contexto);

            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, contexto.Response.StatusCode);
            contexto.Response.Body.Seek(0, SeekOrigin.Begin);
            var cuerpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
            Assert.Contains("El servicio de datos no est", cuerpo);
            Assert.Contains("Fallo de infraestructura (Base de Datos).", cuerpo);
        }

        [Fact]
        public async Task InvokeAsync_CuandoOcurreErrorInesperado_DebeDevolverInternalServerError()
        {
            var contexto = new DefaultHttpContext();
            contexto.Response.Body = new MemoryStream();
            string errorSecreto = "Fallo critico en el servidor.";

            RequestDelegate siguientePaso = (ctx) => throw new Exception(errorSecreto);
            var middleware = new AduanasExpress.API.Middleware.ManejoErroresMiddleware(siguientePaso, _logger);

            await middleware.InvokeAsync(contexto);

            Assert.Equal((int)HttpStatusCode.InternalServerError, contexto.Response.StatusCode);
            contexto.Response.Body.Seek(0, SeekOrigin.Begin);
            var cuerpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
            Assert.Contains("Ha ocurrido un error inesperado en el servidor.", cuerpo);
            Assert.Contains("Error interno no controlado.", cuerpo);
            Assert.DoesNotContain(errorSecreto, cuerpo);
        }

        [Fact]
        public async Task InvokeAsync_CuandoNoHayExcepcion_DebeResponderOk()
        {
            var contexto = new DefaultHttpContext();
            contexto.Response.Body = new MemoryStream();

            RequestDelegate siguientePaso = (ctx) => Task.CompletedTask;
            var middleware = new AduanasExpress.API.Middleware.ManejoErroresMiddleware(siguientePaso, _logger);

            await middleware.InvokeAsync(contexto);

            Assert.Equal((int)HttpStatusCode.OK, contexto.Response.StatusCode);
        }
    }
}