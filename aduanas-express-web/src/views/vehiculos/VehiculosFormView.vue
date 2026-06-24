<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

import {
    crearVehiculo,
    actualizarVehiculo,
    eliminarVehiculo,
    verVehiculoPorId
} from '@/services/vehiculoService'

const router = useRouter()
const route = useRoute()

const loading = ref(false)
const error = ref('')
const mostrarConfirmacion = ref(false)

const esEdicion = computed(() => !!route.params.id)

const form = ref({
    marca: '',
    modelo: '',
    año: null,
    matricula: '',
    color: '',
    tipo: '',
    capacidad: 0,
    estado: 0,
    kilometraje: 0,
    fechaUltimoMant: null
})

async function guardar() {
    try {
        loading.value = true
        error.value = ''

        if (!form.value.matricula) { error.value = 'La matrícula es requerida.'; return }
        if (!form.value.marca) { error.value = 'La marca es requerida.'; return }
        if (!form.value.modelo) { error.value = 'El modelo es requerido.'; return }
        if (!form.value.tipo) { error.value = 'El tipo es requerido.'; return }
        if (!form.value.color) { error.value = 'El color es requerido.'; return }

        if (esEdicion.value) {
            await actualizarVehiculo(route.params.id, form.value)
        } else {
            const payload = { ...form.value, fechaUltimoMant: form.value.fechaUltimoMant || null }
            await crearVehiculo(payload)
        }

        router.push('/vehiculos')
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al guardar el vehículo.'
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
        await eliminarVehiculo(route.params.id)
        router.push('/vehiculos')
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al eliminar el vehículo.'
    } finally {
        loading.value = false
        mostrarConfirmacion.value = false
    }
}

async function cargarVehiculo() {
    try {
        loading.value = true
        const response = await verVehiculoPorId(route.params.id)
        const data = Array.isArray(response.data)
            ? response.data.find(v => v.id == route.params.id)
            : response.data
        if (!data) throw new Error('Vehículo no encontrado.')

        // Formatear la fecha si existe
        let fechaFormato = ''
        if (data.fechaUltimoMant) {
            const fecha = new Date(data.fechaUltimoMant)
            fechaFormato = fecha.toISOString().split('T')[0]
        }

        form.value = {
            marca: data.marca,
            modelo: data.modelo,
            año: data.año,
            matricula: data.matricula,
            color: data.color,
            tipo: data.tipo,
            capacidad: data.capacidad,
            estado: data.estado,
            kilometraje: data.kilometraje,
            fechaUltimoMant: fechaFormato
        }
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'No se pudo cargar el vehículo.'
    } finally {
        loading.value = false
    }
}
onMounted(async () => {
    if (esEdicion.value) await cargarVehiculo()
})
</script>

