using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuario = await _usuarioService.ObtenerTodos();
        return Ok(usuario);
    }
    [Authorize(Roles = "Administrador")]
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id)
    {
        var usuario = await _usuarioService.ObtenerPorId(Id);
        if (usuario == null)
        {
            throw new Exception("Error no se encontro el usuario.");
        }
        ;
        return Ok(usuario);
    }
    // [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Crear(CreateUsuario createUsuario)
    {
        await _usuarioService.Crear(createUsuario);
        return Created();
    }
    [Authorize(Roles = "Administrador")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id, UpdateUsuario updateUsuario)
    {
        await _usuarioService.Actualizar(Id, updateUsuario);
        return Ok();
    }
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _usuarioService.Eliminar(id);
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
        await _usuarioService.Desactivar(id);
        return NoContent();
    }
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _usuarioService.Activar(id);
        return NoContent();
    }
    [Authorize(Roles = "Administrador,Supervisor")]
    [HttpGet("por-rol/{rol}")]
    public async Task<IActionResult> ObtenerPorRol(string rol)
    {
        var usuarios = await _usuarioService.ObtenerPorRol(rol);
        return Ok(usuarios);
    }

}
