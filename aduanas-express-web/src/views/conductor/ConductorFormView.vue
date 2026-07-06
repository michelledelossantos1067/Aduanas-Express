<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { obtenerUsuariosPorRol } from '@/services/usuarioService'

import {
    crearConductor,
    actualizarConductor,
    eliminarConductor,
    verConductorPorId
} from '@/services/conductorService'

const router = useRouter()
const route = useRoute()
const supervisores = ref([])

const loading = ref(false)
const error = ref('')
const mostrarConfirmacion = ref(false)

const esEdicion = computed(() => !!route.params.id)

const form = ref({
    nombre: '',
    apellido: '',
    cedula: '',
    numeroLicencia: '',
    tipoLicencia: '',
    fechaVencLicencia: '',
    telefono: '',
    direccion: '',
    supervisorId: null,
    estado: 0
})

function formatearCedula(e) {
    let digits = e.target.value.replace(/\D/g, '').slice(0, 11)
    console.log('digits:', digits)
    let formatted = digits
    if (digits.length > 3 && digits.length <= 10) {
        formatted = digits.slice(0, 3) + '-' + digits.slice(3)
    } else if (digits.length > 10) {
        formatted = digits.slice(0, 3) + '-' + digits.slice(3, 10) + '-' + digits.slice(10)
    }
    form.value.cedula = formatted
    console.log('formatted:', formatted)
}

function formatearCedulaStr(val) {
    const d = (val ?? '').replace(/\D/g, '').slice(0, 11)
    if (d.length > 10) return d.slice(0, 3) + '-' + d.slice(3, 10) + '-' + d.slice(10)
    if (d.length > 3) return d.slice(0, 3) + '-' + d.slice(3)
    return d
}

function formatearLicencia(e) {
    let digits = e.target.value.replace(/\D/g, '').slice(0, 10)
    let formatted = digits
    if (digits.length > 6) {
        formatted = digits.slice(0, 3) + '-' + digits.slice(3, 6) + '-' + digits.slice(6)
    } else if (digits.length > 3) {
        formatted = digits.slice(0, 3) + '-' + digits.slice(3)
    }
    form.value.numeroLicencia = formatted
}

async function guardar() {
    try {
        loading.value = true
        error.value = ''

        if (!form.value.nombre) { error.value = 'El nombre es requerido.'; return }
        if (!form.value.apellido) { error.value = 'El apellido es requerido.'; return }
        if (!form.value.cedula) { error.value = 'La cédula es requerida.'; return }

        const cedulaRegex = /^\d{3}-\d{7}-\d{1}$/
        if (!cedulaRegex.test(form.value.cedula)) {
            error.value = 'Cédula inválida. Use el formato 001-0000000-0.'
            return
        }

        if (!form.value.numeroLicencia) { error.value = 'El número de licencia es requerido.'; return }
        if (!form.value.tipoLicencia) { error.value = 'El tipo de licencia es requerido.'; return }
        if (!form.value.fechaVencLicencia) { error.value = 'La fecha de vencimiento es requerida.'; return }
        if (!form.value.telefono) { error.value = 'El teléfono es requerido.'; return }
        if (!form.value.direccion) { error.value = 'La dirección es requerida.'; return }

        const payload = {
            ...form.value,
            cedula: form.value.cedula,
            supervisorId: Number(form.value.supervisorId),
            estado: Number(form.value.estado),
            fechaVencLicencia: form.value.fechaVencLicencia
                ? new Date(form.value.fechaVencLicencia + 'T00:00:00').toISOString()
                : null
        }
        console.log('cedula enviada:', payload.cedula, 'longitud:', payload.cedula.length)

        if (esEdicion.value) {
            console.log('payload completo:', JSON.stringify(payload))
            await actualizarConductor(route.params.id, payload)
        } else {
            await crearConductor(payload)
        }
        console.log('cedula enviada:', payload.cedula, '| longitud:', payload.cedula.length)

        router.push('/conductores')
    } catch (e) {
        console.log('error completo:', JSON.stringify(e?.response?.data))
        const data = e?.response?.data
        if (data?.errors) {
            error.value = Object.values(data.errors).flat().join(' ')
        } else {
            error.value = data?.message || data?.title || 'Error al guardar el conductor.'
        }
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
        await eliminarConductor(route.params.id)
        router.push('/conductores')
    } catch (e) {
        error.value = e?.response?.data?.message || 'Error al eliminar el conductor.'
    } finally {
        loading.value = false
        mostrarConfirmacion.value = false
    }
}

