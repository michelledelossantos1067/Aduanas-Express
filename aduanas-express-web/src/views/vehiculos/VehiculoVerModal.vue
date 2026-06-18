<script setup>
import { ref, watch } from 'vue'
import { verVehiculoPorId } from '@/services/vehiculoService'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    vehiculoId: { type: [String, Number], default: null }
})

const emit = defineEmits(['update:modelValue'])

const loading = ref(false)
const error = ref('')
const vehiculo = ref(null)

const estadosVehiculo = [
    { label: 'Disponible', value: 0 },
    { label: 'En Viaje', value: 1 },
    { label: 'En Mantenimiento', value: 2 },
    { label: 'Fuera de Servicio', value: 3 },
]

const estadoBadgeClase = {
    0: 'badge-disponible',
    1: 'badge-en-viaje',
    2: 'badge-mantenimiento',
    3: 'badge-fuera-servicio',
}

const estadoLabel = (valor) =>
    estadosVehiculo.find((e) => e.value === valor)?.label ?? valor

function formatFecha(fecha) {
    if (!fecha) return '—'
    return new Date(fecha).toLocaleDateString('es-DO', {
        day: '2-digit', month: '2-digit', year: 'numeric',
    })
}

async function cargarVehiculo(id) {
    if (!id) return
    loading.value = true
    error.value = ''
    vehiculo.value = null
    try {
        const res = await verVehiculoPorId(id)
        const data = Array.isArray(res.data)
            ? res.data.find(v => v.id == id)
            : res.data
        if (!data) throw new Error('Vehículo no encontrado.')
        vehiculo.value = data
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'No se pudo cargar el vehículo.'
    } finally {
        loading.value = false
    }
}

watch(() => props.modelValue, (abierto) => {
    if (abierto) cargarVehiculo(props.vehiculoId)
})

function cerrar() {
    emit('update:modelValue', false)
}
</script>

