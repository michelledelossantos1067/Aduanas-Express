
namespace AduanasExpress.Domain.Entitis;
public class Usuario{
    public int id {get;set;}
    public string nombre {get;set;}
    public string apellido {get;set;}
    public string email {get;set;}
    public string telefono {get;set;}
    public string direccion {get;set;}
    public Rol rol {get;set;}
}