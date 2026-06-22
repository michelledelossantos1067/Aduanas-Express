using AduanasExpress.Application.DTOs.Rol;
using AduanasExpress.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class RolController : ControllerBase
{
    private readonly IRolService _rolService;
    public RolController(IRolService rolService) => _rolService = rolService;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _rolService.ObtenerTodos());

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var rol = await _rolService.ObtenerPorId(id);
        return rol == null ? NotFound() : Ok(rol);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CreateRolDTO dto)
    {
        await _rolService.Crear(dto);
        return Created();
    }

    [HttpPut("{id}/permisos")]
    public async Task<IActionResult> ActualizarPermisos(int id, UpdatePermisosDTO dto)
    {
        await _rolService.ActualizarPermisos(id, dto);
        return Ok();
    }
}
