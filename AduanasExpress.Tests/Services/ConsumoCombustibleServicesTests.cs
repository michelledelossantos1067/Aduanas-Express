using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class ConsumoCombustibleServicesTests
    {
        private readonly Mock<IConsumoCombustibleRepositories> _repositoryMock;
        private readonly ConsumoCombustibleServices _service;

        public ConsumoCombustibleServicesTests()
        {
            _repositoryMock = new Mock<IConsumoCombustibleRepositories>();
            _service = new ConsumoCombustibleServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenConsumos_DebeRetornarLista()
        {
            var consumos = new List<ConsumoCombustible>
            {
                new ConsumoCombustible { Id = 1, Galones = 10, CostoPorGalon = 250, CostoTotal = 2500, VehiculoId = 1 },
                new ConsumoCombustible { Id = 2, Galones = 20, CostoPorGalon = 250, CostoTotal = 5000, VehiculoId = 2 }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(consumos);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteConsumo_DebeRetornarConsumo()
        {
            var consumo = new ConsumoCombustible { Id = 1, Galones = 10, CostoPorGalon = 250, CostoTotal = 2500, VehiculoId = 1 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(consumo);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteConsumo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((ConsumoCombustible?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearConsumo()
        {
            var createDto = new CreateConsumoCombustibleDTOs
            {
                Galones = 10,
                CostoPorGalon = 250,
                CostoTotal = 2500,
                VehiculoId = 1
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<ConsumoCombustible>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<ConsumoCombustible>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteConsumo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((ConsumoCombustible?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateConsumoCombustibleDTOs()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteConsumo_DebeEliminar()
        {
            var consumo = new ConsumoCombustible { Id = 1, Galones = 10, CostoPorGalon = 250, CostoTotal = 2500, VehiculoId = 1 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(consumo);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteConsumo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((ConsumoCombustible?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}