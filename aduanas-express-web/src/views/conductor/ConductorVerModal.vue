<script setup>
import { ref, watch } from 'vue'
import { verConductorPorId } from '@/services/conductorService'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    conductorId: { type: [String, Number], default: null }
})

const emit = defineEmits(['update:modelValue'])

const loading = ref(false)
const error = ref('')
const conductor = ref(null)

const estadosConductor = [
    { label: 'Disponible', value: 0 },
    { label: 'En Viaje', value: 1 },
    { label: 'Suspendido', value: 2 },
    { label: 'Inactivo', value: 3 },
]

const estadoBadgeClase = {
    0: 'badge-disponible',
    1: 'badge-en-viaje',
    2: 'badge-suspendido',
    3: 'badge-inactivo',
}

const estadoLabel = (valor) =>
    estadosConductor.find((e) => e.value === valor)?.label ?? valor

function formatFecha(fecha) {
    if (!fecha || fecha.startsWith('0001')) return '—'

    const parte = fecha.split('T')[0]
    const [anio, mes, dia] = parte.split('-')
    return `${dia}/${mes}/${anio}`
}

async function cargarConductor(id) {
    if (!id) return
    loading.value = true
    error.value = ''
    conductor.value = null
    try {
        const res = await verConductorPorId(id)
        const data = Array.isArray(res.data)
            ? res.data.find(c => c.id == id)
            : res.data
        if (!data) throw new Error('Conductor no encontrado.')
        conductor.value = data
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'No se pudo cargar el conductor.'
    } finally {
        loading.value = false
    }
}

watch(() => props.modelValue, (abierto) => {
    if (abierto) cargarConductor(props.conductorId)
})

function cerrar() {
    emit('update:modelValue', false)
}
</script>

