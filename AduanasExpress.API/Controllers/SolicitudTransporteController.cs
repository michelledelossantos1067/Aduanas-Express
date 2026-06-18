using System.Security.Claims;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.DTOs.SolicitudTransporte;
[ApiController]
[Route("api/[controller]")]
public class SolicitudTransporteController : ControllerBase
{
    private readonly ISolicitudTransporteService _solicitudTransporteService;

    public SolicitudTransporteController(ISolicitudTransporteService solicitudTransporteService)
    {
        _solicitudTransporteService = solicitudTransporteService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var solicitudTrans = await _solicitudTransporteService.ObtenerTodos();
        return Ok(solicitudTrans);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var solicitudTrans = await _solicitudTransporteService.ObtenerPorId(Id);
        return Ok(solicitudTrans);
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CreateSolicitudTransporteDTOs dto)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _solicitudTransporteService.Crear(dto, usuarioId);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id, UpdateSolicitudTransporteDTOs updateSolicitudTransporteDTOs)
    {
        await _solicitudTransporteService.Actualizar(Id, updateSolicitudTransporteDTOs);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id)
    {
        await _solicitudTransporteService.Eliminar(Id);
        return NoContent();
    }
}
