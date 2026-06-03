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
        var mantenimiento = await _mantenimientoService.ObtenerTodos();
        return Ok(mantenimiento);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var mantenimiento = await _mantenimientoService.ObtenerTodos();
        return Ok(mantenimiento);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateMantenimientoDTOs createMantenimientoDTOs){
        await _mantenimientoService.Crear(createMantenimientoDTOs);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs){
        await _mantenimientoService.Actualizar(Id,updateMantenimientoDTOs);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id){
        await _mantenimientoService.Eliminar(Id);
        return NoContent();
    }

}
