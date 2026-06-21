using AduanasExpress.Application.DTOs.Reporte;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IReporteService
{
    // Consultas
    Task<ReporteViajesDTO> GetReporteViajesAsync(int mes, int anio);
    Task<ReporteConsumoDTO> GetReporteConsumoAsync(int mes, int anio);
    Task<ReporteSolicitudesDTO> GetReporteSolicitudesAsync();
    Task<ReporteConductoresDTO> GetReporteConductoresAsync();

    // Exportar — Viajes
    Task<byte[]> ExportarViajesPdfAsync(int mes, int anio, ReporteConfigDTO? cfg = null);
    Task<byte[]> ExportarViajesExcelAsync(int mes, int anio, ReporteConfigDTO? cfg = null);

    // Exportar — Consumo
    Task<byte[]> ExportarConsumoPdfAsync(int mes, int anio, ReporteConfigDTO? cfg = null);
    Task<byte[]> ExportarConsumoExcelAsync(int mes, int anio, ReporteConfigDTO? cfg = null);

    // Exportar — Solicitudes
    Task<byte[]> ExportarSolicitudesPdfAsync(ReporteConfigDTO? cfg = null);
    Task<byte[]> ExportarSolicitudesExcelAsync(ReporteConfigDTO? cfg = null);

    // Exportar — Conductores
    Task<byte[]> ExportarConductoresPdfAsync(ReporteConfigDTO? cfg = null);
    Task<byte[]> ExportarConductoresExcelAsync(ReporteConfigDTO? cfg = null);
}