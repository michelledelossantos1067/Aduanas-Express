using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class OtpRepository : IOtpRepository
{
    private readonly AppDbContext _context;

    public OtpRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OtpVerification> ObtenerPorEmail(string email)
    {
        return await _context.OtpVerifications
            .Where(o => o.Email == email && !o.IsUsed && o.ExpiryTime > DateTime.Now)
            .FirstOrDefaultAsync();
    }

    public async Task<OtpVerification> ObtenerPorEmailYCodigo(string email, string code)
    {
        return await _context.OtpVerifications
            .FirstOrDefaultAsync(o => o.Email == email && o.Code == code && !o.IsUsed && o.ExpiryTime > DateTime.Now);
    }

    public async Task Crear(OtpVerification otp)
    {
        otp.CreatedAt = DateTime.Now;
        await _context.OtpVerifications.AddAsync(otp);
        await _context.SaveChangesAsync();
    }

    public async Task Actualizar(OtpVerification otp)
    {
        _context.OtpVerifications.Update(otp);
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(int id)
    {
        var otp = await _context.OtpVerifications.FindAsync(id);
        if (otp != null)
        {
            _context.OtpVerifications.Remove(otp);
            await _context.SaveChangesAsync();
        }
    }

    public async Task LimpiarExpirados()
    {
        var expirados = await _context.OtpVerifications
            .Where(o => o.ExpiryTime < DateTime.Now)
            .ToListAsync();

        if (expirados.Any())
        {
            _context.OtpVerifications.RemoveRange(expirados);
            await _context.SaveChangesAsync();
        }
    }
}