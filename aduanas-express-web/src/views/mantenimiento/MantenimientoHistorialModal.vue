<script setup>
import { computed } from 'vue'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    vehiculoId: { type: [Number, String], default: null },
    vehiculoPlaca: { type: String, default: '' },
    registros: { type: Array, default: () => [] },
})
const emit = defineEmits(['update:modelValue'])

const historial = computed(() =>
    props.registros
        .filter(r => r.vehiculoId === props.vehiculoId)
        .slice()
        .sort((a, b) => new Date(b.fechaProgramada) - new Date(a.fechaProgramada))
)

function cerrar() { emit('update:modelValue', false) }

function formatFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}
function formatMoney(n) {
    const v = parseFloat(n) || 0
    return v.toLocaleString('es-DO', { style: 'currency', currency: 'DOP', maximumFractionDigits: 0 })
}
function estadoClase(estado) {
    return {
        'Programado': 'badge-programado', 'En proceso': 'badge-en-proceso',
        'Completado': 'badge-completado', 'Cancelado': 'badge-cancelado',
    }[estado] ?? ''
}
</script>

<template>
    <div v-if="modelValue" class="modal-overlay" @click.self="cerrar">
        <div class="modal-box-historial">
            <div class="historial-header">
                <div>
                    <h3 class="modal-titulo">Historial de mantenimiento</h3>
                    <p class="modal-desc">
                        <span class="placa-badge">{{ vehiculoPlaca }}</span>
                        {{ historial.length }} {{ historial.length === 1 ? 'registro' : 'registros' }} en total
                    </p>
                </div>
                <button class="btn-cerrar" @click="cerrar">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.2">
                        <line x1="18" y1="6" x2="6" y2="18" />
                        <line x1="6" y1="6" x2="18" y2="18" />
                    </svg>
                </button>
            </div>

            <div v-if="historial.length === 0" class="historial-vacio">
                Este vehículo todavía no tiene mantenimientos registrados.
            </div>

            <div v-else class="historial-lista">
                <div v-for="r in historial" :key="r.id" class="historial-item">
                    <div class="historial-item-top">
                        <span class="badge" :class="estadoClase(r.estado)">{{ r.estado }}</span>
                        <span class="historial-tipo">{{ r.tipo }}</span>
                        <span class="historial-costo">{{ r.costo ? formatMoney(r.costo) : '—' }}</span>
                    </div>
                    <p class="historial-desc">{{ r.descripcion }}</p>
                    <div class="historial-fechas">
                        <span>Programado: {{ formatFecha(r.fechaProgramada) }}</span>
                        <span v-if="r.fechaRealizada">Realizado: {{ formatFecha(r.fechaRealizada) }}</span>
                    </div>
                    <div class="historial-meta">
                        <span v-if="r.taller">Taller: {{ r.taller }}</span>
                        <span v-if="r.responsable">Responsable: {{ r.responsable }}</span>
                        <span v-if="r.kilometraje">{{ r.kilometraje }} km</span>
                    </div>
                    <p v-if="r.observaciones" class="historial-obs">{{ r.observaciones }}</p>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, .35);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 999;
    padding: 20px;
}

.modal-box-historial {
    background: #fff;
    border-radius: 16px;
    padding: 22px 24px;
    max-width: 560px;
    width: 100%;
    max-height: 85vh;
    overflow-y: auto;
    box-shadow: 0 8px 30px rgba(0, 0, 0, .15);
}

.historial-header {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    margin-bottom: 16px;
}

.modal-titulo {
    font-size: 1.05rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 4px;
}

.modal-desc {
    font-size: .8rem;
    color: #6b7280;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 8px;
}

.placa-badge {
    background: #111827;
    color: #fff;
    font-size: .72rem;
    font-weight: 700;
    padding: 3px 9px;
    border-radius: 6px;
    letter-spacing: .05em;
}

.btn-cerrar {
    width: 30px;
    height: 30px;
    flex-shrink: 0;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s;
}

.btn-cerrar:hover {
    background: #e5e7eb;
}

.historial-vacio {
    padding: 40px 0;
    text-align: center;
    color: #9ca3af;
    font-size: .875rem;
}

.historial-lista {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.historial-item {
    background: #f9fafb;
    border: 1px solid #f3f4f6;
    border-radius: 10px;
    padding: 14px 16px;
}

.historial-item-top {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 6px;
}

.historial-tipo {
    font-size: .78rem;
    font-weight: 600;
    color: #374151;
}

.historial-costo {
    margin-left: auto;
    font-size: .82rem;
    font-weight: 700;
    color: #111827;
}

.historial-desc {
    font-size: .85rem;
    color: #374151;
    margin: 0 0 8px;
}

.historial-fechas,
.historial-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
    font-size: .72rem;
    color: #6b7280;
    margin-bottom: 4px;
}

.historial-obs {
    font-size: .78rem;
    color: #6b7280;
    background: #fff;
    border: 1px dashed #e5e7eb;
    border-radius: 6px;
    padding: 8px 10px;
    margin: 8px 0 0;
    white-space: pre-wrap;
}

.badge {
    display: inline-block;
    padding: 3px 9px;
    border-radius: 20px;
    font-size: .71rem;
    font-weight: 700;
    white-space: nowrap;
}

.badge-programado {
    background: #dbeafe;
    color: #1e40af;
}

.badge-en-proceso {
    background: #fef3c7;
    color: #92400e;
}

.badge-completado {
    background: #d1fae5;
    color: #065f46;
}

.badge-cancelado {
    background: #fee2e2;
    color: #991b1b;
}
</style>