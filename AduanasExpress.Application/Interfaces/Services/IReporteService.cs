using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.DTOs.Reporte;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IReporteService
{
    Task<List<ReporteViajeDTO>> GetReporteViajesAsync(int mes, int año);
    Task<List<ReporteConsumoDTO>> GetReporteConsumoAsync(int mes, int año);
    Task<ReporteSolicitudDTO> GetReporteSolicitudesAsync();
    Task<List<ReporteConductorDTO>> GetReporteConductoresAsync();
    Task<byte[]> ExportarPdfAsync(int mes, int año);
    Task<byte[]> ExportarExcelAsync(int mes, int año);
}
