using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace AduanasExpress.Infrastructure.Repositories;

public class SolicitudTransporteRepositories : ISolicitudTransporteRepositories
{
    private readonly AppDbContext _context;

    public SolicitudTransporteRepositories(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SolicitudTransporte?>> ObtenerTodos()
    {
        return await _context.SolicitudesTransporte
            .Include(s => s.UsuarioSolicita)
            .ToListAsync();
    }
    public async Task<SolicitudTransporte?> ObtenerPorId(int Id)
    {
        return await _context.SolicitudesTransporte
            .Include(s => s.UsuarioSolicita)
            .FirstOrDefaultAsync(s => s.Id == Id);
    }
    public async Task Crear(SolicitudTransporte solicitudTransporte)
    {
        await _context.AddAsync(solicitudTransporte);
        await _context.SaveChangesAsync();
    }
    public async Task Actualizar(int Id, SolicitudTransporte solicitudTransporte)
    {
        var solicitudTrans = await _context.SolicitudesTransporte.FindAsync(Id);

        if (solicitudTrans == null)
            throw new Exception("No encontrado");

        _context.Entry(solicitudTrans).CurrentValues.SetValues(solicitudTransporte);
        Console.WriteLine(solicitudTrans.PuntoOrigen);

Console.WriteLine(solicitudTrans.Destino);
        await _context.SaveChangesAsync();
    }
    public async Task Eliminar(int Id)
    {
        var solicitudTrans = await _context.SolicitudesTransporte.FindAsync(Id);
        if (solicitudTrans == null)
        {
            throw new Exception("No se puede eliminar este solicitud de transporte.");
        }
        ;
        _context.Remove(solicitudTrans);
        await _context.SaveChangesAsync();
    }
}
