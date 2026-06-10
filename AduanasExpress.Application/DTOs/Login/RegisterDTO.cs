namespace AduanasExpress.Application.DTOs.Login;
public class RegisterDTO{
    public string Nombre {get;set;}
    public string Email { get; set; }
    public string Apellido { get; set; }
    public string Password {get;set;}
    public Roles Rol {get;set;}
}