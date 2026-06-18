using AduanasExpress.Application.DTOs.Vehiculo;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VehiculoController : ControllerBase
{
    private readonly IVehiculoService _vehiculoService;
    public VehiculoController(IVehiculoService vehiculoService)
    {
        _vehiculoService = vehiculoService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var vehiculo = await _vehiculoService.ObtenerTodos();
        return Ok(vehiculo);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var vehiculo = await _vehiculoService.ObtenerPorId(Id);
        return Ok(vehiculo);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateVehiculoDTOs createVehiculoDTOs)
    {
        await _vehiculoService.Crear(createVehiculoDTOs);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id, UpdateVehiculoDTOs updateVehiculoDTOs)
    {
        await _vehiculoService.Actualizar(Id, updateVehiculoDTOs);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id)
    {
        try
        {
            await _vehiculoService.Eliminar(Id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

}
