using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class SolicitudTransporteServicesTests
    {
        private readonly Mock<ISolicitudTransporteRepositories> _repositoryMock;
        private readonly SolicitudTransporteServices _service;

        public SolicitudTransporteServicesTests()
        {
            _repositoryMock = new Mock<ISolicitudTransporteRepositories>();
            _service = new SolicitudTransporteServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenSolicitudes_DebeRetornarLista()
        {
            var solicitudes = new List<SolicitudTransporte>
            {
                new SolicitudTransporte { Id = 1, AreaSolicitante = "Logistica", Destino = "Norte", CantidadColaboradores = 5 },
                new SolicitudTransporte { Id = 2, AreaSolicitante = "Finanzas", Destino = "Sur", CantidadColaboradores = 3 }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(solicitudes);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteSolicitud_DebeRetornarSolicitud()
        {
            var solicitud = new SolicitudTransporte { Id = 1, AreaSolicitante = "Logistica", Destino = "Norte", CantidadColaboradores = 5 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(solicitud);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteSolicitud_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((SolicitudTransporte?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearSolicitud()
        {
            var createDto = new CreateSolicitudTransporteDTOs
            {
                AreaSolicitante = "Logistica",
                CantidadColaboradores = 5,
                FechaViaje = DateTime.Now.AddDays(1),
                HoraSalida = TimeSpan.FromHours(8),
                Destino = "Norte",
                MotivoViaje = "Reunion",
                Estado = EstadosSolicitudes.Pendiente,
                UsuarioSolicitaId = 2005
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<SolicitudTransporte>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<SolicitudTransporte>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteSolicitud_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((SolicitudTransporte?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateSolicitudTransporteDTOs()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteSolicitud_DebeEliminar()
        {
            var solicitud = new SolicitudTransporte { Id = 1, AreaSolicitante = "Logistica", Destino = "Norte", CantidadColaboradores = 5 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(solicitud);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteSolicitud_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((SolicitudTransporte?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}
