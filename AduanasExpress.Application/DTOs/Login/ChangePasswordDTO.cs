namespace AduanasExpress.Application.DTOs.Login;
public class ChangePasswordDTO
{
    public string Email { get; set; }
    public string PasswordActual {get;set;}
    public string PasswordNueva { get; set; }
}