<template>
    <div class="vf-page">

        <div class="vf-header">
            <div class="vf-header-left">
                <button class="btn-back" @click="router.push('/vehiculos')">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <polyline points="15 18 9 12 15 6" />
                    </svg>
                    Vehículos
                </button>
                <div class="vf-breadcrumb-sep">/</div>
                <span class="vf-breadcrumb-current">{{ esEdicion ? 'Editar Vehículo' : 'Nuevo Vehículo' }}</span>
            </div>
        </div>

        <div class="vf-page-title">
            <div class="vf-title-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="1" y="3" width="15" height="13" rx="2" />
                    <path d="M16 8h4l3 3v5h-7V8z" />
                    <circle cx="5.5" cy="18.5" r="2.5" />
                    <circle cx="18.5" cy="18.5" r="2.5" />
                </svg>
            </div>
            <div>
                <h1>{{ esEdicion ? 'Editar Vehículo' : 'Registrar Nuevo Vehículo' }}</h1>
                <p>{{ esEdicion ? 'Actualice los datos del vehículo seleccionado.' : 'Complete el formulario para incorporar un vehículo a la flota.' }}</p>
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
                    <p class="aside-value">Gestión de Flota</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Operación</p>
                    <p class="aside-value">{{ esEdicion ? 'Modificación de registro' : 'Alta de vehículo' }}</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Campos obligatorios</p>
                    <ul class="aside-list">
                        <li>Matrícula</li>
                        <li>Marca</li>
                        <li>Modelo</li>
                        <li>Tipo</li>
                        <li>Color</li>
                    </ul>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Estado actual</p>
                    <span class="aside-badge" :class="{
                        'badge-disponible': form.estado === 0,
                        'badge-en-viaje': form.estado === 1,
                        'badge-mantenimiento': form.estado === 2,
                        'badge-fuera-servicio': form.estado === 3,
                    }">
                        {{
                            form.estado === 0 ? 'Disponible' :
                                form.estado === 1 ? 'En Viaje' :
                                    form.estado === 2 ? 'En Mantenimiento' :
                                        'Fuera de Servicio'
                        }}
                    </span>
                </div>
            </aside>

            <div class="vf-card">

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">01</span>
                        <h3>Identificación</h3>
                    </div>
                    <div class="form-grid">
                        <div class="field field-highlight">
                            <label>Matrícula <span class="req">*</span></label>
                            <input v-model="form.matricula" type="text" placeholder="Ej. A-123456" autocomplete="off" />
                        </div>
                        <div class="field">
                            <label>Año</label>
                            <input v-model="form.año" type="number" placeholder="2024" min="1900" max="2100" />
                        </div>
                    </div>
                </div>

                <div class="section-divider"></div>

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">02</span>
                        <h3>Descripción del Vehículo</h3>
                    </div>
                    <div class="form-grid col-3">
                        <div class="field">
                            <label>Marca <span class="req">*</span></label>
                            <input v-model="form.marca" type="text" placeholder="Toyota" />
                        </div>
                        <div class="field">
                            <label>Modelo <span class="req">*</span></label>
                            <input v-model="form.modelo" type="text" placeholder="Hilux" />
                        </div>
                        <div class="field">
                            <label>Color <span class="req">*</span></label>
                            <input v-model="form.color" type="text" placeholder="Blanco" />
                        </div>
                        <div class="field">
                            <label>Tipo <span class="req">*</span></label>
                            <select v-model="form.tipo">
                                <option value="" disabled>Seleccionar tipo…</option>
                                <option>Automóvil</option>
                                <option>Jeepeta</option>
                                <option>Camioneta</option>
                                <option>Camión</option>
                                <option>Autobús</option>
                                <option>Motocicleta</option>
                                <option>Otro</option>
                            </select>
                        </div>
                        <div class="field">
                            <label>Capacidad</label>
                            <div class="input-suffix-wrap">
                                <input v-model="form.capacidad" type="number" placeholder="5" min="0" />
                                <span class="input-suffix">pasajeros</span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="section-divider"></div>

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">03</span>
                        <h3>Operación y Mantenimiento</h3>
                    </div>
                    <div class="form-grid">
                        <div class="field">
                            <label>Estado operativo</label>
                            <select v-model="form.estado">
                                <option :value="0">Disponible</option>
                                <option :value="1">En Viaje</option>
                                <option :value="2" disabled>En Mantenimiento</option>
                                <option :value="3">Fuera de Servicio</option>
                            </select>
                            <p v-if="form.estado === 2" class="field-hint">
                                Este estado lo controla automáticamente el módulo de Mantenimiento y no puede
                                cambiarse aquí manualmente. Para liberar el vehículo, completa o cancela su
                                mantenimiento activo.
                            </p>
                        </div>
                        <div class="field">
                            <label>Kilometraje</label>
                            <div class="input-suffix-wrap">
                                <input v-model="form.kilometraje" type="number" placeholder="0" min="0" />
                                <span class="input-suffix">km</span>
                            </div>
                        </div>
                        <div class="field">
                            <label>Último mantenimiento</label>
                            <input type="date" v-model="form.fechaUltimoMant" />
                        </div>
                    </div>
                    <button v-if="esEdicion" type="button" class="btn-mantenimiento-link"
                        @click="router.push({ path: '/mantenimiento', query: { vehiculoId: route.params.id } })">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2">
                            <rect x="1" y="3" width="15" height="13" rx="2" />
                            <path d="M16 8h4l3 3v5h-7V8z" />
                            <circle cx="5.5" cy="18.5" r="2.5" />
                            <circle cx="18.5" cy="18.5" r="2.5" />
                        </svg>
                        Registrar mantenimiento para este vehículo
                    </button>
                </div>

                <div class="action-bar">
                    <div class="action-bar-left">
                        <button v-if="esEdicion" class="btn-danger" @click="confirmarEliminar" :disabled="loading">
                            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2">
                                <polyline points="3 6 5 6 21 6" />
                                <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                                <path d="M10 11v6" />
                                <path d="M14 11v6" />
                            </svg>
                            Eliminar registro
                        </button>
                    </div>
                    <div class="action-bar-right">
                        <button class="btn-secondary" @click="router.push('/vehiculos')" :disabled="loading">
                            Cancelar
                        </button>
                        <button class="btn-primary" @click="guardar" :disabled="loading">
                            <span v-if="loading" class="btn-spinner"></span>
                            <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2.5">
                                <polyline points="20 6 9 17 4 12" />
                            </svg>
                            {{ loading ? 'Guardando…' : (esEdicion ? 'Actualizar Vehículo' : 'Registrar Vehículo') }}
                        </button>
                    </div>
                </div>

            </div>
        </div>

        <Teleport to="body">
            <div v-if="mostrarConfirmacion" class="modal-overlay" @click.self="mostrarConfirmacion = false">
                <div class="modal">
                    <div class="modal-icon">
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2">
                            <path
                                d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                            <line x1="12" y1="9" x2="12" y2="13" />
                            <line x1="12" y1="17" x2="12.01" y2="17" />
                        </svg>
                    </div>
                    <h2 class="modal-titulo">¿Eliminar este vehículo?</h2>
                    <p class="modal-desc">
                        Esta acción eliminará el registro de forma permanente y no podrá deshacerse.
                    </p>
                    <div class="modal-acciones">
                        <button class="btn-cancelar-modal" @click="mostrarConfirmacion = false">Cancelar</button>
                        <button class="btn-confirmar-modal" @click="eliminar" :disabled="loading">
                            {{ loading ? 'Eliminando…' : 'Confirmar eliminación' }}
                        </button>
                    </div>
                </div>
            </div>
        </Teleport>

    </div>
