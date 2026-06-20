using AduanasExpress.Application.DTOs.Asignacion;

namespace AduanasExpress.Application.Interfaces.Services;
public interface IAsignacionService{
    public Task<List<AsignacionResponseDTO?>> ObtenerTodos();
    public Task<AsignacionResponseDTO?> ObtenerPorId(int Id);
    public Task Crear(CreateAsignacionDTO createAsignacionDTO);
    Task<DisponiblesResponseDTO> ObtenerDisponibles(int solicitudId);
    Task Finalizar(int id);
    Task Cancelar(int id, string motivo, int usuarioId);
}
