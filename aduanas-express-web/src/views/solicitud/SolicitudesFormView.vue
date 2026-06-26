<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { verSolicitudPorId, crearSolicitud, actualizarSolicitud } from '../../services/solicitudService'

const router = useRouter()
const route = useRoute()

const loading = ref(false)
const error = ref('')

const esEdicion = computed(() => !!route.params.id)

const form = ref({
    areaSolicitante: '',
    destino: '',
    puntoOrigen: '',
    tipoViaje: 0,
    motivoViaje: '',
    fechaViaje: '',
    horaSalida: '',
    horaEstimada: '',
    cantidadColaboradores: 1,
    estado: 0,
})

const estadosSolicitud = [
    { value: 0, label: 'Pendiente' },
    { value: 1, label: 'Aprobada' },
    { value: 2, label: 'Rechazada' },
    { value: 3, label: 'Cancelada' },
    { value: 4, label: 'Finalizada' },
]

const estadoLabel = computed(() => {
    const e = estadosSolicitud.find(e => e.value === form.value.estado)
    return e ? e.label : 'Desconocido'
})

const estadoBadgeClase = computed(() => {
    const mapa = {
        0: 'badge-pendiente',
        1: 'badge-aprobada',
        2: 'badge-rechazada',
        3: 'badge-cancelada',
        4: 'badge-finalizada',
    }
    return mapa[form.value.estado] ?? 'badge-pendiente'
})


const origenQuery = ref('')
const destinoQuery = ref('')

const origenes = ref([])
const destinos = ref([])

let timeoutOrigen = null
let timeoutDestino = null

async function buscarUbicaciones(query, target) {
    if (!query || query.length < 3) return

    try {
        const res = await fetch(
            `https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}&limit=10&addressdetails=1&countrycodes=do`,
            {
                headers: {
                    'Accept-Language': 'es'
                }
            }
        )
        const data = await res.json()
        target.value = data.map(d => d.display_name)

    } catch (e) {
        target.value = []
    }
}

watch(origenQuery, (val) => {
    clearTimeout(timeoutOrigen)
    timeoutOrigen = setTimeout(() => {
        buscarUbicaciones(val, origenes)
    }, 400)
})

watch(destinoQuery, (val) => {
    clearTimeout(timeoutDestino)
    timeoutDestino = setTimeout(() => {
        buscarUbicaciones(val, destinos)
    }, 400)
})


async function cargarSolicitud() {
    loading.value = true
    error.value = ''
    try {
        const res = await verSolicitudPorId(route.params.id)
        const s = res.data

        form.value = {
            areaSolicitante: s.areaSolicitante ?? '',
            destino: s.destino ?? '',
            puntoOrigen: s.puntoOrigen ?? '',
            tipoViaje: s.tipoViaje ?? 0,
            motivoViaje: s.motivoViaje ?? '',
            fechaViaje: s.fechaViaje ? s.fechaViaje.substring(0, 10) : '',
            horaSalida: s.horaSalida ? s.horaSalida.substring(0, 5) : '',
            horaEstimada: s.horaEstimada ? s.horaEstimada.substring(0, 5) : '',
            cantidadColaboradores: s.cantidadColaboradores ?? 1,
            estado: s.estado ?? 0,
        }
        if (s.puntoOrigen) {
            origenes.value = [s.puntoOrigen]
        }
        if (s.destino) {
            destinos.value = [s.destino]
        }

    } catch (e) {
        error.value = 'No se pudo cargar la solicitud.'
    } finally {
        loading.value = false
    }
}

async function guardar() {
    error.value = ''

    const payload = { ...form.value }

    loading.value = true
    try {
        if (esEdicion.value) {
            await actualizarSolicitud(route.params.id, payload)
        } else {
            await crearSolicitud(payload)
        }
        router.push('/solicitudes')
    } catch (e) {
        error.value = 'Error al guardar'
    } finally {
        loading.value = false
    }
}

