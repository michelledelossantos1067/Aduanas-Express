using Microsoft.AspNetCore.Mvc;

using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;

[ApiController]
[Route("api/[controller]")]
public class MantenimientoController : ControllerBase{
    private readonly IMantenimientoService _mantenimientoService;

    public MantenimientoController(IMantenimientoService mantenimientoService){
        _mantenimientoService = mantenimientoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var mantenimientos = await _mantenimientoService.ObtenerTodos();
        return Ok(mantenimientos);
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        try{
            var mantenimiento = await _mantenimientoService.ObtenerPorId(Id);
            return Ok(mantenimiento);
        } catch (Exception ex){
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CreateMantenimientoDTOs createMantenimientoDTOs){
        try{
            await _mantenimientoService.Crear(createMantenimientoDTOs);
            return Created();
        } catch (Exception ex){
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs){
        try{
            await _mantenimientoService.Actualizar(Id,updateMantenimientoDTOs);
            return Ok();
        } catch (Exception ex){
            return BadRequest(new { message = ex.Message });
        }
    }

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
