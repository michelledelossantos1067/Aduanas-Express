using AduanasExpress.Application.DTOs.ConsumoCombustible;
using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsumoCombustibleController : ControllerBase{
    private readonly IConsumoCombustibleService _consumoCombustibleService;

    public ConsumoCombustibleController(IConsumoCombustibleService consumoCombustibleService){
        _consumoCombustibleService = consumoCombustibleService;
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var consumoCombustible = await _consumoCombustibleService.ObtenerTodos();
        return Ok(consumoCombustible);
    }
    [Authorize(Roles = "Administrador,Supervisor,Operador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var consumoCombustible = await _consumoCombustibleService.ObtenerPorId(Id);
        return Ok(consumoCombustible);
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPost]
    public async Task<IActionResult> Crear(CreateConsumoCombustibleDTOs createConsumoCombustibleDTOs){
        await _consumoCombustibleService.Crear(createConsumoCombustibleDTOs);
        return Created();
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateConsumoCombustibleDTOs updateConsumoCombustibleDTOs){
        await _consumoCombustibleService.Actualizar(Id,updateConsumoCombustibleDTOs);
        return Ok();
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id){
        await _consumoCombustibleService.Eliminar(Id);
        return NoContent();
    }

}
