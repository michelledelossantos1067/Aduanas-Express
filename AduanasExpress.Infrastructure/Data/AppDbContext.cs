using AduanasExpress.Domain.Entitis;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Data;
public class AppDbContext : DbContext{

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios {get;set;}
    public DbSet<Vehiculo> Vehiculos {get;set;}
    public DbSet<Mantenimiento> Mantenimientos { get; set; }
    public DbSet<ConsumoCombustible> ConsumoCombustibles { get; set; }

}