import api from '../utils/axiosConfig'

export const crearUsuario = (data) => {
    return api.post('/usuario', data)
}
export const actualizarUsuario = (Id, data) => {
    return api.put(`/usuario/${Id}`, data)
}
export const eliminarUsuario = (Id) => {
    return api.delete(`/usuario/${Id}`)
}
export const obtenerUsuario = () => {
    return api.get('/usuario')
}
export const obtenerUsuarios = (Id) => {
    return api.get(`/usuario/${Id}`)
}
export const desactivarUsuario = (id) => api.patch(`/usuario/${id}/desactivar`)
export const activarUsuario = (id) => api.patch(`/usuario/${id}/activar`)
