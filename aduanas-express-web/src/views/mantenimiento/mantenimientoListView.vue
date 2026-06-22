<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useMantenimientos, TIPOS, ESTADOS } from './composables/useMantenimientos'
import { usePermisos } from '@/composables/usePermisos'
import MantenimientoEliminarModal from './MantenimientoEliminarModal.vue'
import MantenimientoFinalizarModal from './MantenimientoFinalizarModal.vue'
import MantenimientoHistorialModal from './MantenimientoHistorialModal.vue'

const router = useRouter()
const { puede } = usePermisos()

const {
    registros, loading, eliminando, errorMsg, exitoMsg,
    busqueda, filtroEstado, filtroTipo,
    registrosFiltrados, resumen,
    cargar, guardar, eliminar, avisar,
} = useMantenimientos()

const mostrarEliminarModal = ref(false)
const registroAEliminar = ref(null)

const mostrarFinalizarModal = ref(false)
const registroAFinalizar = ref(null)
const finalizando = ref(false)

const mostrarHistorialModal = ref(false)
const historialVehiculoId = ref(null)
const historialVehiculoPlaca = ref('')

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
        'Cancelado': 'badge-cancelado',
    }[estado] ?? ''
}

function tipoClase(tipo) {
    return {
        'Preventivo': 'badge-preventivo',
        'Correctivo': 'badge-correctivo',
        'Emergencia': 'badge-emergencia',
    }[tipo] ?? ''
}

function nuevo() {
    router.push({ name: 'mantenimientoNuevo' })
}

function editar(r) {
    router.push({ name: 'editarMantenimiento', params: { id: r.id } })
}

function pedirEliminar(r) {
    registroAEliminar.value = r
    mostrarEliminarModal.value = true
}

async function confirmarEliminar(r) {
    if (!r) return
    try {
        await eliminar(r.id)
    } catch (e) {
    } finally {
        registroAEliminar.value = null
    }
}

function pedirFinalizar(r) {
    registroAFinalizar.value = r
    mostrarFinalizarModal.value = true
}

async function confirmarFinalizar(datos) {
    const r = registroAFinalizar.value
    if (!r) return

    let observacionesFinal = r.observaciones || ''
    if (datos.reporteFinal) {
        const nota = `[Cierre ${datos.fechaRealizada}] ${datos.reporteFinal}`
        observacionesFinal = observacionesFinal ? `${observacionesFinal}\n\n${nota}` : nota
    }

    const payloadFinal = {
        vehiculoId: r.vehiculoId,
        tipo: r.tipo,
        descripcion: r.descripcion,
        estado: 'Completado',
        fechaProgramada: r.fechaProgramada ? r.fechaProgramada.substring(0, 10) : '',
        fechaRealizada: datos.fechaRealizada,
        kilometraje: datos.kilometraje !== null ? datos.kilometraje : (r.kilometraje ?? null),
        costo: datos.costo !== null ? datos.costo : (r.costo ?? 0),
        taller: r.taller,
        responsable: r.responsable,
        observaciones: observacionesFinal,
    }

    finalizando.value = true
    try {
        await guardar('editar', payloadFinal, r.id)
        avisar('Mantenimiento finalizado correctamente.')
        mostrarFinalizarModal.value = false
    } catch (e) {
        console.error(e)
        avisar('Error al finalizar el mantenimiento.', 'error')
    } finally {
        finalizando.value = false
        registroAFinalizar.value = null
    }
}

