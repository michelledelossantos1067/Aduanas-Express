<script setup>
import { ref, watch } from 'vue'
import { verAsignacionesPorId } from '@/services/asignacionService'
import { formatFecha, formatNumero } from './composables/useAsignacionHelpers'

const props = defineProps({
    show:         { type: Boolean, default: false },
    asignacionId: { type: Number,  default: null  },
})

const emit = defineEmits(['close', 'editar'])

const loading    = ref(false)
const errorMsg   = ref('')
const asignacion = ref(null)

const estadosAsignacion = [
    { label: 'Pendiente',  value: 0 },
    { label: 'Activa',     value: 1 },
    { label: 'Completada', value: 2 },
    { label: 'Cancelada',  value: 3 },
]

const estadoBadgeClase = {
    0: 'badge-pendiente',
    1: 'badge-activa',
    2: 'badge-completada',
    3: 'badge-cancelada',
}

const estadoLabel = (valor) =>
    estadosAsignacion.find((e) => e.value === valor)?.label ?? valor

watch(
    () => [props.show, props.asignacionId],
    async ([show, id]) => {
        if (!show || !id) return
        asignacion.value = null
        errorMsg.value   = ''
        loading.value    = true
        try {
            const res = await verAsignacionesPorId(id)
            asignacion.value = res.data
        } catch (e) {
            console.error(e)
            errorMsg.value = 'No se pudo cargar la asignación.'
        } finally {
            loading.value = false
        }
    }
)

function cerrar() { emit('close') }
function editar() {
    emit('editar', props.asignacionId)
    emit('close')
}
</script>

