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
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id){
        var usuario = await _usuarioService.ObtenerPorId(id);
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
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id,UpdateUsuario updateUsuario){
        await _usuarioService.Actualizar(id,updateUsuario);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id){
        await _usuarioService.Eliminar(id);
        return NoContent();
    }

}
