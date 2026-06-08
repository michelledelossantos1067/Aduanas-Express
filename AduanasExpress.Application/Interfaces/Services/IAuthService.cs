
using AduanasExpress.Application.DTOs.Login;
using AduanasExpress.Application.DTOs.Usuario;

namespace AduanasExpress.Application.interfaces.Services;
public interface IAuthService{
    public Task<AuthResponseDTOs?> Login(AuthDTOs authDTOs);
    public Task Logout();
    public Task Register(RegisterDTO registerDTO);
    public Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO);
    public Task ChangePassword(ChangePasswordDTO changePasswordDTO);
}