async function cargarConductor() {
    try {
        loading.value = true

        const response = await verConductorPorId(route.params.id)
        const data = Array.isArray(response.data)
            ? response.data.find(c => c.id == route.params.id)
            : response.data
        if (!data) throw new Error('Conductor no encontrado.')

        function formatearFecha(fechaStr) {
            if (!fechaStr || fechaStr.startsWith('0001')) return ''

            try {
                if (typeof fechaStr === 'string' && fechaStr.match(/^\d{4}-\d{2}-\d{2}/)) {
                    return fechaStr.substring(0, 10)
                }

                const fecha = new Date(fechaStr)
                if (isNaN(fecha.getTime())) return ''

                const year = fecha.getFullYear()
                const month = String(fecha.getMonth() + 1).padStart(2, '0')
                const day = String(fecha.getDate()).padStart(2, '0')

                return `${year}-${month}-${day}`
            } catch (e) {
                console.error('Error al formatear fecha:', e)
                return ''
            }
        }

        form.value = {
            nombre: data.nombre ?? '',
            apellido: data.apellido ?? '',
            cedula: formatearCedulaStr(data.cedula ?? ''),
            numeroLicencia: data.numeroLicencia ?? '',
            tipoLicencia: data.tipoLicencia ?? '',
            fechaVencLicencia: formatearFecha(data.fechaVencLicencia),
            telefono: data.telefono ?? '',
            direccion: data.direccion ?? '',
            supervisorId: data.supervisorId ?? null,
            estado: data.estado ?? 0
        }
        console.log('Fecha cargada:', form.value.fechaVencLicencia)
        console.log('Data completo:', JSON.stringify(data))
    } catch (e) {
        error.value = e?.response?.data?.message || 'No se pudo cargar el conductor.'
    } finally {
        loading.value = false
    }
}

onMounted(async () => {
    const res = await obtenerUsuariosPorRol('Supervisor')
    supervisores.value = res.data
    if (esEdicion.value) await cargarConductor()
})
</script>

