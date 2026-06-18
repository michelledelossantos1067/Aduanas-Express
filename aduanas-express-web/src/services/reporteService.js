import api from '../utils/axiosConfig'

export const getReporteViajes = (mes, año) => {
    return api.get(`/Reportes/viajes/${mes}/${año}`)
}

export const getReporteConsumo = (mes, año) => {
    return api.get(`/Reportes/consumo/${mes}/${año}`)
}

export const getReporteSolicitudes = () => {
    return api.get('/Reportes/solicitudes')
}

export const getReporteConductores = () => {
    return api.get('/Reportes/conductores')
}

export const exportarPdf = (mes, año) => {
    return api.get(`/Reportes/exportar/pdf`, {
        params: { mes, año },
        responseType: 'blob',
    })
}

export const exportarExcel = (mes, año) => {
    return api.get(`/Reportes/exportar/exportar`, {
        params: { mes, año },
        responseType: 'blob',
    })
}