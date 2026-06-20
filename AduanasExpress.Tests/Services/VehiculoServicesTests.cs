using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class VehiculoServicesTests
    {
        private readonly Mock<IVehiculoRepositories> _repositoryMock;
        private readonly VehiculoServices _service;

        public VehiculoServicesTests()
        {
            _repositoryMock = new Mock<IVehiculoRepositories>();
            _service = new VehiculoServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenVehiculos_DebeRetornarLista()
        {
            var vehiculos = new List<Vehiculo>
            {
                new Vehiculo { Id = 1, Marca = "Toyota", Modelo = "Hilux", Matricula = "A001", Color = "Blanco", Capacidad = 5 },
                new Vehiculo { Id = 2, Marca = "Nissan", Modelo = "Frontier", Matricula = "B002", Color = "Negro", Capacidad = 5 }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(vehiculos);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteVehiculo_DebeRetornarVehiculo()
        {
            var vehiculo = new Vehiculo { Id = 1, Marca = "Toyota", Modelo = "Hilux", Matricula = "A001", Color = "Blanco", Capacidad = 5 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(vehiculo);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteVehiculo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Vehiculo?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearVehiculo()
        {
            var createDto = new CreateVehiculoDTOs
            {
                Marca = "Toyota",
                Modelo = "Hilux",
                Matricula = "A001",
                Color = "Blanco",
                Capacidad = 5,
                Kilometraje = 0,
                FechaUltimoMant = DateTime.Now
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Vehiculo>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<Vehiculo>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteVehiculo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Vehiculo?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateVehiculoDTOs()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteVehiculo_DebeEliminar()
        {
            var vehiculo = new Vehiculo { Id = 1, Marca = "Toyota", Modelo = "Hilux", Matricula = "A001", Color = "Blanco", Capacidad = 5 };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(vehiculo);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteVehiculo_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Vehiculo?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}
