<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/authStore'
import {
    verConductores,
    eliminarConductor
} from '../../services/conductorService'
import ConductorVerModal from './ConductorVerModal.vue'
import ConductorEliminarModal from './ConductorEliminarModal.vue'

const authStore = useAuthStore()
const router = useRouter()
const mostrarVer = ref(false)
const conductorVerId = ref(null)
const conductores = ref([])
const loading = ref(false)
const error = ref('')
const busqueda = ref('')
const filtroEstado = ref('')
const filtroTipo = ref('')
const mostrarConfirmacion = ref(false)
const conductoresAEliminar = ref(null)

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

const tiposUnicos = computed(() => [
    ...new Set(conductores.value.map((v) => v.tipo).filter(Boolean)),
])

const resumen = computed(() => {
    const total = conductores.value.length
    const disponibles = conductores.value.filter(v => v.estado === 0).length
    const enViaje = conductores.value.filter(v => v.estado === 1).length
    const suspendido = conductores.value.filter(v => v.estado === 2).length
    const inactivo = conductores.value.filter(v => v.estado === 3).length
    return {
        total,
        disponibles,
        enViaje,
        suspendido,
        inactivo
    }

})

const conductoresFiltrados = computed(() => {
    return conductores.value.filter((v) => {
        const q = busqueda.value.toLowerCase()
        const coincideBusqueda =
            !q ||
            v.matricula?.toLowerCase().includes(q) ||
            v.numeroLicencia?.toLowerCase().includes(q) ||
            v.nombre?.toLowerCase().includes(q)

        const coincideEstado =
            filtroEstado.value === '' ||
            String(v.estado) === filtroEstado.value

        const coincideTipo =
            filtroTipo.value === '' || v.tipo === filtroTipo.value

        return coincideBusqueda && coincideEstado && coincideTipo
    })
})

async function cargarConductores() {
    loading.value = true
    error.value = ''

    try {
        const res = await verConductores()
        conductores.value = res.data
    } catch (e) {
        error.value = 'No se pudieron cargar los conductores.'
    } finally {
        loading.value = false
    }
}
function irANuevo() {
    router.push('/conductores/nuevo')
}

function verConductor(id) {
    conductorVerId.value = id
    mostrarVer.value = true
}
function editarConductor(id) {
    router.push(`/conductores/${id}/editar`)
}

function confirmarEliminar(conductor) {
    conductoresAEliminar.value = conductor
    mostrarConfirmacion.value = true
}

async function ejecutarEliminar() {
    if (!conductoresAEliminar.value) return
    try {
        await eliminarConductor(conductoresAEliminar.value.id)
        conductores.value = conductores.value.filter(
            (v) => v.id !== conductoresAEliminar.value.id
        )
    } catch {
        error.value = 'Error al eliminar el conductores.'
    } finally {
        mostrarConfirmacion.value = false
        conductoresAEliminar.value = null
    }
}

