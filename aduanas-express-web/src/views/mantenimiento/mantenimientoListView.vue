<script setup>
import { ref, computed, onMounted } from 'vue'
import {
    verMantenimiento,
    crearMantenimiento,
    actualizarMantenimiento,
    eliminarMantenimiento,
} from '@/services/mantenimientoService.js'

const registros   = ref([])
const loading     = ref(false)
const guardando   = ref(false)
const eliminando  = ref(null)
const errorMsg    = ref('')
const exitoMsg    = ref('')

const vistaActiva = ref('lista')
const modoForm    = ref('crear')

const busqueda       = ref('')
const filtroEstado   = ref('')
const filtroTipo     = ref('')

const formInicial = () => ({
    id:               null,
    vehiculoId:       '',
    vehiculoPlaca:    '',
    tipo:             'Preventivo',
    descripcion:      '',
    fechaProgramada:  '',
    fechaRealizada:   '',
    kilometraje:      '',
    costo:            '',
    taller:           '',
    responsable:      '',
    observaciones:    '',
    estado:           'Programado',
})

const form = ref(formInicial())

const modalEliminar = ref(false)
const registroAEliminar = ref(null)

const TIPOS   = ['Preventivo', 'Correctivo', 'Emergencia']
const ESTADOS = ['Programado', 'En proceso', 'Completado', 'Cancelado']

const registrosFiltrados = computed(() => {
    const q = busqueda.value.toLowerCase()
    return registros.value.filter(r => {
        const matchQ = !q || [r.vehiculoPlaca, r.tipo, r.taller, r.descripcion, r.responsable]
            .some(v => v?.toLowerCase().includes(q))
        const matchE = !filtroEstado.value || r.estado === filtroEstado.value
        const matchT = !filtroTipo.value   || r.tipo   === filtroTipo.value
        return matchQ && matchE && matchT
    })
})

const resumen = computed(() => ({
    total:      registros.value.length,
    programados: registros.value.filter(r => r.estado === 'Programado').length,
    enProceso:   registros.value.filter(r => r.estado === 'En proceso').length,
    completados: registros.value.filter(r => r.estado === 'Completado').length,
    costoTotal:  registros.value.reduce((s, r) => s + (parseFloat(r.costo) || 0), 0),
}))

