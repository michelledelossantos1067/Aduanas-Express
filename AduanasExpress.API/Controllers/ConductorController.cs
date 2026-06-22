using AduanasExpress.Application.DTOs.Conductor;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConductorController : ControllerBase
{
    private readonly IConductorService _conductorService;

    public ConductorController(IConductorService conductorService)
    {
        _conductorService = conductorService;
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var conductor = await _conductorService.ObtenerTodos();
        return Ok(conductor);
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var conductor = await _conductorService.ObtenerPorId(Id);
        return Ok(conductor);
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CreateConductorDTOs createConductorDTOs)
    {
        await _conductorService.Crear(createConductorDTOs);
        return Created();
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id, UpdateConductorDTOs updateConductorDTOs)
    {
        await _conductorService.Actualizar(Id, updateConductorDTOs);
        return Ok();
    }
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _conductorService.Eliminar(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/desactivar")]
    public async Task<IActionResult> Desactivar(int id)
    {
        await _conductorService.Desactivar(id);
        return NoContent();
    }
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _conductorService.Activar(id);
        return NoContent();
    }

}
