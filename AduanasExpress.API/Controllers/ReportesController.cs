using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
 
[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;
 
    public ReportesController(IReporteService reporteService)
        => _reporteService = reporteService;
 
    // ── Helper: lee config de estilo de los query params ─────
    private ReporteConfigDTO Cfg() => new()
    {
        Estilo       = Request.Query["estilo"].FirstOrDefault()       ?? "light",
        ColorPrimary = Request.Query["colorPrimary"].FirstOrDefault() ?? "#1C3829",
        ColorAccent  = Request.Query["colorAccent"].FirstOrDefault()  ?? "#8A6A2E",
    };
 
    // ── Consultas (sin cambios) ───────────────────────────────
 
    [HttpGet("viajes")]
    public async Task<IActionResult> GetViajes([FromQuery] int mes, [FromQuery] int anio)
        => Ok(await _reporteService.GetReporteViajesAsync(mes, anio));
 
    [HttpGet("consumo")]
    public async Task<IActionResult> GetConsumo([FromQuery] int mes, [FromQuery] int anio)
        => Ok(await _reporteService.GetReporteConsumoAsync(mes, anio));
 
    [HttpGet("solicitudes")]
    public async Task<IActionResult> GetSolicitudes()
        => Ok(await _reporteService.GetReporteSolicitudesAsync());
 
    [HttpGet("conductores")]
    public async Task<IActionResult> GetConductores()
        => Ok(await _reporteService.GetReporteConductoresAsync());
 
    // ── Exportar PDF ─────────────────────────────────────────
 
    [HttpGet("viajes/pdf")]
    public async Task<IActionResult> ExportarViajesPdf([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarViajesPdfAsync(mes, anio, Cfg());
        return File(bytes, "application/pdf", $"viajes_{mes}_{anio}.pdf");
    }
 
    [HttpGet("consumo/pdf")]
    public async Task<IActionResult> ExportarConsumoPdf([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarConsumoPdfAsync(mes, anio, Cfg());
        return File(bytes, "application/pdf", $"consumo_{mes}_{anio}.pdf");
    }
 
    [HttpGet("solicitudes/pdf")]
    public async Task<IActionResult> ExportarSolicitudesPdf()
    {
        var bytes = await _reporteService.ExportarSolicitudesPdfAsync(Cfg());
        return File(bytes, "application/pdf", "solicitudes.pdf");
    }
 
    [HttpGet("conductores/pdf")]
    public async Task<IActionResult> ExportarConductoresPdf()
    {
        var bytes = await _reporteService.ExportarConductoresPdfAsync(Cfg());
        return File(bytes, "application/pdf", "conductores.pdf");
    }
 
    // ── Exportar Excel ────────────────────────────────────────
 
    [HttpGet("viajes/excel")]
    public async Task<IActionResult> ExportarViajesExcel([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarViajesExcelAsync(mes, anio, Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"viajes_{mes}_{anio}.xlsx");
    }
 
    [HttpGet("consumo/excel")]
    public async Task<IActionResult> ExportarConsumoExcel([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarConsumoExcelAsync(mes, anio, Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"consumo_{mes}_{anio}.xlsx");
    }
 
    [HttpGet("solicitudes/excel")]
    public async Task<IActionResult> ExportarSolicitudesExcel()
    {
        var bytes = await _reporteService.ExportarSolicitudesExcelAsync(Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "solicitudes.xlsx");
    }
 
    [HttpGet("conductores/excel")]
    public async Task<IActionResult> ExportarConductoresExcel()
    {
        var bytes = await _reporteService.ExportarConductoresExcelAsync(Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "conductores.xlsx");
    }
}