function formatFecha(f) {
    if (!f) return '—'
    return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatMoney(n) {
    const v = parseFloat(n) || 0
    return v.toLocaleString('es-DO', { style: 'currency', currency: 'DOP', maximumFractionDigits: 0 })
}

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

function estadoClase(estado) {
    return {
        'Programado': 'badge-programado',
        'En proceso': 'badge-en-proceso',
        'Completado': 'badge-completado',
        'Cancelado':  'badge-cancelado',
    }[estado] ?? ''
}

function tipoClase(tipo) {
    return {
        'Preventivo': 'badge-preventivo',
        'Correctivo': 'badge-correctivo',
        'Emergencia': 'badge-emergencia',
    }[tipo] ?? ''
}

async function cargar() {
    loading.value = true
    try {
        const res = await verMantenimiento()
        registros.value = res.data
    } catch (e) {
        console.error(e)
        errorMsg.value = 'Error al cargar los registros.'
        auto_limpiar()
    } finally {
        loading.value = false
    }
}

function abrirCrear() {
    form.value   = formInicial()
    modoForm.value  = 'crear'
    vistaActiva.value = 'form'
    errorMsg.value  = ''
}

function abrirEditar(r) {
    form.value = {
        id:              r.id,
        vehiculoId:      r.vehiculoId      ?? '',
        vehiculoPlaca:   r.vehiculoPlaca   ?? '',
        tipo:            r.tipo            ?? 'Preventivo',
        descripcion:     r.descripcion     ?? '',
        fechaProgramada: r.fechaProgramada ? r.fechaProgramada.substring(0,10) : '',
        fechaRealizada:  r.fechaRealizada  ? r.fechaRealizada.substring(0,10)  : '',
        kilometraje:     r.kilometraje     ?? '',
        costo:           r.costo           ?? '',
        taller:          r.taller          ?? '',
        responsable:     r.responsable     ?? '',
        observaciones:   r.observaciones   ?? '',
        estado:          r.estado          ?? 'Programado',
    }
    modoForm.value    = 'editar'
    vistaActiva.value = 'form'
    errorMsg.value    = ''
}

function cancelarForm() {
    vistaActiva.value = 'lista'
    form.value = formInicial()
}

async function guardar() {
    if (!form.value.vehiculoPlaca || !form.value.descripcion || !form.value.fechaProgramada) {
        errorMsg.value = 'Completa los campos obligatorios: placa, descripción y fecha programada.'
        return
    }
    guardando.value = true
    errorMsg.value  = ''
    exitoMsg.value  = ''
    try {
        const payload = { ...form.value }
        if (modoForm.value === 'crear') {
            await crearMantenimiento(payload)
            exitoMsg.value = 'Mantenimiento registrado correctamente.'
        } else {
            await actualizarMantenimiento(form.value.id, payload)
            exitoMsg.value = 'Mantenimiento actualizado correctamente.'
        }
        await cargar()
        vistaActiva.value = 'lista'
        form.value = formInicial()
        auto_limpiar()
    } catch (e) {
        console.error(e)
        errorMsg.value = 'Error al guardar el registro.'
    } finally {
        guardando.value = false
    }
}

function pedirEliminar(r) {
    registroAEliminar.value = r
    modalEliminar.value     = true
}

async function confirmarEliminar() {
    const r = registroAEliminar.value
    if (!r) return
    eliminando.value    = r.id
    modalEliminar.value = false
    try {
        await eliminarMantenimiento(r.id)
        exitoMsg.value = 'Registro eliminado.'
        await cargar()
        auto_limpiar()
    } catch (e) {
        errorMsg.value = 'Error al eliminar el registro.'
        auto_limpiar()
    } finally {
        eliminando.value         = null
        registroAEliminar.value  = null
    }
}

function auto_limpiar() {
    setTimeout(() => { exitoMsg.value = ''; errorMsg.value = '' }, 3500)
}

onMounted(cargar)
</script>

<template>
    <div class="mant-page">

        <div class="mant-header">
            <div>
                <h1 class="mant-title">Mantenimiento de vehículos</h1>
                <p class="mant-sub">Gestión de mantenimientos preventivos y correctivos</p>
            </div>
            <div class="mant-header-actions">
                <button class="btn-actualizar" @click="cargar">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                        <polyline points="1 4 1 10 7 10"/>
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95"/>
                    </svg>
                    Actualizar
                </button>
                <button class="btn-nuevo" @click="abrirCrear">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                        <line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>
                    </svg>
                    Nuevo mantenimiento
                </button>
            </div>
        </div>

        <div v-if="exitoMsg" class="notif notif-exito">{{ exitoMsg }}</div>
        <div v-if="errorMsg && vistaActiva === 'lista'" class="notif notif-error">{{ errorMsg }}</div>

        <template v-if="vistaActiva === 'lista'">

            <div class="resumen-row">
                <div class="resumen-card">
                    <span class="res-icon res-icon-total">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                        </svg>
                    </span>
                    <div>
                        <p class="res-num">{{ resumen.total }}</p>
                        <p class="res-label">Total registros</p>
                    </div>
                </div>
                <div class="resumen-card">
                    <span class="res-icon res-icon-programado">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                            <line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/>
                            <line x1="3" y1="10" x2="21" y2="10"/>
                        </svg>
                    </span>
                    <div>
                        <p class="res-num azul">{{ resumen.programados }}</p>
                        <p class="res-label">Programados</p>
                    </div>
                </div>
                <div class="resumen-card">
                    <span class="res-icon res-icon-proceso">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <circle cx="12" cy="12" r="10"/>
                            <polyline points="12 6 12 12 16 14"/>
                        </svg>
                    </span>
                    <div>
                        <p class="res-num naranja">{{ resumen.enProceso }}</p>
                        <p class="res-label">En proceso</p>
                    </div>
                </div>
                <div class="resumen-card">
                    <span class="res-icon res-icon-completado">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <polyline points="20 6 9 17 4 12"/>
                        </svg>
                    </span>
                    <div>
                        <p class="res-num verde">{{ resumen.completados }}</p>
                        <p class="res-label">Completados</p>
                    </div>
                </div>
                <div class="resumen-card resumen-card-costo">
                    <span class="res-icon res-icon-costo">
                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                            <line x1="12" y1="1" x2="12" y2="23"/>
                            <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                        </svg>
                    </span>
                    <div>
                        <p class="res-num">{{ formatMoney(resumen.costoTotal) }}</p>
                        <p class="res-label">Costo total</p>
                    </div>
                </div>
            </div>

            <div class="toolbar">
                <div class="search-wrap">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                        <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
                    </svg>
                    <input v-model="busqueda" type="text" placeholder="Buscar por placa, taller, tipo..." class="search-input" />
                </div>
                <select v-model="filtroEstado" class="filtro-select">
                    <option value="">Todos los estados</option>
                    <option v-for="e in ESTADOS" :key="e">{{ e }}</option>
                </select>
                <select v-model="filtroTipo" class="filtro-select">
                    <option value="">Todos los tipos</option>
                    <option v-for="t in TIPOS" :key="t">{{ t }}</option>
                </select>
            </div>

            <div class="tabla-wrap">
                <div v-if="loading" class="estado-carga">
                    <div class="spinner"></div>
                    <p>Cargando registros...</p>
                </div>

                <template v-else>
                    <div v-if="registrosFiltrados.length === 0" class="estado-vacio">
                        <svg width="44" height="44" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.2">
                            <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                        </svg>
                        <p>No se encontraron registros de mantenimiento.</p>
                        <button class="btn-nuevo btn-nuevo-sm" @click="abrirCrear">Registrar mantenimiento</button>
                    </div>

                    <table v-else class="mant-tabla">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Vehículo</th>
                                <th>Tipo</th>
                                <th>Descripción</th>
                                <th>F. Programada</th>
                                <th>F. Realizada</th>
                                <th>Taller / Responsable</th>
                                <th>Costo</th>
                                <th>Estado</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="r in registrosFiltrados" :key="r.id">
                                <td class="td-id">{{ formatNumero(r.id) }}</td>
                                <td>
                                    <span class="placa-badge">{{ r.vehiculoPlaca ?? '—' }}</span>
                                </td>
                                <td>
                                    <span class="badge" :class="tipoClase(r.tipo)">{{ r.tipo }}</span>
                                </td>
                                <td class="td-desc">{{ r.descripcion }}</td>
                                <td>{{ formatFecha(r.fechaProgramada) }}</td>
                                <td>{{ formatFecha(r.fechaRealizada) }}</td>
                                <td class="td-taller">
                                    <span>{{ r.taller ?? '—' }}</span>
                                    <span v-if="r.responsable" class="td-sub">{{ r.responsable }}</span>
                                </td>
                                <td class="td-costo">{{ r.costo ? formatMoney(r.costo) : '—' }}</td>
                                <td>
                                    <span class="badge" :class="estadoClase(r.estado)">{{ r.estado }}</span>
                                </td>
                                <td class="td-acciones">
                                    <button class="btn-accion btn-editar" @click="abrirEditar(r)" title="Editar">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
                                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
                                        </svg>
                                    </button>
                                    <button
                                        class="btn-accion btn-eliminar"
                                        @click="pedirEliminar(r)"
                                        :disabled="eliminando === r.id"
                                        title="Eliminar"
                                    >
                                        <div v-if="eliminando === r.id" class="spinner-btn-dark"></div>
                                        <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                                            <polyline points="3 6 5 6 21 6"/>
                                            <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/>
                                            <path d="M10 11v6"/><path d="M14 11v6"/>
                                            <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/>
                                        </svg>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </template>
            </div>
        </template>

        <template v-else>
            <div class="form-wrap">

                <div class="form-header">
                    <button class="btn-volver" @click="cancelarForm">
                        <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                            <polyline points="15 18 9 12 15 6"/>
                        </svg>
                        Volver a la lista
                    </button>
                    <h2 class="form-titulo">
                        {{ modoForm === 'crear' ? 'Registrar mantenimiento' : 'Editar mantenimiento' }}
                    </h2>
                </div>

                <div v-if="errorMsg" class="notif notif-error" style="margin-bottom:16px">{{ errorMsg }}</div>

                <div class="form-grid">

                    <div class="form-section">
                        <p class="form-section-title">
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                                <rect x="1" y="3" width="15" height="13" rx="2"/>
                                <path d="M16 8h4l3 3v5h-7V8z"/>
                                <circle cx="5.5" cy="18.5" r="2.5"/>
                                <circle cx="18.5" cy="18.5" r="2.5"/>
                            </svg>
                            Vehículo
                        </p>
                        <div class="form-row-2">
                            <div class="form-group">
                                <label class="form-label">Placa <span class="req">*</span></label>
                                <input v-model="form.vehiculoPlaca" type="text" class="form-input" placeholder="Ej. A123BC" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">ID Vehículo</label>
                                <input v-model="form.vehiculoId" type="text" class="form-input" placeholder="ID en el sistema" />
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <p class="form-section-title">
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                                <path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>
                            </svg>
                            Detalles del mantenimiento
                        </p>
                        <div class="form-row-2">
                            <div class="form-group">
                                <label class="form-label">Tipo <span class="req">*</span></label>
                                <select v-model="form.tipo" class="form-select">
                                    <option v-for="t in TIPOS" :key="t">{{ t }}</option>
                                </select>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Estado</label>
                                <select v-model="form.estado" class="form-select">
                                    <option v-for="e in ESTADOS" :key="e">{{ e }}</option>
                                </select>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Descripción <span class="req">*</span></label>
                            <textarea v-model="form.descripcion" class="form-textarea" rows="3" placeholder="Describe el trabajo a realizar o realizado..." />
                        </div>
                    </div>

                    <div class="form-section">
                        <p class="form-section-title">
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                                <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
                                <line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/>
                                <line x1="3" y1="10" x2="21" y2="10"/>
                            </svg>
                            Fechas y kilometraje
                        </p>
                        <div class="form-row-3">
                            <div class="form-group">
                                <label class="form-label">Fecha programada <span class="req">*</span></label>
                                <input v-model="form.fechaProgramada" type="date" class="form-input" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Fecha realizada</label>
                                <input v-model="form.fechaRealizada" type="date" class="form-input" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Kilometraje actual</label>
                                <input v-model="form.kilometraje" type="number" class="form-input" placeholder="km" min="0" />
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <p class="form-section-title">
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                                <line x1="12" y1="1" x2="12" y2="23"/>
                                <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
                            </svg>
                            Taller y costo
                        </p>
                        <div class="form-row-3">
                            <div class="form-group">
                                <label class="form-label">Taller / Proveedor</label>
                                <input v-model="form.taller" type="text" class="form-input" placeholder="Nombre del taller" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Responsable</label>
                                <input v-model="form.responsable" type="text" class="form-input" placeholder="Nombre del responsable" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Costo (DOP)</label>
                                <input v-model="form.costo" type="number" class="form-input" placeholder="0.00" min="0" step="0.01" />
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <p class="form-section-title">
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8">
                                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                                <polyline points="14 2 14 8 20 8"/>
                                <line x1="16" y1="13" x2="8" y2="13"/><line x1="16" y1="17" x2="8" y2="17"/>
                            </svg>
                            Observaciones
                        </p>
                        <div class="form-group">
                            <textarea v-model="form.observaciones" class="form-textarea" rows="3" placeholder="Notas adicionales, piezas cambiadas, próximo mantenimiento..." />
                        </div>
                    </div>

                </div>

                <div class="form-acciones">
                    <button class="btn-cancelar" @click="cancelarForm">Cancelar</button>
                    <button class="btn-guardar" :disabled="guardando" @click="guardar">
                        <div v-if="guardando" class="spinner-btn"></div>
                        <svg v-else width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                            <polyline points="20 6 9 17 4 12"/>
                        </svg>
                        {{ guardando ? 'Guardando...' : (modoForm === 'crear' ? 'Registrar mantenimiento' : 'Guardar cambios') }}
                    </button>
                </div>
            </div>
        </template>

        <div v-if="modalEliminar" class="modal-overlay" @click.self="modalEliminar = false">
            <div class="modal-box">
                <div class="modal-icon">
                    <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#dc2626" stroke-width="2">
                        <polyline points="3 6 5 6 21 6"/>
                        <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/>
                        <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/>
                    </svg>
                </div>
                <h3 class="modal-titulo">¿Eliminar registro?</h3>
                <p class="modal-desc">
                    Estás a punto de eliminar el mantenimiento
                    <strong>{{ formatNumero(registroAEliminar?.id) }}</strong>
                    del vehículo <strong>{{ registroAEliminar?.vehiculoPlaca }}</strong>.
                    Esta acción no se puede deshacer.
                </p>
                <div class="modal-acciones">
                    <button class="btn-cancelar" @click="modalEliminar = false">Cancelar</button>
                    <button class="btn-eliminar-confirmar" @click="confirmarEliminar">Sí, eliminar</button>
                </div>
            </div>
        </div>

    </div>
</template>

<style scoped>

.mant-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.mant-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
}