onMounted(() => {
    if (esEdicion.value) cargarSolicitud()
})
</script>
<template>
    <div class="sf-page">

        <div class="sf-header">
            <div class="sf-header-left">
                <button class="btn-back" @click="router.push('/solicitudes')">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <polyline points="15 18 9 12 15 6" />
                    </svg>
                    Solicitudes
                </button>
                <div class="sf-breadcrumb-sep">/</div>
                <span class="sf-breadcrumb-current">
                    {{ esEdicion ? 'Editar solicitud' : 'Nueva solicitud' }}
                </span>
            </div>
        </div>

        <div class="sf-page-title">
            <div class="sf-title-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <rect x="1" y="3" width="15" height="13" rx="2" />
                    <path d="M16 8h4l3 3v5h-7V8z" />
                    <circle cx="5.5" cy="18.5" r="2.5" />
                    <circle cx="18.5" cy="18.5" r="2.5" />
                </svg>
            </div>
            <div>
                <h1>{{ esEdicion ? 'Editar solicitud' : 'Nueva solicitud de transporte' }}</h1>
                <p>{{ esEdicion ? 'Actualice los datos de la solicitud seleccionada.' : 'Complete el formulario para registrar una nueva solicitud de transporte.' }}</p>
            </div>
        </div>

        <div v-if="error" class="sf-alert">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            {{ error }}
        </div>

        <div class="sf-layout">

            <aside class="sf-aside">
                <div class="aside-section">
                    <p class="aside-label">Módulo</p>
                    <p class="aside-value">Gestión de Transporte</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Operación</p>
                    <p class="aside-value">{{ esEdicion ? 'Modificación de solicitud' : 'Nueva solicitud' }}</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Campos obligatorios</p>
                    <ul class="aside-list">
                        <li>Área solicitante</li>
                        <li>Punto de origen</li>
                        <li>Destino</li>
                        <li>Fecha de viaje</li>
                        <li>Hora de salida</li>
                    </ul>
                </div>
                <div v-if="esEdicion" class="aside-divider"></div>
                <div v-if="esEdicion" class="aside-section">
                    <p class="aside-label">Estado actual</p>
                    <span class="aside-badge" :class="estadoBadgeClase">
                        {{ estadoLabel }}
                    </span>
                </div>
            </aside>

            <div class="sf-card">

                <div v-if="loading && esEdicion" class="sf-loading">
                    <div class="spinner"></div>
                </div>

                <template v-else>

                    <div class="form-section">
                        <div class="section-header">
                            <span class="section-tag">01</span>
                            <h3>Solicitante</h3>
                        </div>
                        <div class="form-grid">
                            <div class="field field-highlight form-full">
                                <label>Área solicitante <span class="req">*</span></label>
                                <input v-model="form.areaSolicitante" type="text" placeholder="Ej. Recursos Humanos"
                                    autocomplete="off" />
                            </div>
                            <div class="field">
                                <label>Cantidad de colaboradores</label>
                                <div class="input-suffix-wrap">
                                    <input v-model.number="form.cantidadColaboradores" type="number" min="1"
                                        placeholder="1" />
                                    <span class="input-suffix">personas</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="section-divider"></div>

                    <div class="form-section">
                        <div class="section-header">
                            <span class="section-tag">02</span>
                            <h3>Ruta y Horario</h3>
                        </div>
                        <div class="form-grid">
                            <div class="field">
                                <label>Punto de origen <span class="req">*</span></label>

                                <input v-model="origenQuery" type="text" placeholder="Buscar origen..." />

                                <select v-model="form.puntoOrigen">
                                    <option value="" disabled>Seleccione un origen</option>
                                    <option v-for="o in origenes" :key="o" :value="o">
                                        {{ o }}
                                    </option>
                                </select>
                            </div>

                            <div class="field">
                                <label>Destino <span class="req">*</span></label>

                                <input v-model="destinoQuery" type="text" placeholder="Buscar destino..." />

                                <select v-model="form.destino">
                                    <option value="" disabled>Seleccione un destino</option>
                                    <option v-for="d in destinos" :key="d" :value="d">
                                        {{ d }}
                                    </option>
                                </select>
                            </div>
                            <div class="field">
                                <label>Fecha de viaje <span class="req">*</span></label>
                                <input v-model="form.fechaViaje" type="date" />
                            </div>
                            <div class="field">

                            </div>
                            <div class="field">
                                <label>Hora de salida <span class="req">*</span></label>
                                <input v-model="form.horaSalida" type="time" />
                            </div>
                            <div class="field">
                                <label>Hora estimada</label>
                                <input v-model="form.horaEstimada" type="time" />
                            </div>
                        </div>
                    </div>

                    <div class="section-divider"></div>

                    <div class="form-section">
                        <div class="section-header">
                            <span class="section-tag">03</span>
                            <h3>Detalles del viaje</h3>
                        </div>
                        <div class="form-grid">
                            <div class="field form-full">
                                <label>Motivo del viaje</label>
                                <textarea v-model="form.motivoViaje" rows="4"
                                    placeholder="Describe el motivo del viaje..."></textarea>
                            </div>

                            <div v-if="esEdicion" class="field">
                                <label>Estado</label>
                                <select v-model.number="form.estado">
                                    <option v-for="e in estadosSolicitud" :key="e.value" :value="e.value">
                                        {{ e.label }}
                                    </option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="action-bar">
                        <div class="action-bar-left"></div>
                        <div class="action-bar-right">
                            <button class="btn-secondary" @click="router.push('/solicitudes')" :disabled="loading">
                                Cancelar
                            </button>
                            <button class="btn-primary" @click="guardar" :disabled="loading">
                                <span v-if="loading" class="btn-spinner"></span>
                                <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                                    stroke-width="2.5">
                                    <polyline points="20 6 9 17 4 12" />
                                </svg>
                                {{ loading ? 'Guardando…' : esEdicion ? 'Guardar cambios' : 'Crear solicitud' }}
                            </button>
                        </div>
                    </div>

                </template>
            </div>
        </div>
    </div>
