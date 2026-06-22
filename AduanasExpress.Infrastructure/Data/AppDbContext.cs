using AduanasExpress.Domain.Entitis;
using Microsoft.EntityFrameworkCore;

namespace AduanasExpress.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.RolId)
            .HasColumnName("Rol");

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany()
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RolPermiso>()
            .HasOne(p => p.Rol)
            .WithMany(r => r.Permisos)
            .HasForeignKey(p => p.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolPermiso>()
            .HasIndex(p => new { p.RolId, p.Modulo, p.Accion })
            .IsUnique();

        modelBuilder.Entity<Rol>().HasData(
    new Rol { Id = 1, Nombre = "Administrador", Descripcion = "Acceso total al sistema", EsSistema = true },
    new Rol { Id = 2, Nombre = "Supervisor", Descripcion = "Acceso parcial", EsSistema = true },
    new Rol { Id = 3, Nombre = "Operador", Descripcion = "Registrar solicitudes y visualizar todo", EsSistema = true }
);

        modelBuilder.Entity<RolPermiso>().HasData(SeedRolPermisos());

        modelBuilder.Entity<SolicitudTransporte>()
            .HasOne(s => s.UsuarioSolicita)
            .WithMany()
            .HasForeignKey(s => s.UsuarioSolicitaId)
            .OnDelete(DeleteBehavior.NoAction);

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
    }

    // Reproduce exactamente la matriz de permisos que ya tenías hardcodeada en RolesView.vue,
    private static List<RolPermiso> SeedRolPermisos()
    {
        var permisos = new List<RolPermiso>();
        int id = 1;

        void Agregar(int rolId, string modulo, params (string accion, bool permitido)[] acciones)
        {
            foreach (var (accion, permitido) in acciones)
                permisos.Add(new RolPermiso { Id = id++, RolId = rolId, Modulo = modulo, Accion = accion, Permitido = permitido });
        }

        // Administrador (Id = 1)
        Agregar(1, "vehiculos", ("ver", true), ("crear", true), ("editar", true), ("cancelar", true));
        Agregar(1, "conductores", ("ver", true), ("crear", true), ("editar", true), ("cancelar", true));
        Agregar(1, "solicitudes", ("ver", true), ("crear", true), ("editar", true), ("cancelar", true));
        Agregar(1, "asignaciones", ("ver", true), ("asignar", true), ("editar", true), ("cancelar", true));
        Agregar(1, "reportes", ("ver", true), ("exportar", true), ("estadisticas", true));
        Agregar(1, "usuarios", ("ver", true), ("crear", true), ("editar", true), ("cancelar", true));

        // Supervisor (Id = 2)
        Agregar(2, "vehiculos", ("ver", true), ("crear", false), ("editar", true), ("cancelar", false));
        Agregar(2, "conductores", ("ver", true), ("crear", true), ("editar", true), ("cancelar", false));
        Agregar(2, "solicitudes", ("ver", true), ("crear", true), ("editar", true), ("cancelar", false));
        Agregar(2, "asignaciones", ("ver", true), ("asignar", true), ("editar", true), ("cancelar", false));
        Agregar(2, "reportes", ("ver", true), ("exportar", true), ("estadisticas", false));
        Agregar(2, "usuarios", ("ver", true), ("crear", false), ("editar", false), ("cancelar", false));

        // Operador (Id = 3)
        Agregar(3, "vehiculos", ("ver", true), ("crear", false), ("editar", false), ("cancelar", false));
        Agregar(3, "conductores", ("ver", true), ("crear", false), ("editar", false), ("cancelar", false));
        Agregar(3, "solicitudes", ("ver", true), ("crear", true), ("editar", false), ("cancelar", false));
        Agregar(3, "asignaciones", ("ver", true), ("asignar", false), ("editar", false), ("cancelar", false));
        Agregar(3, "reportes", ("ver", true), ("exportar", false), ("estadisticas", false));
        Agregar(3, "usuarios", ("ver", true), ("crear", false), ("editar", false), ("cancelar", false));
        return permisos;
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Mantenimiento> Mantenimientos { get; set; }
    public DbSet<ConsumoCombustible> ConsumoCombustibles { get; set; }
    public DbSet<Conductor> Conductores { get; set; }
    public DbSet<SolicitudTransporte> SolicitudesTransporte { get; set; }
    public DbSet<Asignacion> Asignaciones { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
}