.mant-title {
    font-size: 1.45rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0 0 2px;
}

.mant-sub {
    font-size: .8rem;
    color: #9ca3af;
    margin: 0;
}

.mant-header-actions { display: flex; gap: 10px; }

.btn-actualizar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #fff;
    border: 1.5px solid #d1d5db;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    transition: all .15s;
}
.btn-actualizar:hover { background: #f3f4f6; border-color: #9ca3af; }

.btn-nuevo {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 20px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}
.btn-nuevo:hover { background: #14532d; }

.btn-nuevo-sm {
    margin-top: 8px;
    padding: 8px 18px;
    font-size: .82rem;
}

.notif {
    padding: 12px 18px;
    border-radius: 10px;
    font-size: .875rem;
    font-weight: 500;
    margin-bottom: 16px;
}
.notif-exito { background: #d1fae5; color: #065f46; border: 1px solid #6ee7b7; }
.notif-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

.resumen-row {
    display: flex;
    gap: 12px;
    margin-bottom: 16px;
    flex-wrap: wrap;
}

.resumen-card {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    padding: 14px 18px;
    display: flex;
    align-items: center;
    gap: 12px;
    flex: 1;
    min-width: 150px;
}

.resumen-card-costo { flex: 1.5; }

.res-icon {
    width: 38px;
    height: 38px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.res-icon-total      { background: #f3f4f6; color: #374151; }
.res-icon-programado { background: #dbeafe; color: #1e40af; }
.res-icon-proceso    { background: #fef3c7; color: #92400e; }
.res-icon-completado { background: #d1fae5; color: #065f46; }
.res-icon-costo      { background: #ede9fe; color: #6d28d9; }

.res-num {
    font-size: 1.4rem;
    font-weight: 800;
    color: #111827;
    margin: 0 0 1px;
    line-height: 1;
}

.res-label {
    font-size: .72rem;
    color: #6b7280;
    margin: 0;
    font-weight: 500;
}

.azul    { color: #2563eb; }
.naranja { color: #d97706; }
.verde   { color: #16a34a; }

.toolbar {
    display: flex;
    gap: 10px;
    margin-bottom: 14px;
    flex-wrap: wrap;
}

.search-wrap {
    display: flex;
    align-items: center;
    gap: 8px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    padding: 0 12px;
    flex: 1;
    min-width: 220px;
}

.search-input {
    flex: 1;
    border: none;
    outline: none;
    font-size: .85rem;
    color: #111827;
    padding: 9px 0;
    background: transparent;
    font-family: inherit;
}
.search-input::placeholder { color: #9ca3af; }

.filtro-select {
    padding: 9px 12px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .85rem;
    color: #374151;
    font-family: inherit;
    cursor: pointer;
    outline: none;
    transition: border-color .15s;
}
.filtro-select:focus { border-color: #1a3a2a; }

.tabla-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

.mant-tabla {
    width: 100%;
    border-collapse: collapse;
    font-size: .875rem;
}

.mant-tabla th {
    padding: 13px 14px;
    text-align: left;
    font-size: .7rem;
    font-weight: 600;
    color: #9ca3af;
    letter-spacing: .05em;
    border-bottom: 1.5px solid #f3f4f6;
    white-space: nowrap;
}

.mant-tabla td {
    padding: 12px 14px;
    color: #374151;
    border-bottom: 1px solid #f9fafb;
    vertical-align: middle;
}

.mant-tabla tbody tr:last-child td { border-bottom: none; }
.mant-tabla tbody tr:hover { background: #fafafa; }

.td-id { font-weight: 700; color: #111827; }

.placa-badge {
    background: #111827;
    color: #fff;
    font-size: .72rem;
    font-weight: 700;
    padding: 3px 9px;
    border-radius: 6px;
    letter-spacing: .05em;
}

.td-desc {
    max-width: 200px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    color: #374151;
}

.td-taller {
    display: flex;
    flex-direction: column;
    gap: 1px;
}
.td-sub {
    font-size: .72rem;
    color: #9ca3af;
}

.td-costo {
    font-weight: 600;
    color: #111827;
    white-space: nowrap;
}

.td-acciones {
    display: flex;
    gap: 6px;
    align-items: center;
}

.btn-accion {
    width: 30px;
    height: 30px;
    display: flex;
    align-items: center;
    justify-content: center;
    border: 1.5px solid transparent;
    border-radius: 7px;
    cursor: pointer;
    transition: all .15s;
}

.btn-editar {
    background: #f0fdf4;
    border-color: #bbf7d0;
    color: #15803d;
}
.btn-editar:hover { background: #dcfce7; }

.btn-eliminar {
    background: #fff1f2;
    border-color: #fecdd3;
    color: #dc2626;
}
.btn-eliminar:hover:not(:disabled) { background: #fee2e2; }
.btn-eliminar:disabled { opacity: .5; cursor: default; }

.badge {
    display: inline-block;
    padding: 3px 9px;
    border-radius: 20px;
    font-size: .71rem;
    font-weight: 700;
    white-space: nowrap;
}

.badge-programado  { background: #dbeafe; color: #1e40af; }
.badge-en-proceso  { background: #fef3c7; color: #92400e; }
.badge-completado  { background: #d1fae5; color: #065f46; }
.badge-cancelado   { background: #fee2e2; color: #991b1b; }

.badge-preventivo  { background: #ede9fe; color: #6d28d9; }
.badge-correctivo  { background: #fef3c7; color: #92400e; }
.badge-emergencia  { background: #fee2e2; color: #991b1b; }

.estado-carga {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 60px 0;
    color: #6b7280;
}

.estado-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
    padding: 56px 0;
    color: #9ca3af;
    font-size: .9rem;
    text-align: center;
}

.form-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0,0,0,.07);
    overflow: hidden;
}

.form-header {
    display: flex;
    align-items: center;
    gap: 14px;
    padding: 18px 24px;
    border-bottom: 1px solid #f3f4f6;
}

.btn-volver {
    display: inline-flex;
    align-items: center;
    gap: 5px;
    padding: 6px 14px;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .82rem;
    font-weight: 600;
    color: #374151;
    cursor: pointer;
    transition: all .15s;
    flex-shrink: 0;
}
.btn-volver:hover { background: #f3f4f6; }

.form-titulo {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.form-grid {
    padding: 20px 24px;
    display: flex;
    flex-direction: column;
    gap: 0;
}

.form-section {
    padding: 18px 0;
    border-bottom: 1px solid #f3f4f6;
}
.form-section:last-child { border-bottom: none; }

.form-section-title {
    display: flex;
    align-items: center;
    gap: 7px;
    font-size: .72rem;
    font-weight: 700;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: .07em;
    margin: 0 0 14px;
    color: #6b7280;
}

.form-row-2 {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 14px;
}

.form-row-3 {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 14px;
}

.form-group {
    display: flex;
    flex-direction: column;
    gap: 5px;
    margin-bottom: 12px;
}
.form-group:last-child { margin-bottom: 0; }

.form-label {
    font-size: .78rem;
    font-weight: 600;
    color: #374151;
}

.req { color: #dc2626; }

.form-input,
.form-select,
.form-textarea {
    padding: 9px 12px;
    background: #f9fafb;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .875rem;
    color: #111827;
    font-family: inherit;
    outline: none;
    transition: border-color .15s;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
    border-color: #1a3a2a;
    background: #fff;
}

.form-textarea { resize: vertical; min-height: 72px; }

.form-acciones {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding: 18px 24px;
    border-top: 1px solid #f3f4f6;
    background: #fafafa;
}

.btn-cancelar {
    padding: 10px 22px;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s;
}
.btn-cancelar:hover { background: #f3f4f6; }

.btn-guardar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 10px 24px;
    background: #1a3a2a;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}
.btn-guardar:hover:not(:disabled) { background: #14532d; }
.btn-guardar:disabled { opacity: .5; cursor: default; }

.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,.35);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 999;
}

.modal-box {
    background: #fff;
    border-radius: 16px;
    padding: 28px 28px 24px;
    max-width: 400px;
    width: 90%;
    box-shadow: 0 8px 30px rgba(0,0,0,.15);
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    gap: 10px;
}

.modal-icon {
    width: 54px;
    height: 54px;
    background: #fee2e2;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 4px;
}

.modal-titulo {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.modal-desc {
    font-size: .875rem;
    color: #6b7280;
    margin: 0;
    line-height: 1.5;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    margin-top: 8px;
    width: 100%;
}

.modal-acciones .btn-cancelar { flex: 1; }

.btn-eliminar-confirmar {
    flex: 1;
    padding: 10px 0;
    background: #dc2626;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}
.btn-eliminar-confirmar:hover { background: #b91c1c; }

.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn {
    width: 15px; height: 15px;
    border: 2px solid rgba(255,255,255,.4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn-dark {
    width: 14px; height: 14px;
    border: 2px solid #fecdd3;
    border-top-color: #dc2626;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

@media (max-width: 1000px) {
    .form-row-3 { grid-template-columns: 1fr 1fr; }
    .resumen-row { gap: 10px; }
}

@media (max-width: 700px) {
    .mant-page     { padding: 16px; }
    .mant-header   { flex-direction: column; align-items: flex-start; gap: 12px; }
    .form-row-2,
    .form-row-3    { grid-template-columns: 1fr; }
    .toolbar       { flex-direction: column; }
    .resumen-card  { min-width: 120px; }
    .mant-tabla    { font-size: .78rem; }
    .mant-tabla th,
    .mant-tabla td { padding: 10px 9px; }
}
</style>
