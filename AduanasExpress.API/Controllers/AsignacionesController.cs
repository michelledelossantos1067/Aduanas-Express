using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsignacionesController : ControllerBase
{
    private readonly IAsignacionService _asignacionService;

    public AsignacionesController(IAsignacionService asignacionService)
    {
        _asignacionService = asignacionService;
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var asignacion = await _asignacionService.ObtenerTodos();
        return Ok(asignacion);
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var asignacion = await _asignacionService.ObtenerPorId(Id);
        return Ok(asignacion);
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost]
    public async Task<IActionResult> Crear(CreateAsignacionDTO createAsignacionDTO)
    {
        await _asignacionService.Crear(createAsignacionDTO);
        return Created();
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("disponibles")]
    public async Task<IActionResult> ObtenerDisponibles([FromQuery] int solicitudId)
    {
        var resultado = await _asignacionService.ObtenerDisponibles(solicitudId);
        return Ok(resultado);
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost("{id}/finalizar")]
    public async Task<IActionResult> Finalizar(int id)
    {
        await _asignacionService.Finalizar(id);
        return NoContent();
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id, CancelarAsignacionDTO cancelarDTO)
    {
        await _asignacionService.Cancelar(id, cancelarDTO.Motivo, cancelarDTO.UsuarioId);
        return NoContent();
    }
}
