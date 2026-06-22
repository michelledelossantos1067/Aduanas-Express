using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MantenimientoController : ControllerBase{
    private readonly IMantenimientoService _mantenimientoService;

    public MantenimientoController(IMantenimientoService mantenimientoService){
        _mantenimientoService = mantenimientoService;
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var mantenimientos = await _mantenimientoService.ObtenerTodos();
        return Ok(mantenimientos);
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        try{
            var mantenimiento = await _mantenimientoService.ObtenerPorId(Id);
            return Ok(mantenimiento);
        } catch (Exception ex){
            return NotFound(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost]
    public async Task<IActionResult> Crear(CreateMantenimientoDTOs createMantenimientoDTOs){
        try{
            await _mantenimientoService.Crear(createMantenimientoDTOs);
            return Created();
        } catch (Exception ex){
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs){
        try{
            await _mantenimientoService.Actualizar(Id,updateMantenimientoDTOs);
            return Ok();
        } catch (Exception ex){
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id){
        try{
            await _mantenimientoService.Eliminar(Id);
            return NoContent();
        } catch (Exception ex){
            return BadRequest(new { message = ex.Message });
        }
    }

}
