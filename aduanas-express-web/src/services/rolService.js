import api from '../utils/axiosConfig'

export const obtenerRoles = () => api.get('/Rol')
export const crearRol = (dto) => api.post('/Rol', dto)

export const actualizarPermisos = (id, permisos) => {
    return api.put(`/Rol/${id}/permisos`, { permisos })
}