</template>

<style scoped>
.sf-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
    color: #111827;
}

.sf-header {
    margin-bottom: 24px;
    display: flex;
    align-items: center;
    justify-content: space-between;
}

.sf-header-left {
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

.sf-breadcrumb-sep {
    color: #d1d5db;
    font-size: 0.875rem;
}

.sf-breadcrumb-current {
    font-size: 0.875rem;
    font-weight: 600;
    color: #111827;
}

.sf-page-title {
    display: flex;
    align-items: center;
    gap: 14px;
    margin-bottom: 24px;
}

.sf-title-icon {
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

.sf-page-title h1 {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    letter-spacing: -0.02em;
}

.sf-page-title p {
    font-size: 0.875rem;
    color: #6b7280;
    margin: 3px 0 0;
}

.sf-alert {
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

.sf-layout {
    display: grid;
    grid-template-columns: 220px 1fr;
    gap: 20px;
    align-items: start;
}

.sf-aside {
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

.badge-pendiente {
    background: #fef3c7;
    color: #92400e;
}

.badge-aprobada {
    background: #d1fae5;
    color: #065f46;
}

.badge-rechazada {
    background: #fee2e2;
    color: #991b1b;
}

.badge-cancelada {
    background: #dbeafe;
    color: #1e40af;
}

.badge-finalizada {
    background: #ede9fe;
    color: #6d28d9;
}

.sf-card {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .06);
    overflow: hidden;
}

.sf-loading {
    display: flex;
    justify-content: center;
    padding: 60px 0;
}

.spinner {
    width: 32px;
    height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
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

.form-full {
    grid-column: 1 / -1;
}

.field {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.field.field-highlight input {
    font-weight: 700;
    letter-spacing: 0.03em;
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
.field select,
.field textarea {
    padding: 0 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.875rem;
    color: #111827;
    background: #fff;
    outline: none;
    transition: border-color 0.15s, box-shadow 0.15s;
    font-family: inherit;
    width: 100%;
    box-sizing: border-box;
}

.field input,
.field select {
    height: 42px;
}

.field textarea {
    padding: 10px 12px;
    resize: vertical;
    min-height: 100px;
    height: auto;
    line-height: 1.5;
}

.field input:focus,
.field select:focus,
.field textarea:focus {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, 0.1);
}

.field input::placeholder,
.field textarea::placeholder {
    color: #9ca3af;
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
    padding: 0 12px;
    height: 42px;
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
.btn-secondary {
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
.btn-secondary:disabled {
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

.btn-spinner {
    width: 13px;
    height: 13px;
    border: 2px solid rgba(255, 255, 255, .4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin 0.65s linear infinite;
    flex-shrink: 0;
}

@media (max-width: 900px) {
    .sf-layout {
        grid-template-columns: 1fr;
    }

    .sf-aside {
        position: static;
    }
}

@media (max-width: 640px) {
    .sf-page {
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

    .tipo-viaje-group {
        flex-direction: column;
    }
}

.tipo-viaje-group {
    display: flex;
    gap: 12px;
}

.tipo-btn {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 11px 16px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    background: #fff;
    color: #374151;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    transition: border-color 0.15s, background 0.15s, color 0.15s;
    font-family: inherit;
    text-align: left;
}

.tipo-btn:hover {
    border-color: #1a3a2a;
    background: #f0fdf4;
}

.tipo-btn-activo {
    border-color: #1a3a2a;
    background: #f0fdf4;
    color: #1a3a2a;
}

.tipo-hint {
    font-size: 0.72rem;
    font-weight: 400;
    color: #9ca3af;
    margin-left: auto;
}

.tipo-btn-activo .tipo-hint {
    color: #4d7c5f;
}
</style>