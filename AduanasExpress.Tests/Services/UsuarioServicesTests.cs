using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class UsuarioServicesTests
    {
        private readonly Mock<IUsuarioRepositories> _repositoryMock;
        private readonly UsuarioServices _service;

        public UsuarioServicesTests()
        {
            _repositoryMock = new Mock<IUsuarioRepositories>();
            _service = new UsuarioServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task ObtenerTodos_CuandoExistenUsuarios_DebeRetornarLista()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com", Rol = Roles.Administrador },
                new Usuario { Id = 2, Nombre = "Maria", Apellido = "Lopez", Email = "maria@test.com", Rol = Roles.Operador }
            };
            _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(usuarios);

            var resultado = await _service.ObtenerTodos();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoExisteUsuario_DebeRetornarUsuario()
        {
            var usuario = new Usuario { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com", Rol = Roles.Administrador };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(usuario);

            var resultado = await _service.ObtenerPorId(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorId_CuandoNoExisteUsuario_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.ObtenerPorId(99));
        }

        [Fact]
        public async Task Crear_CuandoDatosValidos_DebeCrearUsuario()
        {
            var createDto = new CreateUsuario
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                Password = "password123",
                Rol = Roles.Administrador
            };
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

            await _service.Crear(createDto);

            _repositoryMock.Verify(r => r.Crear(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Actualizar_CuandoNoExisteUsuario_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Actualizar(99, new UpdateUsuario()));
        }

        [Fact]
        public async Task Eliminar_CuandoExisteUsuario_DebeEliminar()
        {
            var usuario = new Usuario { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com", Rol = Roles.Administrador };
            _repositoryMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(usuario);
            _repositoryMock.Setup(r => r.Eliminar(1)).Returns(Task.CompletedTask);

            await _service.Eliminar(1);

            _repositoryMock.Verify(r => r.Eliminar(1), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteUsuario_DebeLanzarExcepcion()
        {
            _repositoryMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<Exception>(() => _service.Eliminar(99));
        }
    }
}
