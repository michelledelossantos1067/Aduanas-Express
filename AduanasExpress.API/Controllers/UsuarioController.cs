using AduanasExpress.Infrastructure.Services;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.DTOs.Usuario;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase{
    private readonly IUsuarioService _usuarioService;
    public UsuarioController(IUsuarioService usuarioService){
        _usuarioService = usuarioService;
    }
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(){
        var usuario = await _usuarioService.ObtenerTodos();
        return Ok(usuario);
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> ObtenerPorId(int Id){
        var usuario = await _usuarioService.ObtenerPorId(Id);
        if(usuario == null){
            throw new Exception("Error no se encontro el usuario.");
        };
        return Ok(usuario);
    }
    [HttpPost]
    public async Task<IActionResult> Crear(CreateUsuario createUsuario){
        await _usuarioService.Crear(createUsuario);
        return Created();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> Actualizar(int Id,UpdateUsuario updateUsuario){
        await _usuarioService.Actualizar(Id,updateUsuario);
        return Ok();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> Eliminar(int Id){
        await _usuarioService.Eliminar(Id);
        return NoContent();
    }

}
