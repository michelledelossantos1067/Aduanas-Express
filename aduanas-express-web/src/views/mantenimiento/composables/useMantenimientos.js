import { ref, computed } from 'vue'
import {
    verMantenimiento,
    crearMantenimiento,
    actualizarMantenimiento,
    eliminarMantenimiento,
} from '@/services/mantenimientoService.js'

export const TIPOS = ['Preventivo', 'Correctivo', 'Emergencia']
export const ESTADOS = ['Programado', 'En proceso', 'Completado', 'Cancelado']

export function useMantenimientos() {
    const registros = ref([])
    const loading = ref(false)
    const eliminando = ref(null)
    const errorMsg = ref('')
    const exitoMsg = ref('')

    const busqueda = ref('')
    const filtroEstado = ref('')
    const filtroTipo = ref('')

    const registrosFiltrados = computed(() => {
        const q = busqueda.value.toLowerCase()
        return registros.value.filter(r => {
            const matchQ = !q || [r.vehiculoPlaca, r.tipo, r.taller, r.descripcion, r.responsable]
                .some(v => v?.toLowerCase().includes(q))
            const matchE = !filtroEstado.value || r.estado === filtroEstado.value
            const matchT = !filtroTipo.value || r.tipo === filtroTipo.value
            return matchQ && matchE && matchT
        })
    })

    const resumen = computed(() => ({
        total: registros.value.length,
        programados: registros.value.filter(r => r.estado === 'Programado').length,
        enProceso: registros.value.filter(r => r.estado === 'En proceso').length,
        completados: registros.value.filter(r => r.estado === 'Completado').length,
        costoTotal: registros.value.reduce((s, r) => s + (parseFloat(r.costo) || 0), 0),
    }))

    function avisar(msg, tipo = 'exito') {
        if (tipo === 'exito') exitoMsg.value = msg
        else errorMsg.value = msg
        setTimeout(() => { exitoMsg.value = ''; errorMsg.value = '' }, 3500)
    }

    async function cargar() {
        loading.value = true
        try {
            const res = await verMantenimiento()
            registros.value = res.data
        } catch (e) {
            console.error(e)
            avisar('Error al cargar los registros.', 'error')
        } finally {
            loading.value = false
        }
    }

    async function guardar(modo, payload, id = null) {
        if (modo === 'crear') {
            await crearMantenimiento(payload)
        } else {
            await actualizarMantenimiento(id, payload)
        }
        await cargar()
    }

    async function eliminar(id) {
        eliminando.value = id
        try {
            await eliminarMantenimiento(id)
            await cargar()
            avisar('Registro eliminado.')
        } catch (e) {
            console.error(e)
            avisar('Error al eliminar el registro.', 'error')
            throw e
        } finally {
            eliminando.value = null
        }
    }

    return {
        registros, loading, eliminando, errorMsg, exitoMsg,
        busqueda, filtroEstado, filtroTipo,
        registrosFiltrados, resumen,
        cargar, guardar, eliminar, avisar,
    }
}
