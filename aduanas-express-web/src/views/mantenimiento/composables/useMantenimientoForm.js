import { ref } from 'vue'

const formInicial = () => ({
    id: null,
    vehiculoId: '',
    tipo: 'Preventivo',
    descripcion: '',
    fechaProgramada: '',
    fechaRealizada: '',
    kilometraje: '',
    costo: '',
    taller: '',
    responsable: '',
    observaciones: '',
    estado: 'Programado',
})

export function useMantenimientoForm() {
    const form = ref(formInicial())
    const modoForm = ref('crear')
    const formError = ref('')

    function abrirCrear(vehiculoIdPreseleccionado = null) {
        form.value = formInicial()
        if (vehiculoIdPreseleccionado) form.value.vehiculoId = vehiculoIdPreseleccionado
        modoForm.value = 'crear'
        formError.value = ''
    }

    function abrirEditar(r) {
        form.value = {
            id: r.id,
            vehiculoId: r.vehiculoId ?? '',
            tipo: r.tipo ?? 'Preventivo',
            descripcion: r.descripcion ?? '',
            fechaProgramada: r.fechaProgramada ? r.fechaProgramada.substring(0, 10) : '',
            fechaRealizada: r.fechaRealizada ? r.fechaRealizada.substring(0, 10) : '',
            kilometraje: r.kilometraje ?? '',
            costo: r.costo ?? '',
            taller: r.taller ?? '',
            responsable: r.responsable ?? '',
            observaciones: r.observaciones ?? '',
            estado: r.estado ?? 'Programado',
        }
        modoForm.value = 'editar'
        formError.value = ''
    }

    function validar() {
        if (!form.value.vehiculoId) return 'Selecciona el vehículo.'
        if (!form.value.descripcion) return 'La descripción es obligatoria.'
        if (!form.value.fechaProgramada) return 'La fecha programada es obligatoria.'
        return ''
    }

    function payload() {
        return {
            vehiculoId: Number(form.value.vehiculoId),
            tipo: form.value.tipo,
            descripcion: form.value.descripcion,
            estado: form.value.estado,
            fechaProgramada: form.value.fechaProgramada,
            fechaRealizada: form.value.fechaRealizada || null,
            kilometraje: form.value.kilometraje !== '' ? Number(form.value.kilometraje) : null,
            costo: form.value.costo !== '' ? Number(form.value.costo) : 0,
            taller: form.value.taller,
            responsable: form.value.responsable,
            observaciones: form.value.observaciones,
        }
    }

    return { form, modoForm, formError, abrirCrear, abrirEditar, validar, payload }
}
