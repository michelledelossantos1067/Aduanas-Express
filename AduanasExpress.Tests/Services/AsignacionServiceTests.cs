using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class AsignacionServiceTests
    {
        private readonly Mock<IAsignacionRepository> _repositoryMock;
        private readonly AsignacionService _service;

        public AsignacionServiceTests()
        {
            _repositoryMock = new Mock<IAsignacionRepository>();
            _service = new AsignacionService(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenAsignaciones_DebeRetornarLista()
        {
            // Arrange
            var asignaciones = new List<Asignacion>
            {
                new Asignacion { Id = 1, SolicitudId = 1, VehiculoId = 1, ConductorId = 1, AsignadoPorId = 1, FechaAsignacion = DateTime.Now },
                new Asignacion { Id = 2, SolicitudId = 2, VehiculoId = 2, ConductorId = 2, AsignadoPorId = 1, FechaAsignacion = DateTime.Now }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(asignaciones);

            // Act
            var resultado = await _service.ObtenerTodos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteAsignacion_DebeRetornarAsignacion()
        {
            // Arrange
            var asignacion = new Asignacion { Id = 1, SolicitudId = 1, VehiculoId = 1, ConductorId = 1, AsignadoPorId = 1, FechaAsignacion = DateTime.Now };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(asignacion);

            // Act
            var resultado = await _service.ObtenerPorId(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteAsignacion_DebeLanzarExcepcion()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Asignacion?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearAsignacion()
        {
            // Arrange
            var createDto = new CreateAsignacionDTO
            {
                SolicitudId = 1,
                VehiculoId = 1,
                ConductorId = 1,
                AsignadoPorId = 1,
                FechaAsignacion = DateTime.Now
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Asignacion>())).Returns(Task.CompletedTask);

            // Act
            await _service.Crear(createDto);

            // Assert
            _repositoryMock.Verify(r => r.Crear(It.IsAny<Asignacion>()), Times.Once);
        }
    }
}