<template>
    <div class="cf-page">

        <div class="cf-header">
            <div class="cf-header-left">
                <button class="btn-back" @click="router.push('/conductores')">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <polyline points="15 18 9 12 15 6" />
                    </svg>
                    Conductores
                </button>
                <div class="cf-breadcrumb-sep">/</div>
                <span class="cf-breadcrumb-current">{{ esEdicion ? 'Editar Conductor' : 'Nuevo Conductor' }}</span>
            </div>
        </div>

        <div class="cf-page-title">
            <div class="cf-title-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                    <circle cx="12" cy="7" r="4" />
                </svg>
            </div>
            <div>
                <h1>{{ esEdicion ? 'Editar Conductor' : 'Registrar Nuevo Conductor' }}</h1>
                <p>{{ esEdicion ? 'Actualice los datos del conductor seleccionado.' : 'Complete el formulario para incorporar un conductor a la flota.' }}</p>
            </div>
        </div>

        <div v-if="error" class="cf-alert">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            {{ error }}
        </div>

        <div class="cf-layout">

            <aside class="cf-aside">
                <div class="aside-section">
                    <p class="aside-label">Módulo</p>
                    <p class="aside-value">Gestión de Conductores</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Operación</p>
                    <p class="aside-value">{{ esEdicion ? 'Modificación de registro' : 'Alta de conductor' }}</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Campos obligatorios</p>
                    <ul class="aside-list">
                        <li>Nombre y apellido</li>
                        <li>Cédula</li>
                        <li>Número de licencia</li>
                        <li>Tipo de licencia</li>
                        <li>Fecha de vencimiento</li>
                        <li>Teléfono</li>
                        <li>Dirección</li>
                    </ul>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Estado actual</p>
                    <span class="aside-badge" :class="{
                        'badge-activo': form.estado === 0,
                        'badge-disponible': form.estado === 1,
                        'badge-suspendido': form.estado === 2,
                        'badge-inactivo': form.estado === 3,
                    }">
                        {{
                            form.estado === 0 ? 'Activo' :
                                form.estado === 1 ? 'Disponible' :
                                    form.estado === 2 ? 'Suspendido' :
                                        'Inactivo'
                        }}
                    </span>
                </div>
            </aside>

            <div class="cf-card">

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">01</span>
                        <h3>Identidad Personal</h3>
                    </div>
                    <div class="form-grid col-3">
                        <div class="field">
                            <label>Nombre <span class="req">*</span></label>
                            <input v-model="form.nombre" type="text" placeholder="Juan" />
                        </div>
                        <div class="field">
                            <label>Apellido <span class="req">*</span></label>
                            <input v-model="form.apellido" type="text" placeholder="Pérez" />
                        </div>
                        <div class="field field-highlight">
                            <label>Cédula <span class="req">*</span></label>
                            <input :value="form.cedula" type="text" placeholder="001-0000000-0" maxlength="13"
                                @input="formatearCedula" autocomplete="off" />
                        </div>
                    </div>
                </div>

                <div class="section-divider"></div>

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">02</span>
                        <h3>Licencia de Conducir</h3>
                    </div>
                    <div class="form-grid col-3">
                        <div class="field field-highlight">
                            <label>Número de licencia <span class="req">*</span></label>
                            <input :value="form.numeroLicencia" type="text" placeholder="809-000-0000"
                                maxlength="12" autocomplete="off" @input="formatearLicencia" />
                        </div>
                        <div class="field">
                            <label>Tipo de licencia <span class="req">*</span></label>
                            <select v-model="form.tipoLicencia">
                                <option value="" disabled>Seleccionar…</option>
                                <option value="A">A — Motocicleta</option>
                                <option value="B">B — Vehículo liviano</option>
                                <option value="C">C — Camión</option>
                                <option value="D">D — Autobús</option>
                                <option value="E">E — Vehículo articulado</option>
                            </select>
                        </div>
                        <div class="field">
                            <label>Fecha de vencimiento <span class="req">*</span></label>
                            <input type="date" v-model="form.fechaVencLicencia" />
                        </div>
                    </div>
                </div>

                <div class="section-divider"></div>

                <div class="form-section">
                    <div class="section-header">
                        <span class="section-tag">03</span>
                        <h3>Contacto y Operación</h3>
                    </div>
                    <div class="form-grid col-3">
                        <div class="field">
                            <label>Teléfono <span class="req">*</span></label>
                            <input v-model="form.telefono" type="tel" placeholder="809-000-0000" />
                        </div>
                        <div class="field field-span2">
                            <label>Dirección <span class="req">*</span></label>
                            <input v-model="form.direccion" type="text" placeholder="Calle, sector, ciudad" />
                        </div>
                        <div class="field">
                            <label>Supervisor <span class="req">*</span></label>
                            <select v-model="form.supervisorId">
                                <option :value="null" disabled>Seleccione un supervisor</option>
                                <option v-for="s in supervisores" :key="s.id" :value="s.id">
                                    {{ s.nombre }} {{ s.apellido }}
                                </option>
                            </select>
                        </div>
                        <div class="field">
                            <label>Estado operativo</label>
                            <select v-model="form.estado">
                                <option :value="0">Activo</option>
                                <option :value="1">Disponible</option>
                                <option :value="2">Suspendido</option>
                                <option :value="3">Inactivo</option>
                            </select>
                        </div>
                    </div>
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
                        <button class="btn-secondary" @click="router.push('/conductores')" :disabled="loading">
                            Cancelar
                        </button>
                        <button class="btn-primary" @click="guardar" :disabled="loading">
                            <span v-if="loading" class="btn-spinner"></span>
                            <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                stroke-width="2.5">
                                <polyline points="20 6 9 17 4 12" />
                            </svg>
                            {{ loading ? 'Guardando…' : (esEdicion ? 'Actualizar Conductor' : 'Registrar Conductor') }}
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
                    <h2 class="modal-titulo">¿Eliminar este conductor?</h2>
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
.cf-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
    color: #111827;
}

.cf-header {
    margin-bottom: 24px;
}

.cf-header-left {
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

.cf-breadcrumb-sep {
    color: #d1d5db;
    font-size: 0.875rem;
}

.cf-breadcrumb-current {
    font-size: 0.875rem;
    font-weight: 600;
    color: #111827;
}

.cf-page-title {
    display: flex;
    align-items: center;
    gap: 14px;
    margin-bottom: 24px;
}

.cf-title-icon {
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

.cf-page-title h1 {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    letter-spacing: -0.02em;
}

.cf-page-title p {
    font-size: 0.875rem;
    color: #6b7280;
    margin: 3px 0 0;
}

.cf-alert {
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

.cf-layout {
    display: grid;
    grid-template-columns: 220px 1fr;
    gap: 20px;
    align-items: start;
}

.cf-aside {
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

.badge-activo {
    background: #d1fae5;
    color: #065f46;
}

.badge-disponible {
    background: #dbeafe;
    color: #1e40af;
}

.badge-suspendido {
    background: #fef3c7;
    color: #92400e;
}

.badge-inactivo {
    background: #f3f4f6;
    color: #6b7280;
}

.cf-card {
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

.field-span2 {
    grid-column: span 2;
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
    .cf-layout {
        grid-template-columns: 1fr;
    }

    .cf-aside {
        position: static;
    }

    .form-grid.col-3 {
        grid-template-columns: repeat(2, 1fr);
    }

    .field-span2 {
        grid-column: span 1;
    }
}

@media (max-width: 640px) {
    .cf-page {
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

    .form-grid,
    .form-grid.col-3 {
        grid-template-columns: 1fr;
    }

    .field-span2 {
        grid-column: span 1;
    }
}
</style>