</template>

<style scoped>
:root {
    --green: #1a3a2a;
    --green-h: #14532d;
    --surface: #f3f4f6;
    --card: #ffffff;
    --border: #e5e7eb;
    --text: #111827;
    --muted: #6b7280;
    --tag-bg: #f0fdf4;
    --tag-text: #166534;
}

.vf-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
    color: #111827;
}

.vf-header {
    margin-bottom: 24px;
    display: flex;
    align-items: center;
    justify-content: space-between;
}

.vf-header-left {
    display: flex;
    align-items: center;
    gap: 8px;
}

.btn-back {
    display: inline-flex;
    align-items: center;
    gap: 5px;
    background: none;
    border: none;
    font-size: 0.875rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    padding: 0;
    transition: color 0.15s;
}

.btn-back:hover {
    color: #1a3a2a;
}

.vf-breadcrumb-sep {
    color: #d1d5db;
    font-size: 0.875rem;
}

.vf-breadcrumb-current {
    font-size: 0.875rem;
    font-weight: 600;
    color: #111827;
}

.vf-page-title {
    display: flex;
    align-items: center;
    gap: 14px;
    margin-bottom: 24px;
}

.vf-title-icon {
    width: 44px;
    height: 44px;
    background: #1a3a2a;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #fff;
    flex-shrink: 0;
}

.vf-page-title h1 {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    letter-spacing: -0.02em;
}

.vf-page-title p {
    font-size: 0.875rem;
    color: #6b7280;
    margin: 3px 0 0;
}

.vf-alert {
    display: flex;
    align-items: center;
    gap: 10px;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 12px 16px;
    font-size: 0.875rem;
    color: #991b1b;
    margin-bottom: 20px;
}

.vf-layout {
    display: grid;
    grid-template-columns: 220px 1fr;
    gap: 20px;
    align-items: start;
}

