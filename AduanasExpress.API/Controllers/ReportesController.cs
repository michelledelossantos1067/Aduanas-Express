using AduanasExpress.Application.DTOs.Reporte;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
[ApiController]
[Route("api/reportes")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;
 
    public ReportesController(IReporteService reporteService)
        => _reporteService = reporteService;
 
    private ReporteConfigDTO Cfg() => new()
    {
        Estilo       = Request.Query["estilo"].FirstOrDefault()       ?? "light",
        ColorPrimary = Request.Query["colorPrimary"].FirstOrDefault() ?? "#1C3829",
        ColorAccent  = Request.Query["colorAccent"].FirstOrDefault()  ?? "#8A6A2E",
    };
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("viajes")]
    public async Task<IActionResult> GetViajes([FromQuery] int mes, [FromQuery] int anio)
        => Ok(await _reporteService.GetReporteViajesAsync(mes, anio));
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("consumo")]
    public async Task<IActionResult> GetConsumo([FromQuery] int mes, [FromQuery] int anio)
        => Ok(await _reporteService.GetReporteConsumoAsync(mes, anio));
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("solicitudes")]
    public async Task<IActionResult> GetSolicitudes()
        => Ok(await _reporteService.GetReporteSolicitudesAsync());
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("conductores")]
    public async Task<IActionResult> GetConductores()
        => Ok(await _reporteService.GetReporteConductoresAsync());
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("viajes/pdf")]
    public async Task<IActionResult> ExportarViajesPdf([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarViajesPdfAsync(mes, anio, Cfg());
        return File(bytes, "application/pdf", $"viajes_{mes}_{anio}.pdf");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("consumo/pdf")]
    public async Task<IActionResult> ExportarConsumoPdf([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarConsumoPdfAsync(mes, anio, Cfg());
        return File(bytes, "application/pdf", $"consumo_{mes}_{anio}.pdf");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("solicitudes/pdf")]
    public async Task<IActionResult> ExportarSolicitudesPdf()
    {
        var bytes = await _reporteService.ExportarSolicitudesPdfAsync(Cfg());
        return File(bytes, "application/pdf", "solicitudes.pdf");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("conductores/pdf")]
    public async Task<IActionResult> ExportarConductoresPdf()
    {
        var bytes = await _reporteService.ExportarConductoresPdfAsync(Cfg());
        return File(bytes, "application/pdf", "conductores.pdf");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("viajes/excel")]
    public async Task<IActionResult> ExportarViajesExcel([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarViajesExcelAsync(mes, anio, Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"viajes_{mes}_{anio}.xlsx");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("consumo/excel")]
    public async Task<IActionResult> ExportarConsumoExcel([FromQuery] int mes, [FromQuery] int anio)
    {
        var bytes = await _reporteService.ExportarConsumoExcelAsync(mes, anio, Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"consumo_{mes}_{anio}.xlsx");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("solicitudes/excel")]
    public async Task<IActionResult> ExportarSolicitudesExcel()
    {
        var bytes = await _reporteService.ExportarSolicitudesExcelAsync(Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "solicitudes.xlsx");
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("conductores/excel")]
    public async Task<IActionResult> ExportarConductoresExcel()
    {
        var bytes = await _reporteService.ExportarConductoresExcelAsync(Cfg());
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "conductores.xlsx");
    }
}