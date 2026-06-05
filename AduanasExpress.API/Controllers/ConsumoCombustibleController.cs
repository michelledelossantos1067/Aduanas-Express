using Microsoft.AspNetCore.Mvc;

using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using AduanasExpress.Application.DTOs.ConsumoCombustible;

[ApiController]
[Route("api/[controller]")]
public class ConsumoCombustibleController : ControllerBase{
    private readonly IConsumoCombustibleService _consumoCombustibleService;

    public ConsumoCombustibleController(IConsumoCombustibleService consumoCombustibleService){
        _consumoCombustibleService = consumoCombustibleService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var consumoCombustible = await _consumoCombustibleService.ObtenerTodos();
        return Ok(consumoCombustible);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var consumoCombustible = await _consumoCombustibleService.ObtenerTodos();
        return Ok(consumoCombustible);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs){
        await _consumoCombustibleService.Crear(createConsumoCombustibleDTOs);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateConsumoCombustibleDTOs updateConsumoCombustibleDTOs){
        await _consumoCombustibleService.Actualizar(Id,updateConsumoCombustibleDTOs);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id){
        await _consumoCombustibleService.Eliminar(Id);
        return NoContent();
    }

}