function cancelarEliminar() {
    mostrarConfirmacion.value = false
    conductoresAEliminar.value = null
}
function exportar() {
    const headers = ['Nombre', 'Apellido', 'Cedula', 'NumeroLicencia', 'TipoLicencia', 'FechaVencLicencia', 'Telefono', 'Direccion', 'Supervisor', 'Estado']
    const filas = conductores.value.map((v) => [
        v.nombre, v.apellido, v.cedula, v.numeroLicencia, v.tipoLicencia, v.fechaVencLicencia,
        v.telefono, v.direccion, v.supervisor, v.estado, estadoLabel(v.estado)
    ])
    const csv = [headers, ...filas].map((f) => f.join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = 'conductores.csv'
    a.click()
    URL.revokeObjectURL(url)
}

function formatFecha(fecha) {
    if (!fecha || fecha.startsWith('0001')) return '—'

    const parte = fecha.split('T')[0]
    const [anio, mes, dia] = parte.split('-')
    return `${dia}/${mes}/${anio}`
}

onMounted(cargarConductores)
</script>

<template>
    <div class="cond-page">

        <div class="cond-header">
            <h1 class="cond-title">Conductores</h1>
            <div class="cond-header-actions">
                <button class="btn-exportar" @click="exportar">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                        <polyline points="7 10 12 15 17 10" />
                        <line x1="12" y1="15" x2="12" y2="3" />
                    </svg>
                    Exportar
                </button>
                <button class="btn-nuevo" @click="irANuevo">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <line x1="12" y1="5" x2="12" y2="19" />
                        <line x1="5" y1="12" x2="19" y2="12" />
                    </svg>
                    Nuevo Conductores
                </button>
            </div>
        </div>

        <div class="cond-resumen">
            <div class="resumen-card">

                <span class="resumen-dot dot-total"></span>
                <div>
                    <p class="resumen-num">{{ resumen.total }}</p>
                    <p class="resumen-label">Total</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-disponible"></span>
                <div>
                    <p class="resumen-num">{{ resumen.disponibles }}</p>
                    <p class="resumen-label">Disponibles</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-viaje"></span>
                <div>
                    <p class="resumen-num">{{ resumen.enViaje }}</p>
                    <p class="resumen-label">En Viaje</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-mant"></span>
                <div>
                    <p class="resumen-num">{{ resumen.suspendido }}</p>
                    <p class="resumen-label">Suspendidos</p>
                </div>
            </div>
            <div class="resumen-card">
                <span class="resumen-dot dot-mant"></span>
                <div>
                    <p class="resumen-num">{{ resumen.inactivo }}</p>
                    <p class="resumen-label">Inactivos</p>
                </div>
            </div>
        </div>

        <div class="cond-filtros">
            <div class="filtro-search">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busqueda" type="text" placeholder="Buscar por nombre, licencia o cedula..."
                    class="filtro-input" />
            </div>

            <select v-model="filtroEstado" class="filtro-select">
                <option value="">Todos los estados</option>
                <option v-for="e in estadosConductores" :key="e.value" :value="String(e.value)">
                    {{ e.label }}
                </option>
            </select>

            <select v-model="filtroTipo" class="filtro-select">
                <option value="">Todos los tipos</option>
                <option v-for="t in tiposUnicos" :key="t" :value="t">{{ t }}</option>
            </select>
        </div>

        <div v-if="loading" class="cond-estado">
            <div class="spinner"></div>
            <p>Cargando conductores....</p>
        </div>

        <div v-else-if="error" class="cond-error">
            <p>{{ error }}</p>
            <button class="btn-reintentar" @click="cargarConductores">Reintentar</button>
        </div>

        <div v-else-if="conductoresFiltrados.length > 0" class="cond-grid">
            <div v-for="v in conductoresFiltrados" :key="v.id" class="cond-card">

                <div class="card-top">

                    <span class="card-cedula">{{ v.nombre }}</span>
                    <span class="badge" :class="estadoBadgeClase[v.estado]">
                        {{ estadoLabel(v.estado) }}
                    </span>
                </div>

                <p class="card-subtitulo">{{ v.tipo }} · {{ v.capacidad }} pasajeros</p>

                <div class="card-divider"></div>

                <div class="card-detalles">
                    <div class="detalle-fila">
                        <span class="detalle-label">Licencia</span>
                        <span class="detalle-valor">{{ v.tipoLicencia ?? '—' }}</span>
                    </div>
                    <div class="detalle-fila">
                        <span class="detalle-label">Vence licencia</span>
                        <span class="detalle-valor">{{ formatFecha(v.fechaVencLicencia) }}</span>

                    </div>
                    <div class="detalle-fila">
                        <span class="detalle-label">Teléfono</span>
                        <span class="detalle-valor">{{ v.telefono }}</span>
                    </div>
                </div>

                <div class="card-acciones">
                    <button class="btn-accion btn-ver" @click="verConductor(v.id)">Ver</button>
                    <button class="btn-accion btn-editar" @click="editarConductor(v.id)">Editar</button>
                    <button class="btn-accion btn-eliminar" @click="confirmarEliminar(v)">Eliminar</button>
                </div>
            </div>
        </div>

        <div v-else class="cond-vacio">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.5">
                <rect x="1" y="3" width="15" height="13" rx="2" />
                <path d="M16 8h4l3 3v5h-7V8z" />
                <circle cx="5.5" cy="18.5" r="2.5" />
                <circle cx="18.5" cy="18.5" r="2.5" />
            </svg>
            <p>No se encontraron conductores</p>
            <span>Prueba ajustando los filtros o agrega un nuevo conductor.</span>
        </div>

        <ConductorVerModal v-model="mostrarVer" :conductor-id="conductorVerId" />

        <ConductorEliminarModal v-model="mostrarConfirmacion" :conductor="conductoresAEliminar"
            @eliminado="(id) => conductores = conductores.filter(c => c.id !== id)" />

    </div>
</template>

<style scoped>

.cond-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.cond-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 28px;
}

.cond-title {
    font-size: 1.75rem;
    font-weight: 700;
    color: #111827;
    letter-spacing: -0.02em;
    margin: 0;
}

.cond-header-actions {
    display: flex;
    gap: 12px;
}

.btn-exportar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #fff;
    border: 1.5px solid #d1d5db;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: border-color 0.15s, background 0.15s;
}

.btn-exportar:hover {
    border-color: #9ca3af;
    background: #f9fafb;
}

.btn-nuevo {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    transition: background 0.15s;
}

.btn-nuevo:hover {
    background: #14532d;
}

.cond-resumen {
    display: grid;
    grid-template-columns: repeat(5, 1fr);
    gap: 16px;
    margin-bottom: 24px;
}

.resumen-card {
    background: #fff;
    border-radius: 12px;
    padding: 18px 20px;
    display: flex;
    align-items: center;
    gap: 14px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, .06);
}

.resumen-dot {
    width: 14px;
    height: 14px;
    border-radius: 4px;
    flex-shrink: 0;
}

.dot-total {
    background: #d1d5db;
}

.dot-disponible {
    background: #bbf7d0;
}

.dot-viaje {
    background: #bfdbfe;
}