function abrirHistorial(r) {
    historialVehiculoId.value = r.vehiculoId
    historialVehiculoPlaca.value = r.vehiculoPlaca
    mostrarHistorialModal.value = true
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
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.2">
                        <polyline points="1 4 1 10 7 10" />
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95" />
                    </svg>
                    Actualizar
                </button>
                <button v-if="puede.gestionarMantenimiento" class="btn-nuevo" @click="nuevo">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <line x1="12" y1="5" x2="12" y2="19" />
                        <line x1="5" y1="12" x2="19" y2="12" />
                    </svg>
                    Nuevo mantenimiento
                </button>
            </div>
        </div>

        <div v-if="exitoMsg" class="notif notif-exito">{{ exitoMsg }}</div>
        <div v-if="errorMsg" class="notif notif-error">{{ errorMsg }}</div>

        <div class="resumen-row">
            <div class="resumen-card">
                <span class="res-icon res-icon-total">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="1.8">
                        <path
                            d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z" />
                    </svg>
                </span>
                <div>
                    <p class="res-num">{{ resumen.total }}</p>
                    <p class="res-label">Total registros</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="res-icon res-icon-programado">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="1.8">
                        <rect x="3" y="4" width="18" height="18" rx="2" ry="2" />
                        <line x1="16" y1="2" x2="16" y2="6" />
                        <line x1="8" y1="2" x2="8" y2="6" />
                        <line x1="3" y1="10" x2="21" y2="10" />
                    </svg>
                </span>
                <div>
                    <p class="res-num azul">{{ resumen.programados }}</p>
                    <p class="res-label">Programados</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="res-icon res-icon-proceso">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="1.8">
                        <circle cx="12" cy="12" r="10" />
                        <polyline points="12 6 12 12 16 14" />
                    </svg>
                </span>
                <div>
                    <p class="res-num naranja">{{ resumen.enProceso }}</p>
                    <p class="res-label">En proceso</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="res-icon res-icon-completado">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="1.8">
                        <polyline points="20 6 9 17 4 12" />
                    </svg>
                </span>
                <div>
                    <p class="res-num verde">{{ resumen.completados }}</p>
                    <p class="res-label">Completados</p>
                </div>
            </div>
            <div class="resumen-card resumen-card-costo">
                <span class="res-icon res-icon-costo">
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="1.8">
                        <line x1="12" y1="1" x2="12" y2="23" />
                        <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
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
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busqueda" type="text" placeholder="Buscar por placa, taller, tipo..."
                    class="search-input" />
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
                        <path
                            d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z" />
                    </svg>
                    <p>No se encontraron registros de mantenimiento.</p>
                    <button v-if="puede.gestionarMantenimiento" class="btn-nuevo btn-nuevo-sm" @click="nuevo">Registrar
                        mantenimiento</button>
                </div>

                <div v-else class="tabla-scroll">
                    <table class="mant-tabla">
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
                                <th v-if="puede.gestionarMantenimiento" class="th-acciones"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="r in registrosFiltrados" :key="r.id">
                                <td class="td-id">{{ formatNumero(r.id) }}</td>
                                <td data-label="Vehículo">
                                    <button class="placa-badge placa-badge-btn" @click="abrirHistorial(r)"
                                        title="Ver historial de este vehículo">
                                        {{ r.vehiculoPlaca ?? '—' }}
                                    </button>
                                </td>
                                <td data-label="Tipo">
                                    <span class="badge" :class="tipoClase(r.tipo)">{{ r.tipo }}</span>
                                </td>
                                <td class="td-desc" data-label="Descripción">{{ r.descripcion }}</td>
                                <td data-label="F. Programada">{{ formatFecha(r.fechaProgramada) }}</td>
                                <td data-label="F. Realizada">{{ formatFecha(r.fechaRealizada) }}</td>
                                <td class="td-taller" data-label="Taller / Responsable">
                                    <span>{{ r.taller ?? '—' }}</span>
                                    <span v-if="r.responsable" class="td-sub">{{ r.responsable }}</span>
                                </td>
                                <td class="td-costo" data-label="Costo">{{ r.costo ? formatMoney(r.costo) : '—' }}</td>
                                <td data-label="Estado">
                                    <span class="badge" :class="estadoClase(r.estado)">{{ r.estado }}</span>
                                </td>
                                <td v-if="puede.gestionarMantenimiento" class="td-acciones">
                                    <button v-if="r.estado === 'Programado' || r.estado === 'En proceso'"
                                        class="btn-accion btn-finalizar" @click="pedirFinalizar(r)"
                                        title="Finalizar mantenimiento">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2.2">
                                            <polyline points="20 6 9 17 4 12" />
                                        </svg>
                                    </button>
                                    <button class="btn-accion btn-editar" @click="editar(r)" title="Editar">
                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2">
                                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                                        </svg>
                                    </button>
                                    <button class="btn-accion btn-eliminar" @click="pedirEliminar(r)"
                                        :disabled="eliminando === r.id" title="Eliminar">
                                        <div v-if="eliminando === r.id" class="spinner-btn-dark"></div>
                                        <svg v-else width="14" height="14" viewBox="0 0 24 24" fill="none"
                                            stroke="currentColor" stroke-width="2">
                                            <polyline points="3 6 5 6 21 6" />
                                            <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                                            <path d="M10 11v6" />
                                            <path d="M14 11v6" />
                                            <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
                                        </svg>
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </template>
        </div>

        <MantenimientoEliminarModal v-model="mostrarEliminarModal" :registro="registroAEliminar"
            @confirmar="confirmarEliminar" />

        <MantenimientoFinalizarModal v-model="mostrarFinalizarModal" :registro="registroAFinalizar"
            :guardando="finalizando" @confirmar="confirmarFinalizar" />

        <MantenimientoHistorialModal v-model="mostrarHistorialModal" :vehiculo-id="historialVehiculoId"
            :vehiculo-placa="historialVehiculoPlaca" :registros="registros" />

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

.mant-header-actions {
    display: flex;
    gap: 10px;
}

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

.btn-actualizar:hover {
    background: #f3f4f6;
    border-color: #9ca3af;
}

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

.btn-nuevo:hover {
    background: #14532d;
}

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

.notif-exito {
    background: #d1fae5;
    color: #065f46;
    border: 1px solid #6ee7b7;
}

.notif-error {
    background: #fee2e2;
    color: #991b1b;
    border: 1px solid #fca5a5;
}

.resumen-row {
    display: flex;
    gap: 12px;
    margin-bottom: 16px;
    flex-wrap: wrap;
}

.resumen-card {
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    padding: 14px 18px;
    display: flex;
    align-items: center;
    gap: 12px;
    flex: 1;
    min-width: 150px;
}

.resumen-card-costo {
    flex: 1.5;
}

.res-icon {
    width: 38px;
    height: 38px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.res-icon-total {
    background: #f3f4f6;
    color: #374151;
}

.res-icon-programado {
    background: #dbeafe;
    color: #1e40af;
}

.res-icon-proceso {
    background: #fef3c7;
    color: #92400e;
}

.res-icon-completado {
    background: #d1fae5;
    color: #065f46;
}

.res-icon-costo {
    background: #ede9fe;
    color: #6d28d9;
}

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

.azul {
    color: #2563eb;
}

.naranja {
    color: #d97706;
}

.verde {
    color: #16a34a;
}

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

.search-input::placeholder {
    color: #9ca3af;
}

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

.filtro-select:focus {
    border-color: #1a3a2a;
}

.tabla-wrap {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    overflow: hidden;
}

.tabla-scroll {
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
}

.mant-tabla {
    width: 100%;
    min-width: 920px;
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

.mant-tabla tbody tr:last-child td {
    border-bottom: none;
}

.mant-tabla tbody tr:hover {
    background: #fafafa;
}

.td-id {
    font-weight: 700;
    color: #111827;
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

.placa-badge-btn {
    border: none;
    cursor: pointer;
    transition: background .15s;
    font-family: inherit;
}

.placa-badge-btn:hover {
    background: #374151;
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

.th-acciones {
    width: 110px;
}

.td-acciones {
    display: flex;
    gap: 6px;
    align-items: center;
    flex-wrap: nowrap;
    white-space: nowrap;
}

.btn-accion {
    width: 30px;
    height: 30px;
    flex-shrink: 0;
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

.btn-editar:hover {
    background: #dcfce7;
}

.btn-finalizar {
    background: #f0fdf4;
    border-color: #bbf7d0;
    color: #16a34a;
}

.btn-finalizar:hover {
    background: #dcfce7;
}

.btn-eliminar {
    background: #fff1f2;
    border-color: #fecdd3;
    color: #dc2626;
}

.btn-eliminar:hover:not(:disabled) {
    background: #fee2e2;
}

.btn-eliminar:disabled {
    opacity: .5;
    cursor: default;
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

.badge-preventivo {
    background: #ede9fe;
    color: #6d28d9;
}

.badge-correctivo {
    background: #fef3c7;
    color: #92400e;
}

.badge-emergencia {
    background: #fee2e2;
    color: #991b1b;
}

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

.spinner {
    width: 32px;
    height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

.spinner-btn-dark {
    width: 14px;
    height: 14px;
    border: 2px solid #fecdd3;
    border-top-color: #dc2626;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

@media (max-width: 1000px) {
    .resumen-row {
        gap: 10px;
    }
}

@media (max-width: 760px) {
    .mant-page {
        padding: 16px;
    }

    .mant-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
    }

    .toolbar {
        flex-direction: column;
    }

    .resumen-card {
        min-width: 120px;
    }

    /* La tabla se convierte en una lista de tarjetas: sin scroll horizontal,
       cada fila se apila y cada celda muestra su etiqueta. */
    .tabla-scroll {
        overflow-x: visible;
    }

    .tabla-wrap {
        background: transparent;
        box-shadow: none;
        overflow: visible;
    }

    .mant-tabla {
        display: block;
        width: 100%;
        min-width: 0;
    }

    .mant-tabla thead {
        display: none;
    }

    .mant-tabla tbody {
        display: block;
    }

    .mant-tabla tr {
        display: block;
        background: #fff;
        border-radius: 12px;
        box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
        padding: 12px 14px;
        margin-bottom: 10px;
    }

    .mant-tabla tbody tr:last-child {
        margin-bottom: 0;
    }

    .mant-tabla td {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
        padding: 7px 0;
        border-bottom: none;
        text-align: right;
    }

    .mant-tabla td:not(.td-id):not(.td-acciones)::before {
        content: attr(data-label);
        font-size: .68rem;
        font-weight: 700;
        color: #9ca3af;
        text-transform: uppercase;
        letter-spacing: .04em;
        text-align: left;
        flex-shrink: 0;
    }

    .mant-tabla td.td-id {
        font-size: 1rem;
        justify-content: flex-start;
        padding-bottom: 8px;
        margin-bottom: 4px;
        border-bottom: 1px solid #f3f4f6;
    }

    .mant-tabla td.td-desc {
        max-width: none;
        white-space: normal;
        overflow: visible;
        text-overflow: clip;
    }

    .mant-tabla td.td-taller {
        align-items: flex-end;
        text-align: right;
    }

    .mant-tabla td.td-acciones {
        justify-content: flex-end;
        padding-top: 9px;
        margin-top: 4px;
        border-top: 1px solid #f3f4f6;
    }
}
</style>