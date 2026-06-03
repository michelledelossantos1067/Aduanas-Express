using AduanasExpress.Infrastructure.Services;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.DTOs.Login;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService){
        _authService = authService;
    }
    [HttpPost]
    public async Task<IActionResult> Login(AuthDTOs authDTOs){
        var login = await _authService.Login(authDTOs);
        if(login == null){
            return Unauthorized();
        };
        return Ok(login);
    }
}
