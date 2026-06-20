<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { verAsignaciones, finalizarAsignacion, cancelarAsignacion } from '@/services/asignacionService.js'
import ModalFinalizarAsignacion from './ModalFinalizarAsignacion.vue'
import ModalCancelarAsignacion from './ModalCancelarAsignacion.vue'
import {
    TABS_HISTORIAL,
    formatFecha,
    formatHora,
    formatNumero,
    estadoAsignacionLabel,
    estadoAsignacionClase,
    puedeFinalizarse,
    puedeCancelarse,
} from './composables/useAsignacionHelpers'

const emit = defineEmits(['exito', 'error', 'cancelada'])

const authStore = useAuthStore()
const historial = ref([])
const loading = ref(false)
const filtroHistorial = ref('en-curso')
const finalizando = ref(null)
const cancelando = ref(null)

const modalFinalizar = ref(false)
const asignacionAFinalizar = ref(null)

const modalCancelar = ref(false)
const asignacionACancelar = ref(null)
const motivoCancelacion = ref('')

const historialFiltrado = computed(() =>
    historial.value.filter(a => {
        const label = estadoAsignacionLabel(a)
        if (filtroHistorial.value === 'pendientes') return label === 'Pendiente'
        if (filtroHistorial.value === 'en-curso') return label === 'En curso'
        if (filtroHistorial.value === 'finalizadas') return label === 'Finalizada'
        if (filtroHistorial.value === 'canceladas') return label === 'Cancelada'
        return false
    })
)

async function cargarHistorial() {
    loading.value = true
    try {
        const res = await verAsignaciones()
        historial.value = res.data
    } catch (e) {
        console.error(e)
    } finally {
        loading.value = false
    }
}

function abrirModalFinalizar(a) {
    asignacionAFinalizar.value = a
    modalFinalizar.value = true
}

function abrirModalCancelar(a) {
    asignacionACancelar.value = a
    motivoCancelacion.value = ''
    modalCancelar.value = true
}

async function confirmarFinalizar() {
    const id = asignacionAFinalizar.value?.id
    finalizando.value = id
    modalFinalizar.value = false
    try {
        await finalizarAsignacion(id)
        emit('exito', 'Viaje finalizado correctamente.')
        await cargarHistorial()
    } catch (e) {
        emit('error', e?.response?.data?.message || 'Error al finalizar el viaje.')
    } finally {
        finalizando.value = null
        asignacionAFinalizar.value = null
    }
}

async function confirmarCancelar() {
    const id = asignacionACancelar.value?.id
    cancelando.value = id
    modalCancelar.value = false
    try {
        await cancelarAsignacion(id, motivoCancelacion.value, authStore.usuario.id)
        emit('exito', 'Asignación cancelada.')
        emit('cancelada')
        await cargarHistorial()
    } catch (e) {
        emit('error', e?.response?.data?.message || 'Error al cancelar la asignación.')
    } finally {
        cancelando.value = null
        asignacionACancelar.value = null
    }
}

defineExpose({ cargarHistorial })

onMounted(cargarHistorial)
</script>

