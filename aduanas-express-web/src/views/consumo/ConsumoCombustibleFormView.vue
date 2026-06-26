<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { verConsumoPorId, crearConsumo, actualizarConsumo, eliminarConsumo } from '@/services/consumoCombustibleService'
import { verVehiculos } from '@/services/vehiculoService'
import { verSolicitud } from '@/services/solicitudService'

const router = useRouter()
const route = useRoute()

const loading = ref(false)
const error = ref('')
const mostrarConfirmacion = ref(false)
const vehiculos = ref([])
const solicitudes = ref([])

const esEdicion = computed(() => !!route.params.id)

const form = ref({
    galones: null,
    costoPorGalon: null,
    costoTotal: null,
    vehiculoId: null,
    solicitudId: null,
})

// Calcular costo total automáticamente
function calcularTotal() {
    if (form.value.galones && form.value.costoPorGalon) {
        form.value.costoTotal = (
            parseFloat(form.value.galones) * parseFloat(form.value.costoPorGalon)
        ).toFixed(2)
    }
}

async function guardar() {
    try {
        loading.value = true
        error.value = ''

        if (!form.value.galones || form.value.galones <= 0) { error.value = 'La cantidad de galones es requerida.'; return }
        if (!form.value.costoPorGalon || form.value.costoPorGalon <= 0) { error.value = 'El costo por galón es requerido.'; return }
        if (!form.value.costoTotal || form.value.costoTotal <= 0) { error.value = 'El costo total es requerido.'; return }
        if (!form.value.vehiculoId) { error.value = 'Debe seleccionar un vehículo.'; return }

        const payload = {
            galones: parseFloat(form.value.galones),
            costoPorGalon: parseFloat(form.value.costoPorGalon),
            costoTotal: parseFloat(form.value.costoTotal),
            vehiculoId: parseInt(form.value.vehiculoId),
            solicitudId: form.value.solicitudId ? parseInt(form.value.solicitudId) : null,
        }

        if (esEdicion.value) {
            await actualizarConsumo(route.params.id, payload)
        } else {
            await crearConsumo(payload)
        }

        router.push('/consumo-combustible')
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al guardar.'
    } finally {
        loading.value = false
    }
}

function confirmarEliminar() {
    mostrarConfirmacion.value = true
}

async function eliminar() {
    try {
        loading.value = true
        await eliminarConsumo(route.params.id)
        router.push('/consumo-combustible')
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al eliminar.'
    } finally {
        loading.value = false
        mostrarConfirmacion.value = false
    }
}

async function cargarConsumo() {
    try {
        loading.value = true
        const res = await verConsumoPorId(route.params.id)
        const data = res.data
        if (!data) throw new Error('Registro no encontrado.')

        form.value = {
            galones: data.Galones,
            costoPorGalon: data.CostoPorGalon,
            costoTotal: data.CostoTotal,
            vehiculoId: data.VehiculoId,
            solicitudId: data.SolicitudId ?? null,
        }
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'No se pudo cargar el registro.'
    } finally {
        loading.value = false
    }
}

onMounted(async () => {
    const resV = await verVehiculos()
    vehiculos.value = resV.data

    const resS = await verSolicitud()
    solicitudes.value = resS.data

    if (esEdicion.value) await cargarConsumo()
})
</script>

