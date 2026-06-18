using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class AuthServicesTests
    {
        private readonly Mock<IUsuarioRepositories> _repositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthServices _service;

        public AuthServicesTests()
        {
            _repositoryMock = new Mock<IUsuarioRepositories>();
            _configurationMock = new Mock<IConfiguration>();

            // Configurar JWT mock
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("clave-secreta-super-larga-para-testing-1234");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("AduanasExpress");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("AduanasExpressUsers");

            _service = new AuthServices(_repositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Login_CuandoCredencialesValidas_DebeRetornarToken()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                Password = passwordHash,
                Rol = Roles.Administrador
            };
            _repositoryMock.Setup(r => r.ObtenerPorEmail("juan@test.com")).ReturnsAsync(usuario);

            var authDto = new AuthDTOs { Email = "juan@test.com", Password = "password123" };

            // Act
            var resultado = await _service.Login(authDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado.Token);
            Assert.Equal("Juan", resultado.Nombre);
        }

        [Fact]
        public async Task Login_CuandoEmailNoExiste_DebeRetornarNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObtenerPorEmail("noexiste@test.com")).ReturnsAsync((Usuario?)null);
            var authDto = new AuthDTOs { Email = "noexiste@test.com", Password = "password123" };

            // Act
            var resultado = await _service.Login(authDto);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Login_CuandoPasswordIncorrecta_DebeRetornarNull()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("passwordCorrecto");
            var usuario = new Usuario { Id = 1, Nombre = "Juan", Email = "juan@test.com", Password = passwordHash, Rol = Roles.Administrador };
            _repositoryMock.Setup(r => r.ObtenerPorEmail("juan@test.com")).ReturnsAsync(usuario);

            var authDto = new AuthDTOs { Email = "juan@test.com", Password = "passwordIncorrecto" };

            // Act
            var resultado = await _service.Login(authDto);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Register_CuandoEmailYaExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var usuarioExistente = new Usuario { Id = 1, Email = "juan@test.com" };
            _repositoryMock.Setup(r => r.ObtenerPorEmail("juan@test.com")).ReturnsAsync(usuarioExistente);

            var registerDto = new RegisterDTO { Email = "juan@test.com", Password = "password123", Nombre = "Juan", Apellido = "Perez", Rol = Roles.Administrador };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.Register(registerDto));
        }

        [Fact]
        public async Task Register_CuandoDatosValidos_DebeCrearUsuario()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObtenerPorEmail("nuevo@test.com")).ReturnsAsync((Usuario?)null);
            _repositoryMock.Setup(r => r.Crear(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

            var registerDto = new RegisterDTO { Email = "nuevo@test.com", Password = "password123", Nombre = "Nuevo", Apellido = "Usuario", Rol = Roles.Administrador };

            // Act
            await _service.Register(registerDto);

            // Assert
            _repositoryMock.Verify(r => r.Crear(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_CuandoEmailNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObtenerPorEmail("noexiste@test.com")).ReturnsAsync((Usuario?)null);
            var resetDto = new ResetPasswordDTO { Email = "noexiste@test.com" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.ResetPassword(resetDto));
        }

        [Fact]
        public async Task ResetPassword_CuandoEmailExiste_DebeRetornarPasswordTemporal()
        {
            // Arrange
            var usuario = new Usuario { Id = 1, Email = "juan@test.com", Password = "hashAnterior" };
            _repositoryMock.Setup(r => r.ObtenerPorEmail("juan@test.com")).ReturnsAsync(usuario);
            _repositoryMock.Setup(r => r.Actualizar(It.IsAny<int>(), It.IsAny<Usuario>())).Returns(Task.CompletedTask);

            var resetDto = new ResetPasswordDTO { Email = "juan@test.com" };

            // Act
            var resultado = await _service.ResetPassword(resetDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(8, resultado.Length);
        }
    }
}