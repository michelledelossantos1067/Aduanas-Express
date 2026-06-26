namespace AduanasExpress.Application.DTOs.Login;
public class ResetPasswordWithOtpDTO
{
    public string Email { get; set; }
    public string Otp { get; set; }
    public string NuevaPassword { get; set; }
}