<template>
    <div class="vf-page">

        <div class="vf-header">
            <div class="vf-header-left">
                <button class="btn-back" @click="router.push('/consumo-combustible')">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <polyline points="15 18 9 12 15 6" />
                    </svg>
                    Consumo de Combustible
                </button>
                <div class="vf-breadcrumb-sep">/</div>
                <span class="vf-breadcrumb-current">{{ esEdicion ? 'Editar Registro' : 'Nuevo Registro' }}</span>
            </div>
        </div>

        <div class="vf-page-title">
            <div class="vf-title-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M3 22V8a2 2 0 0 1 2-2h7a2 2 0 0 1 2 2v14" />
                    <path d="M14 10h2a2 2 0 0 1 2 2v2" />
                    <path d="M18 14v4a2 2 0 0 0 4 0v-6l-2-4" />
                    <line x1="3" y1="22" x2="14" y2="22" />
                    <line x1="7" y1="6" x2="7" y2="2" />
                    <line x1="10" y1="6" x2="10" y2="2" />
                </svg>
            </div>
            <div>
                <h1>{{ esEdicion ? 'Editar Consumo' : 'Registrar Consumo de Combustible' }}</h1>
                <p>{{ esEdicion ? 'Actualice los datos del consumo.' : 'Complete el formulario para registrar el consumo.' }}</p>
            </div>
        </div>

        <div v-if="error" class="vf-alert">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            {{ error }}
        </div>

        <div class="vf-layout">

            <aside class="vf-aside">
                <div class="aside-section">
                    <p class="aside-label">Módulo</p>
                    <p class="aside-value">Combustible</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Operación</p>
                    <p class="aside-value">{{ esEdicion ? 'Modificación de registro' : 'Nuevo consumo' }}</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Campos obligatorios</p>
                    <ul class="aside-list">
                        <li>Vehículo</li>
                        <li>Galones</li>
                        <li>Costo por galón</li>
                        <li>Costo total</li>
                    </ul>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Costo total calculado</p>
                    <p class="aside-value" style="font-size: 1.1rem; font-weight: 700; color: #1a3a2a;">
                        {{ form.costoTotal ? `$${parseFloat(form.costoTotal).toFixed(2)}` : '—' }}
                    </p>
                </div>
            </aside>

            <div class="vf-card">

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">01</span>
                        <h3>Vehículo</h3>
                    </div>
                    <div class="form-grid">
                        <div class="field field-highlight">
                            <label>Vehículo <span class="req">*</span></label>
                            <select v-model="form.vehiculoId">
                                <option :value="null" disabled>Seleccionar vehículo…</option>
                                <option v-for="v in vehiculos" :key="v.Id" :value="v.Id">
                                    {{ v.Matricula }} — {{ v.Marca }} {{ v.Modelo }}
                                </option>
                            </select>
                        </div>
                        <div class="field">
                            <label>Solicitud <span class="field-optional">(opcional)</span></label>
                            <select v-model="form.solicitudId">
                                <option :value="null">Sin solicitud</option>
                                <option v-for="s in solicitudes" :key="s.id" :value="s.id">
                                    #{{ s.id }} — {{ s.descripcion ?? s.origen ?? 'Solicitud ' + s.id }}
                                </option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="section-divider"></div>

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">02</span>
                        <h3>Detalle del Consumo</h3>
                    </div>
                    <div class="form-grid col-3">
                        <div class="field">
                            <label>Galones <span class="req">*</span></label>
                            <div class="input-suffix-wrap">
                                <input v-model="form.galones" @input="calcularTotal" type="number" placeholder="0.00"
                                    min="0" step="0.01" />
                                <span class="input-suffix">gal</span>
                            </div>
                        </div>
                        <div class="field">
                            <label>Costo por Galón <span class="req">*</span></label>
                            <div class="input-suffix-wrap">
                                <span class="input-suffix"
                                    style="border-left: none; border-right: 1.5px solid #e5e7eb;">$</span>
                                <input v-model="form.costoPorGalon" @input="calcularTotal" type="number"
                                    placeholder="0.00" min="0" step="0.01" />
                            </div>
                        </div>
                        <div class="field">
                            <label>Costo Total <span class="req">*</span></label>
                            <div class="input-suffix-wrap">
                                <span class="input-suffix"
                                    style="border-left: none; border-right: 1.5px solid #e5e7eb;">$</span>
                                <input v-model="form.costoTotal" type="number" placeholder="Calculado automáticamente"
                                    min="0" step="0.01" />
                            </div>
                            <p class="field-hint">Se calcula automáticamente al ingresar galones × costo/galón.</p>
                        </div>
                    </div>
                </div>

                <div class="action-bar">
                    <div class="action-bar-left">
                        <button v-if="esEdicion" class="btn-danger" :disabled="loading" @click="confirmarEliminar">
                            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2">
                                <polyline points="3 6 5 6 21 6" />
                                <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                                <path d="M10 11v6M14 11v6" />
                            </svg>
                            Eliminar
                        </button>
                    </div>
                    <div class="action-bar-right">
                        <button class="btn-secondary" :disabled="loading" @click="router.push('/consumo-combustible')">
                            Cancelar
                        </button>
                        <button class="btn-primary" :disabled="loading" @click="guardar">
                            <span v-if="loading" class="btn-spinner"></span>
                            <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2.5">
                                <polyline points="20 6 9 17 4 12" />
                            </svg>
                            {{ esEdicion ? 'Guardar Cambios' : 'Registrar Consumo' }}
                        </button>
                    </div>
                </div>

            </div>
        </div>

        <!-- Modal confirmación eliminar -->
        <div v-if="mostrarConfirmacion" class="modal-overlay">
            <div class="modal">
                <div class="modal-icon">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path
                            d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                        <line x1="12" y1="9" x2="12" y2="13" />
                        <line x1="12" y1="17" x2="12.01" y2="17" />
                    </svg>
                </div>
                <p class="modal-titulo">¿Eliminar este registro?</p>
                <p class="modal-desc">Esta acción no se puede deshacer. El consumo de combustible será eliminado
                    permanentemente.</p>
                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="mostrarConfirmacion = false">Cancelar</button>
                    <button class="btn-confirmar-modal" :disabled="loading" @click="eliminar">
                        <span v-if="loading" class="btn-spinner" style="border-top-color: #fff;"></span>
                        Sí, eliminar
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<style scoped>
.vf-page {
    max-width: 1100px;
    margin: 0 auto;
    padding: 32px 24px;
}

