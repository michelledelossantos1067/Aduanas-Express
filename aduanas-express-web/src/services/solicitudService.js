import api from '../utils/axiosConfig'

export const crearSolicitud = (data) => {
    return api.post('/SolicitudTransporte', data)
}
export const actualizarSolicitud = (Id, data) => {
    return api.put(`/SolicitudTransporte/${Id}`, data)
}
export const eliminarSolicitud = (Id) => {
    return api.delete(`/SolicitudTransporte/${Id}`)
}
export const verSolicitud = () => {
    return api.get('/SolicitudTransporte')
}
export const verSolicitudPorId = (Id) => {
    return api.get(`/SolicitudTransporte/${Id}`)
}