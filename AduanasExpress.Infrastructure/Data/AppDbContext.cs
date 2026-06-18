using AduanasExpress.Domain.Entitis;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Evitar ciclos de cascade
        modelBuilder.Entity<SolicitudTransporte>()
            .HasOne(s => s.UsuarioSolicita)
            .WithMany()
            .HasForeignKey(s => s.UsuarioSolicitaId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SolicitudTransporte>()
            .HasOne(s => s.Conductor)
            .WithMany()
            .HasForeignKey(s => s.ConductorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SolicitudTransporte>()
            .HasOne(s => s.Vehiculo)
            .WithMany()
            .HasForeignKey(s => s.VehiculoId)
            .OnDelete(DeleteBehavior.NoAction);

        // Asignacion
        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Solicitud)
            .WithMany()
            .HasForeignKey(a => a.SolicitudId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Vehiculo)
            .WithMany()
            .HasForeignKey(a => a.VehiculoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.Conductor)
            .WithMany()
            .HasForeignKey(a => a.ConductorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Asignacion>()
            .HasOne(a => a.AsignadoPor)
            .WithMany()
            .HasForeignKey(a => a.AsignadoPorId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<SolicitudTransporte>()
.HasOne(s => s.Vehiculo)
.WithMany()
.HasForeignKey(s => s.VehiculoId)
.IsRequired(false);  // ← permite null

        modelBuilder.Entity<SolicitudTransporte>()
            .HasOne(s => s.Conductor)
            .WithMany()
            .HasForeignKey(s => s.ConductorId)
            .IsRequired(false);  // ← permite null
    }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Mantenimiento> Mantenimientos { get; set; }
    public DbSet<ConsumoCombustible> ConsumoCombustibles { get; set; }
    public DbSet<Conductor> Conductores { get; set; }
    public DbSet<SolicitudTransporte> SolicitudesTransporte { get; set; }
    public DbSet<Asignacion> Asignaciones { get; set; }
}