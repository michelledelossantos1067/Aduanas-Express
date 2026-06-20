using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AsignacionesController : ControllerBase
{
    private readonly IAsignacionService _asignacionService;

    public AsignacionesController(IAsignacionService asignacionService)
    {
        _asignacionService = asignacionService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var asignacion = await _asignacionService.ObtenerTodos();
        return Ok(asignacion);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var asignacion = await _asignacionService.ObtenerPorId(Id);
        return Ok(asignacion);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateAsignacionDTO createAsignacionDTO)
    {
        await _asignacionService.Crear(createAsignacionDTO);
        return Created();
    }
    [HttpGet("disponibles")]
    public async Task<IActionResult> ObtenerDisponibles([FromQuery] int solicitudId)
    {
        var resultado = await _asignacionService.ObtenerDisponibles(solicitudId);
        return Ok(resultado);
    }
    [HttpPost("{id}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        await _asignacionService.Finalizar(id);
        return NoContent();
    }
    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, CancelarAsignacionDTO cancelarDTO)
    {
        await _asignacionService.Cancelar(id, cancelarDTO.Motivo, cancelarDTO.UsuarioId);
        return NoContent();
    }
}
