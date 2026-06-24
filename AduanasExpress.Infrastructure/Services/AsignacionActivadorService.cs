using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Domain.Entitis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AduanasExpress.Infrastructure.Services;

public class AsignacionActivadorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AsignacionActivadorService> _logger;

    public AsignacionActivadorService(
        IServiceScopeFactory scopeFactory,
        ILogger<AsignacionActivadorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ActivarAsignacionesPendientes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en AsignacionActivadorService");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ActivarAsignacionesPendientes()
    {
        using var scope = _scopeFactory.CreateScope();

        var asignacionRepo = scope.ServiceProvider.GetRequiredService<IAsignacionRepository>();
        var solicitudRepo = scope.ServiceProvider.GetRequiredService<ISolicitudTransporteRepositories>();
        var vehiculoRepo = scope.ServiceProvider.GetRequiredService<IVehiculoRepositories>();
        var conductorRepo = scope.ServiceProvider.GetRequiredService<IConductorRepositories>();

        var pendientes = await asignacionRepo.ObtenerPorEstado(EstadoAsignacion.Pendiente);
        var ahora = DateTime.UtcNow;

        foreach (var asignacion in pendientes)
        {
            var solicitud = await solicitudRepo.ObtenerPorId(asignacion.SolicitudId);
            if (solicitud?.FechaViaje == null) continue;
            var zonaRD = TimeZoneInfo.FindSystemTimeZoneById("America/Santo_Domingo");
            var fechaHoraLocal = solicitud.FechaViaje.Value.Date.Add(solicitud.HoraSalida);
            var fechaHoraUtc = TimeZoneInfo.ConvertTimeToUtc(fechaHoraLocal, zonaRD);

            if (ahora < fechaHoraUtc) continue;
            asignacion.Estado = EstadoAsignacion.EnCurso;
            await asignacionRepo.Actualizar(asignacion.Id, asignacion);

            var vehiculo = await vehiculoRepo.ObtenerPorId(asignacion.VehiculoId);
            if (vehiculo != null)
            {
                vehiculo.Estado = EstadosVehiculo.EnViaje;
                await vehiculoRepo.Actualizar(vehiculo.Id, vehiculo);
            }

            var conductor = await conductorRepo.ObtenerPorId(asignacion.ConductorId);
            if (conductor != null)
            {
                conductor.Estado = EstadosConductor.EnViaje;
                await conductorRepo.Actualizar(conductor.Id, conductor);
            }
            
            _logger.LogInformation(
                "Asignación {Id} activada — FechaViaje: {Fecha}",
                asignacion.Id, solicitud.FechaViaje);
        }
    }
}