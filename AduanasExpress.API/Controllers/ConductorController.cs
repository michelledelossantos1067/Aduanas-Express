using Microsoft.AspNetCore.Mvc;

using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using AduanasExpress.Application.DTOs.Conductor;

[ApiController]
[Route("api/[controller]")]
public class ConductorController : ControllerBase{
    private readonly IConductorService _conductorService;

    public ConductorController(IConductorService conductorService){
        _conductorService = conductorService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var conductor = await _conductorService.ObtenerTodos();
        return Ok(conductor);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var conductor = await _conductorService.ObtenerPorId(Id);
        return Ok(conductor);
    }
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CreateConductorDTOs createConductorDTOs){
        await _conductorService.Crear(createConductorDTOs);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateConductorDTOs updateConductorDTOs){
        await _conductorService.Actualizar(Id,updateConductorDTOs);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id)
    {
        try
        {
            await _conductorService.Eliminar(Id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
