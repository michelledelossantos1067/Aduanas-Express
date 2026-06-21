using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;

namespace AduanasExpress.Infrastructure.Services;
public class MantenimientoServices : IMantenimientoService{
    private readonly IMantenimientoRepositories _mantenimientoRepositories;
    private readonly IVehiculoRepositories _vehiculoRepositories;

    public MantenimientoServices(
        IMantenimientoRepositories mantenimientoRepositories,
        IVehiculoRepositories vehiculoRepositories){
        _mantenimientoRepositories = mantenimientoRepositories;
        _vehiculoRepositories = vehiculoRepositories;
    }

    public async Task<List<MantenimientoResponseDTOs?>> ObtenerTodos(){
        var mantenimientos = await _mantenimientoRepositories.ObtenerTodos();
        if(mantenimientos == null){
            throw new Exception("Error al obtener los mantenimientos.");
        };
        return mantenimientos.Select(m => m.ToResponse()).ToList();
    }

    public async Task<MantenimientoResponseDTOs?> ObtenerPorId(int Id){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("No se encontró el mantenimiento solicitado.");
        };
        return mantenimiento.ToResponse();
    }

    public async Task Crear(CreateMantenimientoDTOs createMantenimientoDTOs){
        var vehiculo = await _vehiculoRepositories.ObtenerPorId(createMantenimientoDTOs.VehiculoId);
        if(vehiculo == null){
            throw new Exception("El vehículo seleccionado no existe.");
        };

        var mantenimiento = new Mantenimiento{
            VehiculoId      = createMantenimientoDTOs.VehiculoId,
            Tipo            = createMantenimientoDTOs.Tipo,
            Descripcion     = createMantenimientoDTOs.Descripcion,
            Estado          = EstadoMantenimientoConverter.Parse(createMantenimientoDTOs.Estado),
            FechaProgramada = createMantenimientoDTOs.FechaProgramada,
            FechaRealizada  = createMantenimientoDTOs.FechaRealizada,
            Kilometraje     = createMantenimientoDTOs.Kilometraje,
            Costo           = createMantenimientoDTOs.Costo,
            Taller          = createMantenimientoDTOs.Taller,
            Responsable     = createMantenimientoDTOs.Responsable,
            Observaciones   = createMantenimientoDTOs.Observaciones,
        };

        await _mantenimientoRepositories.Crear(mantenimiento);
        await SincronizarEstadoVehiculo(vehiculo, mantenimiento);
    }

    public async Task Actualizar(int Id,UpdateMantenimientoDTOs updateMantenimientoDTOs){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("No se encontró el mantenimiento a actualizar.");
        };

        var vehiculoAnteriorId = mantenimiento.VehiculoId;

        var vehiculo = await _vehiculoRepositories.ObtenerPorId(updateMantenimientoDTOs.VehiculoId);
        if(vehiculo == null){
            throw new Exception("El vehículo seleccionado no existe.");
        };

        mantenimiento.VehiculoId      = updateMantenimientoDTOs.VehiculoId;
        mantenimiento.Tipo            = updateMantenimientoDTOs.Tipo;
        mantenimiento.Descripcion     = updateMantenimientoDTOs.Descripcion;
        mantenimiento.Estado          = EstadoMantenimientoConverter.Parse(updateMantenimientoDTOs.Estado);
        mantenimiento.FechaProgramada = updateMantenimientoDTOs.FechaProgramada;
        mantenimiento.FechaRealizada  = updateMantenimientoDTOs.FechaRealizada;
        mantenimiento.Kilometraje     = updateMantenimientoDTOs.Kilometraje;
        mantenimiento.Costo           = updateMantenimientoDTOs.Costo;
        mantenimiento.Taller          = updateMantenimientoDTOs.Taller;
        mantenimiento.Responsable     = updateMantenimientoDTOs.Responsable;
        mantenimiento.Observaciones   = updateMantenimientoDTOs.Observaciones;

        await _mantenimientoRepositories.Actualizar(Id, mantenimiento);
        await SincronizarEstadoVehiculo(vehiculo, mantenimiento);

        if(vehiculoAnteriorId != updateMantenimientoDTOs.VehiculoId){
            await LiberarVehiculoSiNoTieneMantenimientosActivos(vehiculoAnteriorId);
        }
    }

    public async Task Eliminar(int Id){
        var mantenimiento = await _mantenimientoRepositories.ObtenerPorId(Id);
        if(mantenimiento == null){
            throw new Exception("No se encontró el mantenimiento a eliminar.");
        };
        var vehiculoId = mantenimiento.VehiculoId;
        await _mantenimientoRepositories.Eliminar(Id);
        await LiberarVehiculoSiNoTieneMantenimientosActivos(vehiculoId);
    }
    private async Task SincronizarEstadoVehiculo(Vehiculo vehiculo, Mantenimiento mantenimiento){
        var estaActivo = mantenimiento.Estado == EstadosMantenimiento.Programado
                       || mantenimiento.Estado == EstadosMantenimiento.EnProceso;

        if(estaActivo){
            if(vehiculo.Estado != EstadosVehiculo.EnMantenimiento){
                vehiculo.Estado = EstadosVehiculo.EnMantenimiento;
                await _vehiculoRepositories.Actualizar(vehiculo.Id, vehiculo);
            }
            return;
        }

        if(mantenimiento.Estado == EstadosMantenimiento.Completado){
            if(mantenimiento.FechaRealizada.HasValue){
                vehiculo.FechaUltimoMant = mantenimiento.FechaRealizada;
            }
            if(mantenimiento.Kilometraje.HasValue){
                vehiculo.Kilometraje = mantenimiento.Kilometraje.Value;
            }
        }

        await LiberarVehiculoSiNoTieneMantenimientosActivos(vehiculo.Id, vehiculo);
    }

    private async Task LiberarVehiculoSiNoTieneMantenimientosActivos(int vehiculoId, Vehiculo? vehiculoCargado = null){
        var todos = await _mantenimientoRepositories.ObtenerTodos();
        var tieneActivos = todos.Any(m =>
            m != null &&
            m.VehiculoId == vehiculoId &&
            (m.Estado == EstadosMantenimiento.Programado || m.Estado == EstadosMantenimiento.EnProceso));

        if(tieneActivos) return;

        var vehiculo = vehiculoCargado ?? await _vehiculoRepositories.ObtenerPorId(vehiculoId);
        if(vehiculo == null || vehiculo.Estado != EstadosVehiculo.EnMantenimiento) return;

        vehiculo.Estado = EstadosVehiculo.Disponible;
        await _vehiculoRepositories.Actualizar(vehiculo.Id, vehiculo);
    }
}