.vf-aside {
    background: #fff;
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .06);
    position: sticky;
    top: 24px;
}

.aside-section {
    padding: 4px 0;
}

.aside-label {
    font-size: 0.7rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.08em;
    color: #9ca3af;
    margin: 0 0 4px;
}

.aside-value {
    font-size: 0.875rem;
    font-weight: 600;
    color: #111827;
    margin: 0;
}

.aside-divider {
    height: 1px;
    background: #f3f4f6;
    margin: 14px 0;
}

.aside-list {
    padding-left: 16px;
    margin: 0;
    list-style: disc;
}

.aside-list li {
    font-size: 0.8rem;
    color: #374151;
    margin-bottom: 3px;
}

.aside-badge {
    display: inline-block;
    padding: 3px 10px;
    border-radius: 20px;
    font-size: 0.73rem;
    font-weight: 600;
}

.badge-disponible {
    background: #d1fae5;
    color: #065f46;
}

.badge-en-viaje {
    background: #dbeafe;
    color: #1e40af;
}

.badge-mantenimiento {
    background: #fef3c7;
    color: #92400e;
}

.badge-fuera-servicio {
    background: #fee2e2;
    color: #991b1b;
}

.vf-card {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .06);
    overflow: hidden;
}

.form-section {
    padding: 28px 32px;
}

.section-header {
    display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 22px;
}

.section-tag {
    font-size: 0.7rem;
    font-weight: 700;
    letter-spacing: 0.06em;
    color: #166534;
    background: #f0fdf4;
    border: 1px solid #bbf7d0;
    border-radius: 4px;
    padding: 2px 7px;
}

.section-header h3 {
    font-size: 0.9rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    text-transform: uppercase;
    letter-spacing: 0.04em;
}

.section-divider {
    height: 1px;
    background: #f3f4f6;
    margin: 0 32px;
}

.form-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 18px;
}

.form-grid.col-3 {
    grid-template-columns: repeat(3, 1fr);
}

.field {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.field.field-highlight input {
    font-weight: 700;
    letter-spacing: 0.05em;
    font-size: 0.95rem;
    border-color: #1a3a2a;
}

.field label {
    font-size: 0.78rem;
    font-weight: 700;
    color: #374151;
    text-transform: uppercase;
    letter-spacing: 0.05em;
}

.req {
    color: #dc2626;
    margin-left: 2px;
}

.field input,
.field select {
    height: 42px;
    padding: 0 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.875rem;
    color: #111827;
    background: #fff;
    outline: none;
    transition: border-color 0.15s, box-shadow 0.15s;
    font-family: inherit;
}

.field input:focus,
.field select:focus {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, 0.1);
}

.field input::placeholder {
    color: #9ca3af;
}

.field select option:disabled {
    color: #9ca3af;
}

.field-hint {
    margin: 6px 0 0;
    font-size: 0.78rem;
    line-height: 1.4;
    color: #92400e;
    background: #fffbeb;
    border: 1px solid #fde68a;
    border-radius: 7px;
    padding: 8px 10px;
}

.btn-mantenimiento-link {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    margin-top: 16px;
    padding: 9px 16px;
    background: #fff;
    border: 1.5px solid #1a3a2a;
    border-radius: 8px;
    font-size: 0.82rem;
    font-weight: 600;
    color: #1a3a2a;
    cursor: pointer;
    transition: background 0.15s;
    font-family: inherit;
}

.btn-mantenimiento-link:hover {
    background: #f0fdf4;
}

.input-suffix-wrap {
    display: flex;
    align-items: stretch;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    overflow: hidden;
    transition: border-color 0.15s, box-shadow 0.15s;
}

.input-suffix-wrap:focus-within {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, 0.1);
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
    transition: background 0.15s, opacity 0.15s;
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
    animation: spin 0.65s linear infinite;
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
    transition: background 0.15s;
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

    .section-divider {
        margin: 0 16px;
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

    .form-grid {
        grid-template-columns: 1fr;
    }

    .form-grid.col-3 {
        grid-template-columns: 1fr;
    }
}
</style>
