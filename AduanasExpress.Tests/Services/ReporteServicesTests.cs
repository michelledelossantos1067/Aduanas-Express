using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Services;
using Moq;

namespace AduanasExpress.Tests.Services
{
    public class ReporteServiceTests
    {
        private readonly Mock<IAsignacionRepository> _asignacionMock;
        private readonly Mock<ISolicitudTransporteRepositories> _solicitudMock;
        private readonly Mock<IConsumoCombustibleRepositories> _consumoMock;
        private readonly ReporteService _service;

        public ReporteServiceTests()
        {
            _asignacionMock = new Mock<IAsignacionRepository>();
            _solicitudMock = new Mock<ISolicitudTransporteRepositories>();
            _consumoMock = new Mock<IConsumoCombustibleRepositories>();
            _service = new ReporteService(_asignacionMock.Object, _solicitudMock.Object, _consumoMock.Object);
        }

        [Fact]
        public async Task GetReporteViajesAsync_DebeRetornarListaDeSolicitudes()
        {
            var solicitudes = new List<SolicitudTransporte>
            {
                new SolicitudTransporte { Id = 1, AreaSolicitante = "Logistica", Destino = "Norte", CantidadColaboradores = 5 },
                new SolicitudTransporte { Id = 2, AreaSolicitante = "Finanzas", Destino = "Sur", CantidadColaboradores = 3 }
            };
            _solicitudMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(solicitudes);

            var resultado = await _service.GetReporteViajesAsync(1, 2026);

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task GetReporteSolicitudesAsync_DebeContarEstadosCorrectamente()
        {
            var solicitudes = new List<SolicitudTransporte>
            {
                new SolicitudTransporte { Id = 1, Estado = EstadosSolicitudes.Pendiente },
                new SolicitudTransporte { Id = 2, Estado = EstadosSolicitudes.Aprobada },
                new SolicitudTransporte { Id = 3, Estado = EstadosSolicitudes.Rechazada },
                new SolicitudTransporte { Id = 4, Estado = EstadosSolicitudes.Pendiente }
            };
            _solicitudMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(solicitudes);

            var resultado = await _service.GetReporteSolicitudesAsync();

            Assert.NotNull(resultado);
            Assert.Equal(4, resultado.TotalSolicitudes);
            Assert.Equal(2, resultado.Pendientes);
            Assert.Equal(1, resultado.Aprobadas);
            Assert.Equal(1, resultado.Rechazadas);
        }

        [Fact]
        public async Task GetReporteConsumoAsync_DebeAgruparPorVehiculo()
        {
            var vehiculo = new Vehiculo { Id = 1, Matricula = "A001", Marca = "Toyota" };
            var consumos = new List<ConsumoCombustible>
            {
                new ConsumoCombustible { Id = 1, Galones = 10, CostoTotal = 2500, VehiculoId = 1, Vehiculo = vehiculo, Fecha = new DateTime(2026, 1, 15) },
                new ConsumoCombustible { Id = 2, Galones = 15, CostoTotal = 3750, VehiculoId = 1, Vehiculo = vehiculo, Fecha = new DateTime(2026, 1, 20) }
            };
            _consumoMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(consumos);

            var resultado = await _service.GetReporteConsumoAsync(1, 2026);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(25, resultado[0].TotalGalones);
            Assert.Equal(6250, resultado[0].CostoTotal);
        }

        [Fact]
        public async Task GetReporteConductoresAsync_DebeAgruparPorConductor()
        {
            var conductor = new Conductor { Id = 1, Nombre = "Juan", Apellido = "Perez", NumeroLicencia = "A123" };
            var solicitud = new SolicitudTransporte { Id = 1, CantidadColaboradores = 5 };
            var asignaciones = new List<Asignacion>
            {
                new Asignacion { Id = 1, ConductorId = 1, Conductor = conductor, Solicitud = solicitud },
                new Asignacion { Id = 2, ConductorId = 1, Conductor = conductor, Solicitud = solicitud }
            };
            _asignacionMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(asignaciones);

            var resultado = await _service.GetReporteConductoresAsync();

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal("Juan Perez", resultado[0].NombreConductor);
            Assert.Equal(2, resultado[0].TotalViajes);
            Assert.Equal(10, resultado[0].TotalPasajeros);
        }

        [Fact]
        public async Task ExportarPdfAsync_DebeLanzarNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => _service.ExportarPdfAsync(1, 2026));
        }

        [Fact]
        public async Task ExportarExcelAsync_DebeLanzarNotImplementedException()
        {
            await Assert.ThrowsAsync<NotImplementedException>(() => _service.ExportarExcelAsync(1, 2026));
        }
    }
}
