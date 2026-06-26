using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.interfaces.Repositories;

public interface IOtpRepository
{
    Task<OtpVerification> ObtenerPorEmail(string email);
    Task<OtpVerification> ObtenerPorEmailYCodigo(string email, string code);
    Task Crear(OtpVerification otp);
    Task Actualizar(OtpVerification otp);
    Task Eliminar(int id);
    Task LimpiarExpirados();
}