<template>
    <Teleport to="body">
        <div v-if="modelValue" class="modal-overlay" @click.self="cerrar">
            <div class="modal">

                <!-- Header -->
                <div class="modal-header">
                    <div class="modal-header-icon">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <rect x="1" y="3" width="15" height="13" rx="2" />
                            <path d="M16 8h4l3 3v5h-7V8z" />
                            <circle cx="5.5" cy="18.5" r="2.5" />
                            <circle cx="18.5" cy="18.5" r="2.5" />
                        </svg>
                    </div>
                    <div class="modal-header-text">
                        <h2>Detalle del Vehículo</h2>
                        <p>Información completa del registro</p>
                    </div>
                    <button class="btn-cerrar" @click="cerrar">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                        </svg>
                    </button>
                </div>

                <!-- Loading -->
                <div v-if="loading" class="modal-estado">
                    <div class="spinner"></div>
                    <p>Cargando información…</p>
                </div>

                <!-- Error -->
                <div v-else-if="error" class="modal-alert">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
                    </svg>
                    {{ error }}
                </div>

                <!-- Contenido -->
                <div v-else-if="vehiculo" class="modal-body">

                    <!-- Matrícula destacada + estado -->
                    <div class="ver-top">
                        <div class="ver-matricula">{{ vehiculo.matricula }}</div>
                        <span class="badge" :class="estadoBadgeClase[vehiculo.estado]">
                            {{ estadoLabel(vehiculo.estado) }}
                        </span>
                    </div>

                    <h3 class="ver-nombre">{{ vehiculo.marca }} {{ vehiculo.modelo }} {{ vehiculo.año }}</h3>
                    <p class="ver-subtitulo">{{ vehiculo.tipo }} · {{ vehiculo.capacidad }} pasajeros · {{ vehiculo.color }}</p>

                    <div class="ver-divider"></div>

                    <!-- Grid de detalles -->
                    <div class="ver-grid">
                        <div class="ver-item">
                            <span class="ver-label">Marca</span>
                            <span class="ver-valor">{{ vehiculo.marca ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Modelo</span>
                            <span class="ver-valor">{{ vehiculo.modelo ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Año</span>
                            <span class="ver-valor">{{ vehiculo.año ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Color</span>
                            <span class="ver-valor">{{ vehiculo.color ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Tipo</span>
                            <span class="ver-valor">{{ vehiculo.tipo ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Capacidad</span>
                            <span class="ver-valor">{{ vehiculo.capacidad ?? '—' }} pasajeros</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Kilometraje</span>
                            <span class="ver-valor">{{ vehiculo.kilometraje?.toLocaleString('es-DO') ?? '—' }} km</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Últ. Mantenimiento</span>
                            <span class="ver-valor">{{ formatFecha(vehiculo.fechaUltimoMant) }}</span>
                        </div>
                    </div>
                </div>

                <!-- Footer -->
                <div class="modal-footer">
                    <button class="btn-cerrar-modal" @click="cerrar">Cerrar</button>
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
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
}

.modal {
    background: #fff;
    border-radius: 14px;
    width: 480px;
    max-width: 90vw;
    max-height: 85vh;
    overflow-y: auto;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
}

/* Header */
.modal-header {
    display: flex;
    align-items: center;
    gap: 14px;
    padding: 24px 28px;
    border-bottom: 1px solid #f3f4f6;
}

.modal-header-icon {
    width: 40px;
    height: 40px;
    background: #1a3a2a;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #fff;
    flex-shrink: 0;
}

.modal-header-text { flex: 1; }

.modal-header-text h2 {
    font-size: 1.05rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.modal-header-text p {
    font-size: 0.8rem;
    color: #6b7280;
    margin: 2px 0 0;
}

.btn-cerrar {
    background: none;
    border: none;
    color: #9ca3af;
    cursor: pointer;
    padding: 4px;
    border-radius: 6px;
    display: flex;
    transition: background 0.15s, color 0.15s;
}
.btn-cerrar:hover {
    background: #f3f4f6;
    color: #374151;
}

/* Estados */
.modal-estado {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 50px 0;
    color: #6b7280;
}

.spinner {
    width: 32px;
    height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin 0.75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.modal-alert {
    display: flex;
    align-items: center;
    gap: 10px;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 12px 16px;
    font-size: 0.875rem;
    color: #991b1b;
    margin: 24px 28px;
}

/* Body */
.modal-body {
    padding: 24px 28px;
}

.ver-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 10px;
}

.ver-matricula {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    background: #f3f4f6;
    border-radius: 6px;
    padding: 4px 12px;
    letter-spacing: 0.05em;
}

.ver-nombre {
    font-size: 1.15rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.ver-subtitulo {
    font-size: 0.85rem;
    color: #6b7280;
    margin: 4px 0 0;
}

.ver-divider {
    height: 1px;
    background: #f3f4f6;
    margin: 18px 0;
}

.ver-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 16px 20px;
}

.ver-item {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.ver-label {
    font-size: 0.7rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.08em;
    color: #9ca3af;
}

.ver-valor {
    font-size: 0.9rem;
    font-weight: 600;
    color: #111827;
}

/* Badges */
.badge {
    display: inline-block;
    padding: 3px 10px;
    border-radius: 20px;
    font-size: 0.73rem;
    font-weight: 600;
}
.badge-disponible { background: #d1fae5; color: #065f46; }
.badge-en-viaje { background: #dbeafe; color: #1e40af; }
.badge-mantenimiento { background: #fef3c7; color: #92400e; }
.badge-fuera-servicio { background: #fee2e2; color: #991b1b; }

/* Footer */
.modal-footer {
    display: flex;
    justify-content: flex-end;
    padding: 16px 28px;
    border-top: 1px solid #f3f4f6;
    background: #fafafa;
}

.btn-cerrar-modal {
    padding: 9px 20px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    font-family: inherit;
    transition: background 0.15s;
}
.btn-cerrar-modal:hover { background: #e5e7eb; }

/* Responsive */
@media (max-width: 640px) {
    .modal { width: 95vw; }
    .ver-grid { grid-template-columns: 1fr; }
    .modal-header, .modal-body, .modal-footer { padding-left: 16px; padding-right: 16px; }
}
</style>