<template>
    <Teleport to="body">
        <div v-if="modelValue" class="modal-overlay" @click.self="cerrar">
            <div class="modal">

                <div class="modal-header">
                    <div class="modal-header-icon">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                            <circle cx="12" cy="7" r="4" />
                        </svg>
                    </div>
                    <div class="modal-header-text">
                        <h2>Detalle del Conductor</h2>
                        <p>Información completa del registro</p>
                    </div>
                    <button class="btn-cerrar" @click="cerrar">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                            <line x1="18" y1="6" x2="6" y2="18" /><line x1="6" y1="6" x2="18" y2="18" />
                        </svg>
                    </button>
                </div>

                <div v-if="loading" class="modal-estado">
                    <div class="spinner"></div>
                    <p>Cargando información…</p>
                </div>

                <div v-else-if="error" class="modal-alert">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
                    </svg>
                    {{ error }}
                </div>

                <div v-else-if="conductor" class="modal-body">
                    <div class="ver-top">
                        <div class="ver-nombre">{{ conductor.nombre }} {{ conductor.apellido }}</div>
                        <span class="badge" :class="estadoBadgeClase[conductor.estado]">
                            {{ estadoLabel(conductor.estado) }}
                        </span>
                    </div>

                    <p class="ver-subtitulo">{{ conductor.cedula }} · Lic. {{ conductor.numeroLicencia }}</p>

                    <div class="ver-divider"></div>

                    <div class="ver-grid">
                        <div class="ver-item">
                            <span class="ver-label">Nombre</span>
                            <span class="ver-valor">{{ conductor.nombre ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Apellido</span>
                            <span class="ver-valor">{{ conductor.apellido ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Cédula</span>
                            <span class="ver-valor">{{ conductor.cedula ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Teléfono</span>
                            <span class="ver-valor">{{ conductor.telefono ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Tipo de Licencia</span>
                            <span class="ver-valor">{{ conductor.tipoLicencia ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Núm. Licencia</span>
                            <span class="ver-valor">{{ conductor.numeroLicencia ?? '—' }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Venc. Licencia</span>
                            <span class="ver-valor">{{ formatFecha(conductor.fechaVencLicencia) }}</span>
                        </div>
                        <div class="ver-item">
                            <span class="ver-label">Dirección</span>
                            <span class="ver-valor">{{ conductor.direccion ?? '—' }}</span>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button class="btn-cerrar-modal" @click="cerrar">Cerrar</button>
                </div>
            </div>
        </div>
    </Teleport>
</template>

<style scoped>
.modal-overlay {
    position: fixed; inset: 0;
    background: rgba(0,0,0,.45);
    display: flex; align-items: center; justify-content: center;
    z-index: 100;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
}
.modal {
    background: #fff; border-radius: 14px;
    width: 480px; max-width: 90vw; max-height: 85vh;
    overflow-y: auto;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
}
.modal-header {
    display: flex; align-items: center; gap: 14px;
    padding: 24px 28px;
    border-bottom: 1px solid #f3f4f6;
}
.modal-header-icon {
    width: 40px; height: 40px;
    background: #1a3a2a; border-radius: 10px;
    display: flex; align-items: center; justify-content: center;
    color: #fff; flex-shrink: 0;
}
.modal-header-text { flex: 1; }
.modal-header-text h2 { font-size: 1.05rem; font-weight: 700; color: #111827; margin: 0; }
.modal-header-text p { font-size: 0.8rem; color: #6b7280; margin: 2px 0 0; }
.btn-cerrar {
    background: none; border: none; color: #9ca3af;
    cursor: pointer; padding: 4px; border-radius: 6px;
    display: flex; transition: background 0.15s, color 0.15s;
}
.btn-cerrar:hover { background: #f3f4f6; color: #374151; }
.modal-estado {
    display: flex; flex-direction: column; align-items: center;
    gap: 12px; padding: 50px 0; color: #6b7280;
}
.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb; border-top-color: #1a3a2a;
    border-radius: 50%; animation: spin 0.75s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.modal-alert {
    display: flex; align-items: center; gap: 10px;
    background: #fef2f2; border: 1px solid #fecaca;
    border-radius: 8px; padding: 12px 16px;
    font-size: 0.875rem; color: #991b1b; margin: 24px 28px;
}
.modal-body { padding: 24px 28px; }
.ver-top {
    display: flex; align-items: center;
    justify-content: space-between; margin-bottom: 6px;
}
.ver-nombre {
    font-size: 1.1rem; font-weight: 700; color: #111827;
}
.ver-subtitulo { font-size: 0.85rem; color: #6b7280; margin: 0; }
.ver-divider { height: 1px; background: #f3f4f6; margin: 16px 0; }
.ver-grid {
    display: grid; grid-template-columns: repeat(2, 1fr);
    gap: 16px 20px;
}
.ver-item { display: flex; flex-direction: column; gap: 4px; }
.ver-label {
    font-size: 0.7rem; font-weight: 700;
    text-transform: uppercase; letter-spacing: 0.08em; color: #9ca3af;
}
.ver-valor { font-size: 0.9rem; font-weight: 600; color: #111827; }
.badge {
    display: inline-block; padding: 3px 10px;
    border-radius: 20px; font-size: 0.73rem; font-weight: 600;
}
.badge-disponible { background: #d1fae5; color: #065f46; }
.badge-en-viaje   { background: #dbeafe; color: #1e40af; }
.badge-suspendido { background: #fef3c7; color: #92400e; }
.badge-inactivo   { background: #f3f4f6; color: #6b7280; }
.modal-footer {
    display: flex; justify-content: flex-end;
    padding: 16px 28px;
    border-top: 1px solid #f3f4f6; background: #fafafa;
}
.btn-cerrar-modal {
    padding: 9px 20px; background: #f3f4f6; border: none;
    border-radius: 8px; font-size: 0.875rem; font-weight: 500;
    color: #374151; cursor: pointer; font-family: inherit;
    transition: background 0.15s;
}
.btn-cerrar-modal:hover { background: #e5e7eb; }
@media (max-width: 640px) {
    .modal { width: 95vw; }
    .ver-grid { grid-template-columns: 1fr; }
    .modal-header, .modal-body, .modal-footer { padding-left: 16px; padding-right: 16px; }
}
</style>
