using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.interfaces.Services;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Infrastructure.Data;
using AduanasExpress.Infrastructure.Repositories;
using AduanasExpress.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddScoped<IUsuarioRepositories, UsuarioRepositories>();
builder.Services.AddScoped<IUsuarioService, UsuarioServices>();
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IVehiculoService, VehiculoServices>();
builder.Services.AddScoped<IVehiculoRepositories, VehiculoRepositories>();
builder.Services.AddScoped<IMantenimientoRepositories, MantenimientoRepositories>();
builder.Services.AddScoped<IMantenimientoService, MantenimientoServices>();
builder.Services.AddScoped<IConsumoCombustibleRepositories, ConsumoCombustibleRepositories>();
builder.Services.AddScoped<IConsumoCombustibleService, ConsumoCombustibleServices>();
builder.Services.AddScoped<IConductorRepositories, ConductorRepositories>();
builder.Services.AddScoped<IConductorService, ConductorServices>();
builder.Services.AddScoped<ISolicitudTransporteRepositories, SolicitudTransporteRepositories>();
builder.Services.AddScoped<ISolicitudTransporteService, SolicitudTransporteServices>();
builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();
builder.Services.AddScoped<IAsignacionService, AsignacionService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseMiddleware<AduanasExpress.API.Middleware.ManejoErroresMiddleware>();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.Run();