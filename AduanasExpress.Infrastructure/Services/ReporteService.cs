using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Reportes;
using QuestPDF.Fluent;

namespace AduanasExpress.Infrastructure.Services;

public class ReporteService : IReporteService
{
    private readonly IAsignacionRepository              _asignacionRepo;
    private readonly ISolicitudTransporteRepositories   _solicitudRepo;
    private readonly IConsumoCombustibleRepositories    _consumoRepo;

    public ReporteService(
        IAsignacionRepository            asignacionRepo,
        ISolicitudTransporteRepositories solicitudRepo,
        IConsumoCombustibleRepositories  consumoRepo)
    {
        _asignacionRepo = asignacionRepo;
        _solicitudRepo  = solicitudRepo;
        _consumoRepo    = consumoRepo;
    }


    public async Task<ReporteViajesDTO> GetReporteViajesAsync(int mes, int anio)
    {
        var solicitudes  = await _solicitudRepo.ObtenerTodos();
        var asignaciones = await _asignacionRepo.ObtenerTodos();

        var filtradas = solicitudes
            .Where(s => s.FechaViaje.HasValue
                     && s.FechaViaje.Value.Month == mes
                     && s.FechaViaje.Value.Year  == anio)
            .ToList();

        var mapaAsig = asignaciones
            .GroupBy(a => a.SolicitudId)
            .ToDictionary(g => g.Key, g => g.First());

        var detalles = filtradas
            .Select(s => s.ToReporteViajeDetalleDTO(
                mapaAsig.TryGetValue(s.Id, out var a) ? a : null))
            .ToList();

        return new ReporteViajesDTO
        {
            Mes            = mes,
            Anio           = anio,
            TotalViajes    = detalles.Count,
            Completados    = filtradas.Count(s => s.Estado == EstadosSolicitudes.Finalizada),
            Pendientes     = filtradas.Count(s => s.Estado == EstadosSolicitudes.Pendiente
                                               || s.Estado == EstadosSolicitudes.Aprobada),
            Cancelados     = filtradas.Count(s => s.Estado == EstadosSolicitudes.Cancelada),
            TotalPasajeros = filtradas.Sum(s => s.CantidadColaboradores),
            Detalles       = detalles,
        };
    }

    public async Task<ReporteConsumoDTO> GetReporteConsumoAsync(int mes, int anio)
    {
        var consumos = await _consumoRepo.ObtenerTodos();

        var filtrados = consumos
            .Where(c => c.Fecha.HasValue
                     && c.Fecha.Value.Month == mes
                     && c.Fecha.Value.Year  == anio)
            .ToList();

        var detalles = filtrados
            .GroupBy(c => c.Vehiculo)
            .Select(g => new ReporteConsumoDetalleDTO
            {
                VehiculoPlaca  = g.Key.Matricula,
                VehiculoMarca  = g.Key.Marca,
                TotalGalones   = g.Sum(c => c.Galones),
                CostoTotal     = g.Sum(c => c.CostoTotal),
                TotalRegistros = g.Count(),
            })
            .OrderByDescending(d => d.CostoTotal)
            .ToList();

        decimal totalGalones = detalles.Sum(d => d.TotalGalones);
        decimal costoTotal   = detalles.Sum(d => d.CostoTotal);

        return new ReporteConsumoDTO
        {
            Mes                = mes,
            Anio               = anio,
            CostoTotal         = costoTotal,
            TotalGalones       = totalGalones,
            CostoPromedioGalon = totalGalones > 0 ? costoTotal / totalGalones : 0,
            TotalVehiculos     = detalles.Count,
            Detalles           = detalles,
        };
    }

    public async Task<ReporteSolicitudesDTO> GetReporteSolicitudesAsync()
    {
        var solicitudes = await _solicitudRepo.ObtenerTodos();
        var lista       = solicitudes.ToList();

        var detalles = lista
            .Select(s => s.ToReporteSolicitudDetalleDTO())
            .OrderByDescending(d => d.FechaViaje)
            .ToList();

        return new ReporteSolicitudesDTO
        {
            Total      = lista.Count,
            Aprobadas  = lista.Count(s => s.Estado == EstadosSolicitudes.Aprobada),
            Pendientes = lista.Count(s => s.Estado == EstadosSolicitudes.Pendiente),
            Rechazadas = lista.Count(s => s.Estado == EstadosSolicitudes.Rechazada),
            Canceladas = lista.Count(s => s.Estado == EstadosSolicitudes.Cancelada),
            Finalizadas = lista.Count(s => s.Estado == EstadosSolicitudes.Finalizada),
            Detalles   = detalles,
        };
    }

    public async Task<ReporteConductoresDTO> GetReporteConductoresAsync()
    {
        var asignaciones = await _asignacionRepo.ObtenerTodos();

        var detalles = asignaciones
            .GroupBy(a => a.Conductor)
            .Select(g => new ReporteConductorDetalleDTO
            {
                NombreConductor = $"{g.Key.Nombre} {g.Key.Apellido}",
                Licencia        = g.Key.NumeroLicencia,
                TotalViajes     = g.Count(),
                TotalPasajeros  = g.Sum(a => a.Solicitud.CantidadColaboradores),
                UltimoViaje     = g.Max(a => a.Solicitud.FechaViaje),
            })
            .OrderByDescending(d => d.TotalViajes)
            .ToList();

        int totalViajes    = detalles.Sum(d => d.TotalViajes);
        int totalPasajeros = detalles.Sum(d => d.TotalPasajeros);

        return new ReporteConductoresDTO
        {
            TotalConductores          = detalles.Count,
            TotalViajes               = totalViajes,
            TotalPasajeros            = totalPasajeros,
            PromedioPasajerosPorViaje = totalViajes > 0
                ? (double)totalPasajeros / totalViajes
                : 0,
            Detalles = detalles,
        };
    }

    public async Task<byte[]> ExportarViajesPdfAsync(int mes, int anio, ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteViajesAsync(mes, anio);
        return new ViajesReporteDocumento(dto, cfg ?? new ReporteConfigDTO()).GeneratePdf();
    }

    public async Task<byte[]> ExportarConsumoPdfAsync(int mes, int anio, ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteConsumoAsync(mes, anio);
        return new ConsumoReporteDocumento(dto, cfg ?? new ReporteConfigDTO()).GeneratePdf();
    }

    public async Task<byte[]> ExportarSolicitudesPdfAsync(ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteSolicitudesAsync();
        return new SolicitudesReporteDocumento(dto, cfg ?? new ReporteConfigDTO()).GeneratePdf();
    }

    public async Task<byte[]> ExportarConductoresPdfAsync(ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteConductoresAsync();
        return new ConductoresReporteDocumento(dto, cfg ?? new ReporteConfigDTO()).GeneratePdf();
    }


    public async Task<byte[]> ExportarViajesExcelAsync(int mes, int anio, ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteViajesAsync(mes, anio);
        return ReporteExcelBuilder.GenerarViajes(dto);
    }

    public async Task<byte[]> ExportarConsumoExcelAsync(int mes, int anio, ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteConsumoAsync(mes, anio);
        return ReporteExcelBuilder.GenerarConsumo(dto);
    }

    public async Task<byte[]> ExportarSolicitudesExcelAsync(ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteSolicitudesAsync();
        return ReporteExcelBuilder.GenerarSolicitudes(dto);
    }

    public async Task<byte[]> ExportarConductoresExcelAsync(ReporteConfigDTO? cfg = null)
    {
        var dto = await GetReporteConductoresAsync();
        return ReporteExcelBuilder.GenerarConductores(dto);
    }
}