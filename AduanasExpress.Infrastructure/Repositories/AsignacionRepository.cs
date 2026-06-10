using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Repositories;

public class AsignacionRepository : IAsignacionRepository{
    private readonly AppDbContext _context;

    public AsignacionRepository(AppDbContext context){
        _context = context;
    }

    public async Task<List<Asignacion?>> ObtenerTodos(){
        return await _context.Asignaciones.ToListAsync();
    }
    public async Task<Asignacion?> ObtenerPorId(int Id){
        return await _context.Asignaciones.FindAsync(Id);
    }
    public async Task Crear(Asignacion asignacion){
        _context.AddAsync(asignacion);
        await _context.SaveChangesAsync();
    }
}