<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay" @click.self="cerrar">
            <div class="modal">

                <div class="modal-head">
                    <div>
                        <p class="modal-eyebrow">Detalle</p>
                        <h2 class="modal-titulo">Asignación {{ asignacion ? formatNumero(asignacion.id) : '' }}</h2>
                    </div>
                    <button class="btn-cerrar" @click="cerrar">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <line x1="18" y1="6" x2="6" y2="18"/>
                            <line x1="6" y1="6" x2="18" y2="18"/>
                        </svg>
                    </button>
                </div>

                <div v-if="loading" class="modal-loading">
                    <div class="spinner"></div>
                </div>

                <div v-else-if="errorMsg" class="modal-error">{{ errorMsg }}</div>

                <template v-else-if="asignacion">

                    <div class="detalle-badge">
                        <span class="badge" :class="estadoBadgeClase[asignacion.estado]">
                            {{ estadoLabel(asignacion.estado) }}
                        </span>
                    </div>

                    <div class="seccion">
                        <p class="seccion-titulo">
                            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                                <circle cx="12" cy="7" r="4"/>
                            </svg>
                            Conductor
                        </p>
                        <div class="conductor-bloque" v-if="asignacion.conductor">
                            <div class="conductor-avatar-lg">
                                {{ asignacion.conductor.nombre[0] }}{{ asignacion.conductor.apellido[0] }}
                            </div>
                            <div>
                                <p class="conductor-nombre">{{ asignacion.conductor.nombre }} {{ asignacion.conductor.apellido }}</p>
                                <p class="conductor-meta">Licencia: {{ asignacion.conductor.licencia ?? '—' }}</p>
                                <p class="conductor-meta">Tel: {{ asignacion.conductor.telefono ?? '—' }}</p>
                            </div>
                        </div>
                        <p v-else class="detalle-valor text-muted">Sin conductor asignado</p>
                    </div>

                    <div class="seccion">
                        <p class="seccion-titulo">
                            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <rect x="1" y="3" width="15" height="13" rx="2"/>
                                <path d="M16 8h4l3 3v5h-7V8z"/>
                                <circle cx="5.5" cy="18.5" r="2.5"/>
                                <circle cx="18.5" cy="18.5" r="2.5"/>
                            </svg>
                            Vehículo
                        </p>
                        <div class="detalle-grid" v-if="asignacion.vehiculo">
                            <div class="detalle-item">
                                <span class="detalle-label">Placa</span>
                                <span class="detalle-valor">{{ asignacion.vehiculo.placa }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Modelo</span>
                                <span class="detalle-valor">{{ asignacion.vehiculo.modelo ?? '—' }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Marca</span>
                                <span class="detalle-valor">{{ asignacion.vehiculo.marca ?? '—' }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Capacidad</span>
                                <span class="detalle-valor">{{ asignacion.vehiculo.capacidad ?? '—' }} pasajeros</span>
                            </div>
                        </div>
                        <p v-else class="detalle-valor text-muted">Sin vehículo asignado</p>
                    </div>

                    <div class="seccion" v-if="asignacion.solicitud">
                        <p class="seccion-titulo">
                            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                                <polyline points="14 2 14 8 20 8"/>
                            </svg>
                            Solicitud vinculada
                        </p>
                        <div class="detalle-grid">
                            <div class="detalle-item">
                                <span class="detalle-label">N° Solicitud</span>
                                <span class="detalle-valor">{{ formatNumero(asignacion.solicitudId) }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Destino</span>
                                <span class="detalle-valor">{{ asignacion.solicitud.destino ?? '—' }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Área solicitante</span>
                                <span class="detalle-valor">{{ asignacion.solicitud.areaSolicitante ?? '—' }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Fecha de viaje</span>
                                <span class="detalle-valor">{{ formatFecha(asignacion.solicitud.fechaViaje) }}</span>
                            </div>
                        </div>
                    </div>

                    <div class="seccion">
                        <p class="seccion-titulo">
                            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                                <line x1="16" y1="2" x2="16" y2="6"/>
                                <line x1="8" y1="2" x2="8" y2="6"/>
                                <line x1="3" y1="10" x2="21" y2="10"/>
                            </svg>
                            Datos de asignación
                        </p>
                        <div class="detalle-grid">
                            <div class="detalle-item">
                                <span class="detalle-label">Fecha asignación</span>
                                <span class="detalle-valor">{{ formatFecha(asignacion.fechaAsignacion) }}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Estado</span>
                                <span class="detalle-valor">{{ estadoLabel(asignacion.estado) }}</span>
                            </div>
                            <div class="detalle-item detalle-item-full" v-if="asignacion.observaciones">
                                <span class="detalle-label">Observaciones</span>
                                <span class="detalle-valor">{{ asignacion.observaciones }}</span>
                            </div>
                        </div>
                    </div>
                </template>

                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="cerrar">Cerrar</button>
                    <button class="btn-editar-modal" @click="editar" :disabled="loading || !!errorMsg">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
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
    background: rgba(0,0,0,.45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 100;
}

.modal {
    background: #fff;
    border-radius: 16px;
    padding: 32px;
    width: 580px;
    max-width: 92vw;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
}

.modal-head {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: 20px;
}

.modal-eyebrow {
    font-size: .72rem;
    font-weight: 600;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .08em;
    margin: 0 0 4px;
}

.modal-titulo {
    font-size: 1.15rem;
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
    flex-shrink: 0;
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

.detalle-badge { margin-bottom: 20px; }

.badge {
    display: inline-block;
    padding: 4px 12px;
    border-radius: 20px;
    font-size: .75rem;
    font-weight: 600;
}

.badge-pendiente  { background: #fef3c7; color: #92400e; }
.badge-activa     { background: #d1fae5; color: #065f46; }
.badge-completada { background: #dbeafe; color: #1e40af; }
.badge-cancelada  { background: #fee2e2; color: #991b1b; }

.seccion {
    margin-bottom: 24px;
    padding-bottom: 24px;
    border-bottom: 1px solid #f3f4f6;
}

.seccion:last-of-type { border-bottom: none; }

.seccion-titulo {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: .72rem;
    font-weight: 700;
    color: #6b7280;
    text-transform: uppercase;
    letter-spacing: .07em;
    margin: 0 0 14px;
}

.conductor-bloque {
    display: flex;
    align-items: center;
    gap: 14px;
    background: #f9fafb;
    border-radius: 10px;
    padding: 14px 16px;
}

.conductor-avatar-lg {
    width: 44px;
    height: 44px;
    border-radius: 50%;
    background: #d1fae5;
    color: #065f46;
    font-size: .85rem;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    text-transform: uppercase;
    flex-shrink: 0;
}

.conductor-nombre {
    font-size: .95rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 3px;
}

.conductor-meta {
    font-size: .78rem;
    color: #6b7280;
    margin: 0;
}

.detalle-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 14px;
}

.detalle-item {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.detalle-item-full { grid-column: 1 / -1; }

.detalle-label {
    font-size: .71rem;
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

.text-muted {
    color: #9ca3af;
    font-weight: 400;
    font-style: italic;
}

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

.btn-editar-modal:hover:not(:disabled) { background: #14532d; }

.btn-editar-modal:disabled {
    opacity: .5;
    cursor: default;
}
</style>
