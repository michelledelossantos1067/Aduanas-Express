using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService){
        _reporteService = reporteService;
    }
    [HttpGet("viajes/{mes}/{año}")]
    public async Task<IActionResult> GetReporteViajesAsync(int mes, int año){
        var reporte = await _reporteService.GetReporteViajesAsync(mes,año);
        return Ok(reporte);
    }
    [HttpGet("consumo/{mes}/{año}")]
    public async Task<IActionResult> GetReporteConsumoAsync(int mes, int año){
        var reporte = await _reporteService.GetReporteConsumoAsync(mes,año);
        return Ok(reporte);
    }
    [HttpGet("solicitudes")]
    public async Task<IActionResult> GetReporteSolicitudesAsync(){
        var reporte = await _reporteService.GetReporteSolicitudesAsync();
        return Ok(reporte);
    }
    [HttpGet("conductores")]
    public async Task<IActionResult> GetReporteConductoresAsync(){
        var reporte = await _reporteService.GetReporteConductoresAsync();
        return Ok(reporte);
    }

    [HttpGet("exportar/{pdf}")]
    public async Task<IActionResult> ExportarPdfAsync(int mes, int año){
        var reporte = await _reporteService.ExportarPdfAsync(mes,año);
        return Ok(reporte);
    }
    [HttpGet("exportar/{exportar}")]
    public async Task<IActionResult> ExportarExcelAsync(int mes, int año){
        var reporte = await _reporteService.ExportarExcelAsync(mes,año);
        return Ok(reporte);
    }
}