<template>
    <div class="historial-wrap">
        <div class="hist-tabs">
            <button
                v-for="t in TABS_HISTORIAL"
                :key="t.key"
                class="hist-tab"
                :class="{ 'hist-tab-activo': filtroHistorial === t.key }"
                @click="filtroHistorial = t.key"
            >
                {{ t.label }}
            </button>
        </div>

        <div v-if="loading" class="estado-carga">
            <div class="spinner"></div>
            <p>Cargando historial...</p>
        </div>

        <template v-else>
            <div v-if="historialFiltrado.length === 0" class="estado-vacio">
                <p>No hay asignaciones en esta vista.</p>
            </div>
            <table v-else class="hist-tabla">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Conductor</th>
                        <th>Vehículo</th>
                        <th>Destino</th>
                        <th>Fecha viaje</th>
                        <th>Horario</th>
                        <th>Estado</th>
                        <th>Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="a in historialFiltrado" :key="a.id">
                        <td class="td-id">{{ formatNumero(a.id) }}</td>
                        <td>{{ a.conductor ? `${a.conductor.nombre} ${a.conductor.apellido}` : '—' }}</td>
                        <td>{{ a.vehiculo?.matricula ?? '—' }}</td>
                        <td>{{ a.solicitud?.destino ?? '—' }}</td>
                        <td>{{ formatFecha(a.solicitud?.fechaViaje) }}</td>
                        <td class="td-horario">
                            {{ formatHora(a.solicitud?.horaSalida) }}
                            <template v-if="a.solicitud?.horaLlegada">
                                <span class="hora-sep">→</span>
                                {{ formatHora(a.solicitud.horaLlegada) }}
                            </template>
                        </td>
                        <td>
                            <span class="badge" :class="estadoAsignacionClase(a)">
                                {{ estadoAsignacionLabel(a) }}
                            </span>
                        </td>
                        <td>
                            <div class="td-acciones">
                                <button
                                    v-if="puedeFinalizarse(a)"
                                    class="btn-finalizar"
                                    @click="abrirModalFinalizar(a)"
                                    :disabled="finalizando === a.id"
                                >
                                    <span v-if="finalizando === a.id" class="spinner-btn"></span>
                                    {{ finalizando === a.id ? 'Finalizando…' : 'Finalizar' }}
                                </button>
                                <button
                                    v-if="puedeCancelarse(a)"
                                    class="btn-cancelar-asig"
                                    @click="abrirModalCancelar(a)"
                                    :disabled="cancelando === a.id"
                                >
                                    {{ cancelando === a.id ? 'Cancelando…' : 'Cancelar' }}
                                </button>
                                <span v-if="!puedeFinalizarse(a) && !puedeCancelarse(a)" class="td-vacio">—</span>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </template>

        <ModalFinalizarAsignacion
            :show="modalFinalizar"
            :asignacion="asignacionAFinalizar"
            :loading="!!finalizando"
            @close="modalFinalizar = false"
            @confirmar="confirmarFinalizar"
        />

        <ModalCancelarAsignacion
            v-model:motivo="motivoCancelacion"
            :show="modalCancelar"
            :asignacion="asignacionACancelar"
            :loading="!!cancelando"
            @close="modalCancelar = false"
            @confirmar="confirmarCancelar"
        />
    </div>
</template>

<style scoped>
@import './styles/asignaciones.css';

.historial-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    overflow: hidden;
}

.hist-tabs {
    display: flex;
    gap: 6px;
    padding: 12px 16px;
    border-bottom: 1.5px solid #f3f4f6;
    flex-wrap: wrap;
}

.hist-tab {
    padding: 6px 14px;
    background: #f3f4f6;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .8rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: background .15s;
    font-family: inherit;
}

.hist-tab:hover { background: #e5e7eb; }

.hist-tab-activo {
    background: #1a3a2a;
    color: #fff;
    border-color: #1a3a2a;
}

.hist-tabla {
    width: 100%;
    border-collapse: collapse;
    font-size: .875rem;
}

.hist-tabla th {
    padding: 13px 16px;
    text-align: left;
    font-size: .72rem;
    font-weight: 600;
    color: #9ca3af;
    letter-spacing: .05em;
    border-bottom: 1.5px solid #f3f4f6;
}

.hist-tabla td {
    padding: 13px 16px;
    color: #374151;
    border-bottom: 1px solid #f9fafb;
    vertical-align: middle;
}

.hist-tabla tbody tr:last-child td { border-bottom: none; }
.hist-tabla tbody tr:hover { background: #fafafa; }

.td-id { font-weight: 700; color: #111827; }

.td-horario {
    font-size: .82rem;
    white-space: nowrap;
}

.hora-sep {
    color: #9ca3af;
    margin: 0 4px;
    font-size: .75rem;
}

.td-acciones {
    display: flex;
    gap: 6px;
    align-items: center;
    flex-wrap: wrap;
}

.td-vacio { color: #d1d5db; font-size: .85rem; }

.btn-finalizar {
    padding: 5px 12px;
    background: #1a3a2a;
    border: none;
    border-radius: 6px;
    font-size: .78rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
    white-space: nowrap;
}

.btn-finalizar:hover:not(:disabled) { background: #14532d; }
.btn-finalizar:disabled { opacity: .5; cursor: default; }

.btn-cancelar-asig {
    display: inline-flex;
    align-items: center;
    gap: 5px;
    padding: 5px 10px;
    background: #fff;
    border: 1.5px solid #fecaca;
    border-radius: 6px;
    font-size: .78rem;
    font-weight: 600;
    color: #991b1b;
    cursor: pointer;
    transition: background .15s;
    font-family: inherit;
}

.btn-cancelar-asig:hover:not(:disabled) { background: #fef2f2; }
.btn-cancelar-asig:disabled { opacity: .5; cursor: default; }
</style>
