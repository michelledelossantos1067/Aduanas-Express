using Microsoft.AspNetCore.Mvc;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.DTOs.Asignacion;

[ApiController]
[Route("api/[controller]")]
public class AsignacionesController : ControllerBase{
    private readonly IAsignacionService _asignacionService;

    public AsignacionesController(IAsignacionService asignacionService){
        _asignacionService = asignacionService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var asignacion = await _asignacionService.ObtenerTodos();
        return Ok(asignacion);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var asignacion = await _asignacionService.ObtenerTodos();
        return Ok(asignacion);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateAsignacionDTO createAsignacionDTO){
        await _asignacionService.Crear(createAsignacionDTO);
        return Created();
    }

}
