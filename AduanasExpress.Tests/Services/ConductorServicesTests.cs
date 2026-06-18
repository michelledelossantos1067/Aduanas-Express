using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class ConductorServicesTests
    {
        private readonly Mock<IConductorRepositories> _repositoryMock;
        private readonly ConductorServices _service;

        public ConductorServicesTests()
        {
            _repositoryMock = new Mock<IConductorRepositories>();
            _service = new ConductorServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenConductores_DebeRetornarLista()
        {
            var conductores = new List<Conductor>
            {
                new Conductor { Id = 1, Nombre = "Juan", Apellido = "Perez", Cedula = "001-1234567-8", NumeroLicencia = "A123", Telefono = "809-555-0001" },
                new Conductor { Id = 2, Nombre = "Maria", Apellido = "Lopez", Cedula = "001-7654321-8", NumeroLicencia = "B456", Telefono = "809-555-0002" }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(conductores);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteConductor_DebeRetornarConductor()
        {
            var conductor = new Conductor { Id = 1, Nombre = "Juan", Apellido = "Perez", Cedula = "001-1234567-8", NumeroLicencia = "A123", Telefono = "809-555-0001" };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(conductor);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteConductor_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Conductor?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearConductor()
        {
            var createDto = new CreateConductorDTOs
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Cedula = "001-1234567-8",
                NumeroLicencia = "A123",
                Telefono = "809-555-0001"
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Conductor>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<Conductor>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteConductor_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Conductor?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateConductorDTOs()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteConductor_DebeEliminar()
        {
            var conductor = new Conductor { Id = 1, Nombre = "Juan", Apellido = "Perez", Cedula = "001-1234567-8", NumeroLicencia = "A123", Telefono = "809-555-0001" };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(conductor);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

           
            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteConductor_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Conductor?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}