.dot-mant {
    background: #fde68a;
}

.resumen-num {
    font-size: 1.5rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
    line-height: 1;
}

.resumen-label {
    font-size: 0.78rem;
    color: #6b7280;
    margin: 4px 0 0;
}

.cond-filtros {
    display: flex;
    gap: 12px;
    margin-bottom: 24px;
}

.filtro-search {
    flex: 1;
    display: flex;
    align-items: center;
    gap: 10px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    padding: 0 14px;
    transition: border-color 0.15s;
}

.filtro-search:focus-within {
    border-color: #1a3a2a;
}

.filtro-input {
    flex: 1;
    border: none;
    outline: none;
    font-size: 0.9rem;
    color: #111827;
    padding: 11px 0;
    background: transparent;
}

.filtro-input::placeholder {
    color: #9ca3af;
}

.filtro-select {
    padding: 10px 14px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 10px;
    font-size: 0.875rem;
    color: #374151;
    cursor: pointer;
    outline: none;
    transition: border-color 0.15s;
    min-width: 160px;
}

.filtro-select:focus {
    border-color: #1a3a2a;
}

.cond-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 18px;
}

.cond-card {
    background: #fff;
    border-radius: 14px;
    padding: 20px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    display: flex;
    flex-direction: column;
    gap: 6px;
    transition: box-shadow 0.18s, transform 0.18s;
}

.cond-card:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, .1);
    transform: translateY(-2px);
}

.card-top {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 4px;
}

.card-matricula {
    font-size: 0.9rem;
    font-weight: 700;
    color: #111827;
    background: #f3f4f6;
    border-radius: 6px;
    padding: 3px 10px;
    letter-spacing: 0.04em;
}

.card-nombre {
    font-size: 0.925rem;
    font-weight: 600;
    color: #1f2937;
    margin: 0;
}

.card-subtitulo {
    font-size: 0.8rem;
    color: #6b7280;
    margin: 0 0 4px;
}

.card-divider {
    height: 1px;
    background: #f3f4f6;
    margin: 6px 0;
}

.card-detalles {
    display: flex;
    flex-direction: column;
    gap: 5px;
}

.detalle-fila {
    display: flex;
    justify-content: space-between;
    font-size: 0.8rem;
}

.detalle-label {
    color: #6b7280;
}

.detalle-valor {
    color: #111827;
    font-weight: 500;
}

.card-acciones {
    display: flex;
    gap: 8px;
    margin-top: 12px;
}

.btn-accion {
    flex: 1;
    padding: 7px 0;
    border-radius: 8px;
    font-size: 0.8rem;
    font-weight: 600;
    cursor: pointer;
    border: none;
    transition: filter 0.15s;
}

.btn-accion:hover {
    filter: brightness(0.93);
}

.btn-ver {
    background: #d1fae5;
    color: #065f46;
}

.btn-editar {
    background: #fef3c7;
    color: #92400e;
}

.btn-eliminar {
    background: #fee2e2;
    color: #991b1b;
}

.badge {
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

.cond-estado {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    padding: 60px 0;
    color: #6b7280;
}

.spinner {
    width: 36px;
    height: 36px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin 0.75s linear infinite;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}

.cond-error {
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 10px;
    padding: 20px 24px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    color: #991b1b;
    font-size: 0.9rem;
}

.btn-reintentar {
    padding: 7px 16px;
    background: #fff;
    border: 1.5px solid #fca5a5;
    border-radius: 8px;
    color: #991b1b;
    font-size: 0.8rem;
    cursor: pointer;
}

.cond-vacio {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8px;
    padding: 72px 0;
    color: #9ca3af;
    text-align: center;
}

.cond-vacio p {
    font-size: 1rem;
    font-weight: 600;
    color: #6b7280;
    margin: 8px 0 0;
}

.cond-vacio span {
    font-size: 0.85rem;
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
    border-radius: 16px;
    padding: 32px;
    width: 420px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
}

.modal-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 10px;
}

.modal-desc {
    font-size: 0.9rem;
    color: #4b5563;
    line-height: 1.55;
    margin: 0 0 24px;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: flex-end;
}

.btn-cancelar-modal {
    padding: 9px 18px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
}

.btn-confirmar-modal {
    padding: 9px 18px;
    background: #dc2626;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
}

.btn-confirmar-modal:hover {
    background: #b91c1c;
}

@media (max-width: 1024px) {
    .cond-grid {
        grid-template-columns: repeat(2, 1fr);
    }

    .cond-resumen {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 640px) {
    .cond-page {
        padding: 20px 16px;
    }

    .cond-grid {
        grid-template-columns: 1fr;
    }

    .cond-resumen {
        grid-template-columns: repeat(2, 1fr);
    }

    .cond-filtros {
        flex-direction: column;
    }

    .cond-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 14px;
    }

}
@media (max-width: 1024px) {
    .cond-resumen {
        grid-template-columns: repeat(3, 1fr);
    }
}

@media (max-width: 640px) {
    .cond-resumen {
        grid-template-columns: repeat(2, 1fr);
    }
}
</style>
