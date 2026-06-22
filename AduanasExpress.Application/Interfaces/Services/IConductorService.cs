using AduanasExpress.Application.DTOs.Conductor;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IConductorService{
    public Task<List<ConductorReponseDTOs?>> ObtenerTodos();
    public Task<ConductorReponseDTOs?> ObtenerPorId(int Id);
    public Task Crear(CreateConductorDTOs createConductorDTOs);
    public Task Actualizar(int Id,UpdateConductorDTOs updateConductorDTOs);
    public Task Eliminar(int Id);
    Task Desactivar(int id);
    Task Activar(int id);
    Task<List<ConductorReponseDTOs>> ObtenerDisponiblesEnFecha(DateTime fecha);
}
