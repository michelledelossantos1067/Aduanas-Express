using AduanasExpress.Application.DTOs.Mantenimiento;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Application.Mappings;

public static class MantenimientoMappingExtensions
{
    public static MantenimientoResponseDTOs ToResponse(this Mantenimiento mantenimiento)
    {
        return new MantenimientoResponseDTOs
        {
            Id = mantenimiento.Id,
            VehiculoId = mantenimiento.VehiculoId,
            VehiculoPlaca = mantenimiento.Vehiculo?.Matricula,
            Tipo = mantenimiento.Tipo,
            Descripcion = mantenimiento.Descripcion,
            Estado = EstadoMantenimientoConverter.ToTexto(mantenimiento.Estado),
            FechaProgramada = mantenimiento.FechaProgramada,
            FechaRealizada = mantenimiento.FechaRealizada,
            Kilometraje = mantenimiento.Kilometraje,
            Costo = mantenimiento.Costo,
            Taller = mantenimiento.Taller,
            Responsable = mantenimiento.Responsable,
            Observaciones = mantenimiento.Observaciones,
        };
    }
}

// El frontend trabaja con el estado como texto ("Programado", "En proceso", "Completado", "Cancelado").
// Este conversor evita problemas de serialización de enums y mantiene ese contrato.
public static class EstadoMantenimientoConverter
{
    public static EstadosMantenimiento Parse(string estado) => estado?.Trim().ToLowerInvariant() switch
    {
        "programado" => EstadosMantenimiento.Programado,
        "en proceso" => EstadosMantenimiento.EnProceso,
        "completado" => EstadosMantenimiento.Completado,
        "cancelado" => EstadosMantenimiento.Cancelado,
        _ => EstadosMantenimiento.Programado
    };

    public static string ToTexto(EstadosMantenimiento estado) => estado switch
    {
        EstadosMantenimiento.Programado => "Programado",
        EstadosMantenimiento.EnProceso => "En proceso",
        EstadosMantenimiento.Completado => "Completado",
        EstadosMantenimiento.Cancelado => "Cancelado",
        _ => "Programado"
    };
}
