using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Application.Mappings;

namespace AduanasExpress.Infrastructure.Services;
public class ReporteService : IReporteService
{
    private readonly IAsignacionRepository _asignacionRepository;
    private readonly ISolicitudTransporteRepositories _solicitudRepository;
    private readonly IConsumoCombustibleRepositories _consumoRepository;

    public ReporteService(
        IAsignacionRepository asignacionRepository,
        ISolicitudTransporteRepositories solicitudRepository,
        IConsumoCombustibleRepositories consumoRepository)
    {
        _asignacionRepository = asignacionRepository;
        _solicitudRepository = solicitudRepository;
        _consumoRepository = consumoRepository;
    }
    public async Task<List<ReporteViajeDTO>> GetReporteViajesAsync(int mes, int año)
    {
        var solicitudes  = await _solicitudRepository.ObtenerTodos();
        var asignaciones = await _asignacionRepository.ObtenerTodos();

        // Crear un diccionario: SolicitudId -> Asignacion (toma la más reciente si hay varias)
        var asignacionPorSolicitud = asignaciones
            .GroupBy(a => a.SolicitudId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.FechaAsignacion).First());

        return solicitudes.Select(s =>
        {
            asignacionPorSolicitud.TryGetValue(s.Id, out var asig);
            var nombreConductor = asig?.Conductor != null
                ? $"{asig.Conductor.Nombre} {asig.Conductor.Apellido}"
                : null;
            var vehiculoPlaca = asig?.Vehiculo?.Matricula;
            return s.ToReporteViajeDTO(nombreConductor, vehiculoPlaca);
        }).ToList();
    }

    public async Task<List<ReporteConsumoDTO>> GetReporteConsumoAsync(int mes, int año)
    {
        var consumos = await _consumoRepository.ObtenerTodos();

        return consumos
            .Where(c => c.Fecha.HasValue && c.Fecha.Value.Month == mes && c.Fecha.Value.Year == año)
            .GroupBy(c => c.Vehiculo)
            .Select(g => new ReporteConsumoDTO
            {
                VehiculoPlaca = g.Key.Matricula,
                VehiculoMarca = g.Key.Marca,
                TotalGalones = g.Sum(c => c.Galones),
                CostoTotal = g.Sum(c => c.CostoTotal),
                TotalViajes = g.Count()
            }).ToList();
    }

    public async Task<ReporteSolicitudDTO> GetReporteSolicitudesAsync()
    {
        var solicitudes = await _solicitudRepository.ObtenerTodos();

        return new ReporteSolicitudDTO
        {
            TotalSolicitudes = solicitudes.Count(),
            Pendientes = solicitudes.Count(s => s.Estado == EstadosSolicitudes.Pendiente),
            Aprobadas = solicitudes.Count(s => s.Estado == EstadosSolicitudes.Aprobada),
            Rechazadas = solicitudes.Count(s => s.Estado == EstadosSolicitudes.Rechazada),
            Canceladas = solicitudes.Count(s => s.Estado == EstadosSolicitudes.Cancelada),
            Finalizadas = solicitudes.Count(s => s.Estado == EstadosSolicitudes.Finalizada)
        };
    }

    public async Task<List<ReporteConductorDTO>> GetReporteConductoresAsync()
    {
        var asignaciones = await _asignacionRepository.ObtenerTodos();

        return asignaciones
            .GroupBy(a => a.Conductor)
            .Select(g => new ReporteConductorDTO
            {
                NombreConductor = $"{g.Key.Nombre} {g.Key.Apellido}",
                Licencia = g.Key.NumeroLicencia,
                TotalViajes = g.Count(),
                TotalPasajeros = g.Sum(a => a.Solicitud.CantidadColaboradores)
            }).ToList();
    }

    // Pendiente de implementar con una librería de generación de PDF
    public Task<byte[]> ExportarPdfAsync(int mes, int año)
    {
        throw new NotImplementedException();
    }

    // Pendiente de implementar con una librería de generación de Excel
    public Task<byte[]> ExportarExcelAsync(int mes, int año)
    {
        throw new NotImplementedException();
    }
}
