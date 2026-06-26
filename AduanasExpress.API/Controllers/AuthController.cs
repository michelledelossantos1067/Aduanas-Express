using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Application.interfaces.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthDTOs authDTOs)
    {
        var login = await _authService.Login(authDTOs);
        if (login == null)
        {
            return Unauthorized();
        }
        ;
        return Ok(login);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO registerDTO)
    {
        await _authService.Register(registerDTO);
        return Ok();
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.Logout();
        return Ok();
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
    {
        var passwordTemporal = await _authService.ResetPassword(resetPasswordDTO);
        return Ok(new { passwordTemporal });
    }
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
    {
        await _authService.ChangePassword(changePasswordDTO);
        return Ok();
    }
    [HttpPost("generate-otp")]
    public async Task<IActionResult> GenerateOtp(GenerateOtpDTO request)
    {
        try
        {
            await _authService.GenerateOtp(request);
            return Ok(new { message = "Código OTP enviado a tu email" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("validate-otp")]
    public async Task<IActionResult> ValidateOtp(ValidateOtpDTO request)
    {
        try
        {
            await _authService.ValidateOtp(request);
            return Ok(new { message = "OTP válido" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("reset-password-otp")]
    public async Task<IActionResult> ResetPasswordWithOtp(ResetPasswordWithOtpDTO request)
    {
        try
        {
            await _authService.ResetPasswordWithOtp(request);
            return Ok(new { message = "Contraseña actualizada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
