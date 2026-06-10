using AduanasExpress.Application.DTOs.Asignacion;
using AduanasExpress.Application.Interfaces.Repositories;
using AduanasExpress.Application.Interfaces.Services;
using AduanasExpress.Application.Mappings;
using AduanasExpress.Domain.Entitis;

namespace AduanasExpress.Infrastructure.Services;
public class AsignacionService : IAsignacionService{
    private readonly IAsignacionRepository _asignacionRepository;

    public AsignacionService(IAsignacionRepository asignacionRepository){
        _asignacionRepository = asignacionRepository;
    }

    public async Task<List<AsignacionResponseDTO?>> ObtenerTodos() {
        var asignacion = await _asignacionRepository.ObtenerTodos();
        if (asignacion == null) {
            throw new Exception("Error al obtener las asignaciones.");
        };
        return asignacion.Select(c => c.ToResponse()).ToList();
    }
    public async Task<AsignacionResponseDTO?> ObtenerPorId(int Id){
        var asignacion = await _asignacionRepository.ObtenerPorId(Id);
        if(asignacion == null){
            throw new Exception("Error al buscar la asignacion.");
        };
        return asignacion.ToResponse();
    }
    public async Task Crear(CreateAsignacionDTO createAsignacionDTO){
        var asignacion = new Asignacion
        {
            SolicitudId = createAsignacionDTO.SolicitudId,
            VehiculoId = createAsignacionDTO.VehiculoId,
            ConductorId = createAsignacionDTO.ConductorId,
            FechaAsignacion = createAsignacionDTO.FechaAsignacion,
            AsignadoPorId = createAsignacionDTO.AsignadoPorId
        };
        await _asignacionRepository.Crear(asignacion);
    }
}