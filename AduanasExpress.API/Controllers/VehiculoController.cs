using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiculoController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;
    public VehiculoController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var vehiculo = await _vehiculoService.ObtenerTodos();
        return Ok(vehiculo);
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var vehiculo = await _vehiculoService.ObtenerPorId(Id);
        return Ok(vehiculo);
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost]
    public async Task<IActionResult> Crear(CreateVehiculoDTOs createVehiculoDTOs)
    {
        try
        {
            await _vehiculoService.Crear(createVehiculoDTOs);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id, UpdateVehiculoDTOs updateVehiculoDTOs)
    {
        try
        {
            await _vehiculoService.Actualizar(Id, updateVehiculoDTOs);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _vehiculoService.Eliminar(id);
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
        await _vehiculoService.Desactivar(id);
        return NoContent();
    }
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _vehiculoService.Activar(id);
        return NoContent();
    }
}
