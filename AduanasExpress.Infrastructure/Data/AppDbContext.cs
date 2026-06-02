using Microsoft.EntityFrameworkCore;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Data;
public class AppDbContext : DbContext{

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios {get;set;}
}