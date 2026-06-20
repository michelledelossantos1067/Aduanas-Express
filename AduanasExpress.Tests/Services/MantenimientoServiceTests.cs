using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class MantenimientoServicesTests
    {
        private readonly Mock<IMantenimientoRepositories> _repositoryMock;
        private readonly MantenimientoServices _service;

        public MantenimientoServicesTests()
        {
            _repositoryMock = new Mock<IMantenimientoRepositories>();
            _service = new MantenimientoServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenMantenimientos_DebeRetornarLista()
        {
            var mantenimientos = new List<Mantenimiento>
            {
                new Mantenimiento { Id = 1, TipoMantenimiento = "Aceite", Descripcion = "Cambio de aceite", Costo = 1500, Taller = "Taller A", VehiculoId = 1 },
                new Mantenimiento { Id = 2, TipoMantenimiento = "Frenos", Descripcion = "Cambio de frenos", Costo = 3000, Taller = "Taller B", VehiculoId = 2 }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(mantenimientos);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteMantenimiento_DebeRetornarMantenimiento()
        {
            var mantenimiento = new Mantenimiento { Id = 1, TipoMantenimiento = "Aceite", Descripcion = "Cambio de aceite", Costo = 1500, Taller = "Taller A", VehiculoId = 1 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(mantenimiento);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteMantenimiento_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Mantenimiento?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearMantenimiento()
        {
            var createDto = new CreateMantenimientoDTOs
            {
                TipoMantenimiento = "Aceite",
                Descripcion = "Cambio de aceite",
                Costo = 1500,
                Taller = "Taller A",
                VehiculoId = 1
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Mantenimiento>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<Mantenimiento>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteMantenimiento_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Mantenimiento?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateMantenimientoDTOs()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteMantenimiento_DebeEliminar()
        {
            var mantenimiento = new Mantenimiento { Id = 1, TipoMantenimiento = "Aceite", Descripcion = "Cambio de aceite", Costo = 1500, Taller = "Taller A", VehiculoId = 1 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(mantenimiento);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteMantenimiento_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Mantenimiento?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}