.vf-header {
    margin-bottom: 24px;
}

.vf-header-left {
    display: flex;
    align-items: center;
    gap: 8px;
}

.btn-back {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    background: none;
    border: none;
    color: #6b7280;
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    padding: 0;
    font-family: inherit;
}

.btn-back:hover {
    color: #111827;
}

.vf-breadcrumb-sep {
    color: #d1d5db;
}

.vf-breadcrumb-current {
    font-size: 0.875rem;
    color: #111827;
    font-weight: 500;
}

.vf-page-title {
    display: flex;
    align-items: flex-start;
    gap: 14px;
    margin-bottom: 28px;
}

.vf-title-icon {
    width: 44px;
    height: 44px;
    background: #f0fdf4;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #1a3a2a;
    flex-shrink: 0;
}

.vf-page-title h1 {
    font-size: 1.35rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 4px;
}

.vf-page-title p {
    font-size: 0.875rem;
    color: #6b7280;
    margin: 0;
}

.vf-alert {
    display: flex;
    align-items: center;
    gap: 10px;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 12px 16px;
    color: #991b1b;
    font-size: 0.875rem;
    margin-bottom: 20px;
}

.vf-layout {
    display: grid;
    grid-template-columns: 220px 1fr;
    gap: 24px;
    align-items: start;
}

.vf-aside {
    background: #fff;
    border: 1px solid #f3f4f6;
    border-radius: 12px;
    padding: 20px;
    position: sticky;
    top: 24px;
}

.aside-section {
    padding: 4px 0;
}

.aside-label {
    font-size: 0.72rem;
    font-weight: 600;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .05em;
    margin: 0 0 4px;
}

.aside-value {
    font-size: 0.875rem;
    font-weight: 500;
    color: #111827;
    margin: 0;
}

.aside-divider {
    height: 1px;
    background: #f3f4f6;
    margin: 14px 0;
}

.aside-list {
    margin: 0;
    padding-left: 16px;
}

.aside-list li {
    font-size: 0.82rem;
    color: #374151;
    margin-bottom: 4px;
}

.vf-card {
    background: #fff;
    border: 1px solid #f3f4f6;
    border-radius: 12px;
    overflow: hidden;
}

.form-section {
    padding: 28px 32px;
}

.section-header {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 20px;
}

.section-tag {
    width: 24px;
    height: 24px;
    background: #1a3a2a;
    color: #fff;
    border-radius: 6px;
    font-size: 0.72rem;
    font-weight: 700;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.section-header h3 {
    font-size: 0.95rem;
    font-weight: 600;
    color: #111827;
    margin: 0;
}

.section-divider {
    height: 1px;
    background: #f3f4f6;
}

.form-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 16px;
}

