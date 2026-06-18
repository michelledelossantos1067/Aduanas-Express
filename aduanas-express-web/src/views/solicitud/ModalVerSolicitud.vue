<script setup>
import { ref, watch } from 'vue'
import { verSolicitudPorId } from '../../services/solicitudService'

const props = defineProps({
    show: { type: Boolean, default: false },
    solicitudId: { type: Number, default: null }
})

const emit = defineEmits(['close', 'editar'])

const loading = ref(false)
const errorMsg = ref('')
const solicitud = ref(null)

const estadosSolicitud = [
    { label: 'Pendiente', value: 0 },
    { label: 'Aprobada', value: 1 },
    { label: 'Rechazada', value: 2 },
    { label: 'Cancelada', value: 3 },
    { label: 'Finalizada', value: 4 },
]

const estadoBadgeClase = {
    0: 'badge-pendiente',
    1: 'badge-aprobada',
    2: 'badge-rechazada',
    3: 'badge-cancelada',
    4: 'badge-finalizada',
}

const estadoLabel = (valor) =>
    estadosSolicitud.find((e) => e.value === valor)?.label ?? valor

function formatFecha(fecha) {
    if (!fecha) return '—'
    return new Date(fecha).toLocaleDateString('es-DO', {
        day: '2-digit', month: '2-digit', year: 'numeric'
    })
}

function formatHora(hora) {
    if (!hora) return '—'
    return hora.toString().substring(0, 5)
}

watch(
    () => [props.show, props.solicitudId],
    async ([show, id]) => {
        if (!show || !id) return
        solicitud.value = null
        errorMsg.value = ''
        loading.value = true
        try {
            const res = await verSolicitudPorId(id)
            solicitud.value = res.data
        } catch (e) {
            console.error(e)
            errorMsg.value = 'No se pudo cargar la solicitud.'
        } finally {
            loading.value = false
        }
    }
)

function cerrar() { emit('close') }
function editar() {
    emit('editar', props.solicitudId)
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay" @click.self="cerrar">
            <div class="modal">
                <!-- Cabecera -->
                <div class="modal-head">
                    <h2 class="modal-titulo">Detalle de solicitud</h2>
                    <button class="btn-cerrar" @click="cerrar">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <line x1="18" y1="6" x2="6" y2="18" />
                            <line x1="6" y1="6" x2="18" y2="18" />
                        </svg>
                    </button>
                </div>

                <!-- Cargando -->
                <div v-if="loading" class="modal-loading">
                    <div class="spinner"></div>
                </div>

                <!-- Error -->
                <div v-else-if="errorMsg" class="modal-error">{{ errorMsg }}</div>

                <!-- Contenido -->
                <template v-else-if="solicitud">
                    <div class="detalle-badge">
                        <span class="badge" :class="estadoBadgeClase[solicitud.estado]">
                            {{ estadoLabel(solicitud.estado) }}
                        </span>
                    </div>

                    <div class="detalle-grid">
                        <div class="detalle-item">
                            <span class="detalle-label">Área solicitante</span>
                            <span class="detalle-valor">{{ solicitud.areaSolicitante || '—' }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Destino</span>
                            <span class="detalle-valor">{{ solicitud.destino || '—' }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Fecha de viaje</span>
                            <span class="detalle-valor">{{ formatFecha(solicitud.fechaViaje) }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Colaboradores</span>
                            <span class="detalle-valor">{{ solicitud.cantidadColaboradores }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Hora de salida</span>
                            <span class="detalle-valor">{{ formatHora(solicitud.horaSalida) }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Hora de llegada</span>
                            <span class="detalle-valor">{{ formatHora(solicitud.horaLlegada) }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Vehículo</span>
                            <span class="detalle-valor">{{ solicitud.vehiculo?.placa || 'Sin asignar' }}</span>
                        </div>
                        <div class="detalle-item">
                            <span class="detalle-label">Conductor</span>
                            <span class="detalle-valor">
                                {{
                                    solicitud.conductor
                                        ? `${solicitud.conductor.nombre} ${solicitud.conductor.apellido}`
                                        : 'Sin asignar'
                                }}
                            </span>
                        </div>
                        <div class="detalle-item detalle-item-full">
                            <span class="detalle-label">Motivo del viaje</span>
                            <span class="detalle-valor">{{ solicitud.motivoViaje || '—' }}</span>
                        </div>
                    </div>
                </template>

                <!-- Acciones -->
                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="cerrar">Cerrar</button>
                    <button class="btn-editar-modal" @click="editar">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                        </svg>
                        Editar
                    </button>
                </div>
            </div>
        </div>
    </Teleport>
</template>

<style scoped>
.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, .45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 100;
}

.modal {
    background: #fff;
    border-radius: 16px;
    padding: 32px;
    width: 560px;
    max-width: 92vw;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
}

.modal-head {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.modal-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.btn-cerrar {
    width: 32px;
    height: 32px;
    border: none;
    background: #f3f4f6;
    border-radius: 8px;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #6b7280;
}

.btn-cerrar:hover { background: #e5e7eb; }

.modal-loading {
    display: flex;
    justify-content: center;
    padding: 40px 0;
}

.spinner {
    width: 32px;
    height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.modal-error {
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 14px;
    color: #991b1b;
    font-size: .875rem;
    margin-bottom: 16px;
}

/* Badge de estado */
.detalle-badge {
    margin-bottom: 20px;
}

.badge {
    display: inline-block;
    padding: 4px 12px;
    border-radius: 20px;
    font-size: .75rem;
    font-weight: 600;
}

.badge-pendiente  { background: #fef3c7; color: #92400e; }
.badge-aprobada   { background: #d1fae5; color: #065f46; }
.badge-rechazada  { background: #fee2e2; color: #991b1b; }
.badge-cancelada  { background: #dbeafe; color: #1e40af; }
.badge-finalizada { background: #ede9fe; color: #6d28d9; }

/* Grid de detalle */
.detalle-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 16px;
    margin-bottom: 24px;
}

.detalle-item {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.detalle-item-full {
    grid-column: 1 / -1;
}

.detalle-label {
    font-size: .72rem;
    font-weight: 600;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .05em;
}

.detalle-valor {
    font-size: .9rem;
    color: #111827;
    font-weight: 500;
}

/* Acciones */
.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: flex-end;
    padding-top: 20px;
    border-top: 1px solid #f3f4f6;
}

.btn-cancelar-modal {
    padding: 9px 18px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
}

.btn-cancelar-modal:hover { background: #e5e7eb; }

.btn-editar-modal {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-editar-modal:hover { background: #14532d; }
</style>