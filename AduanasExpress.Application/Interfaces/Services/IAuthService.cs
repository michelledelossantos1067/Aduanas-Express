
using AduanasExpress.Application.DTOs.Usuario;
using AduanasExpress.Application.DTOs.Login;

namespace AduanasExpress.Application.interfaces.Services;
public interface IAuthService{
    public Task<AuthResponseDTOs?> Login(AuthDTOs authDTOs);
}