.form-grid.col-3 {
    grid-template-columns: repeat(3, 1fr);
}

.field {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.field label {
    font-size: 0.82rem;
    font-weight: 600;
    color: #374151;
}

.field-optional {
    font-weight: 400;
    color: #9ca3af;
}

.req {
    color: #dc2626;
}

.field input,
.field select {
    width: 100%;
    padding: 9px 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.875rem;
    color: #111827;
    background: #fff;
    outline: none;
    transition: border-color .15s, box-shadow .15s;
    font-family: inherit;
    box-sizing: border-box;
}

.field input:focus,
.field select:focus {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, .1);
}

.field-highlight input,
.field-highlight select {
    border-color: #6ee7b7;
    background: #f0fdf4;
}

.field-hint {
    font-size: 0.78rem;
    color: #6b7280;
    margin: 0;
}

.input-suffix-wrap {
    display: flex;
    align-items: stretch;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    overflow: hidden;
    transition: border-color .15s, box-shadow .15s;
}

.input-suffix-wrap:focus-within {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, .1);
}

.input-suffix-wrap input {
    border: none;
    border-radius: 0;
    flex: 1;
    box-shadow: none !important;
}

.input-suffix-wrap input:focus {
    border: none;
    box-shadow: none;
}

.input-suffix {
    padding: 0 12px;
    background: #f9fafb;
    border-left: 1.5px solid #e5e7eb;
    font-size: 0.78rem;
    color: #6b7280;
    font-weight: 500;
    display: flex;
    align-items: center;
    white-space: nowrap;
}

.action-bar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 20px 32px;
    border-top: 1px solid #f3f4f6;
    background: #fafafa;
}

.action-bar-left,
.action-bar-right {
    display: flex;
    gap: 10px;
    align-items: center;
}

.btn-primary,
.btn-secondary,
.btn-danger {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    border: none;
    cursor: pointer;
    transition: background .15s, opacity .15s;
    font-family: inherit;
}

.btn-primary:disabled,
.btn-secondary:disabled,
.btn-danger:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

.btn-primary {
    background: #1a3a2a;
    color: #fff;
}

.btn-primary:hover:not(:disabled) {
    background: #14532d;
}

.btn-secondary {
    background: #f3f4f6;
    color: #374151;
    border: 1.5px solid #e5e7eb;
}

.btn-secondary:hover:not(:disabled) {
    background: #e5e7eb;
}

.btn-danger {
    background: #fff;
    color: #991b1b;
    border: 1.5px solid #fecaca;
}

.btn-danger:hover:not(:disabled) {
    background: #fef2f2;
}

.btn-spinner {
    width: 13px;
    height: 13px;
    border: 2px solid rgba(255, 255, 255, .4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .65s linear infinite;
    flex-shrink: 0;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

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
    border-radius: 14px;
    padding: 32px;
    width: 420px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
    text-align: center;
}

.modal-icon {
    width: 52px;
    height: 52px;
    background: #fef3c7;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #92400e;
    margin: 0 auto 16px;
}

.modal-titulo {
    font-size: 1.05rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 8px;
}

.modal-desc {
    font-size: 0.875rem;
    color: #4b5563;
    line-height: 1.55;
    margin: 0 0 24px;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: center;
}

.btn-cancelar-modal {
    padding: 9px 20px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    font-family: inherit;
}

.btn-confirmar-modal {
    padding: 9px 20px;
    background: #dc2626;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    font-family: inherit;
    transition: background .15s;
}

.btn-confirmar-modal:hover:not(:disabled) {
    background: #b91c1c;
}

.btn-confirmar-modal:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

@media (max-width: 900px) {
    .vf-layout {
        grid-template-columns: 1fr;
    }

    .vf-aside {
        position: static;
    }

    .form-grid.col-3 {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 640px) {
    .vf-page {
        padding: 20px 16px;
    }

    .form-section {
        padding: 20px 16px;
    }

    .action-bar {
        padding: 16px;
        flex-direction: column;
        gap: 12px;
    }

    .action-bar-right {
        width: 100%;
        justify-content: flex-end;
    }

    .form-grid,
    .form-grid.col-3 {
        grid-template-columns: 1fr;
    }
}
</style>