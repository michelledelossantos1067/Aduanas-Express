<script setup>
import { ref, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useRouter } from 'vue-router'
import { verVehiculos, activarVehiculo } from '@/services/vehiculoService'
import { verConductores, activarConductor } from '@/services/conductorService'
import { obtenerUsuario, activarUsuario } from '@/services/usuarioService'

const authStore = useAuthStore()
const router = useRouter()

if (authStore.usuario?.rol !== 'Administrador') {
    router.replace('/dashboard')
}

const vehiculos = ref([])
const conductores = ref([])
const usuarios = ref([])
const loading = ref(false)
const error = ref('')
const toast = ref('')
const reactivando = ref(null)

const busquedaVeh = ref('')
const busquedaCond = ref('')
const busquedaUsr = ref('')

const vehiculosArchivados = computed(() =>
    vehiculos.value.filter(v => {
        if (v.isActive) return false
        const q = busquedaVeh.value.toLowerCase()
        return !q || v.matricula?.toLowerCase().includes(q) || v.marca?.toLowerCase().includes(q) || v.modelo?.toLowerCase().includes(q)
    })
)

const conductoresArchivados = computed(() =>
    conductores.value.filter(c => {
        if (c.isActive) return false
        const q = busquedaCond.value.toLowerCase()
        return !q || c.nombre?.toLowerCase().includes(q) || c.apellido?.toLowerCase().includes(q) || c.cedula?.toLowerCase().includes(q)
    })
)

const usuariosArchivados = computed(() =>
    usuarios.value.filter(u => {
        if (u.isActive) return false
        const q = busquedaUsr.value.toLowerCase()
        return !q || u.nombre?.toLowerCase().includes(q) || u.apellido?.toLowerCase().includes(q) || u.email?.toLowerCase().includes(q)
    })
)

const ROLES = [
    { label: 'Administrador', value: 0 },
    { label: 'Supervisor', value: 1 },
    { label: 'Operador', value: 2 },
]
const ESTADOS_VEH = ['Disponible', 'En Viaje', 'En Mantenimiento', 'Fuera de Servicio']
const ESTADOS_COND = ['Disponible', 'En Viaje', 'Suspendido', 'Inactivo']

const estadoBadgeVeh = ['badge-disponible', 'badge-en-viaje', 'badge-mantenimiento', 'badge-fuera-servicio']
const estadoBadgeCond = ['badge-disponible', 'badge-en-viaje', 'badge-suspendido', 'badge-inactivo']

const avatarColors = ['av-purple', 'av-blue', 'av-green', 'av-orange', 'av-teal']

function rolLabel(rol) { return ROLES.find(r => r.value === rol)?.label ?? '—' }
function rolClase(rol) { return ['rol-admin', 'rol-supervisor', 'rol-operador'][rol] ?? '' }
function avatarColor(id) { return avatarColors[id % avatarColors.length] }
function initials(nombre = '', apellido = '') { return `${nombre[0] ?? ''}${apellido[0] ?? ''}`.toUpperCase() }

function formatFecha(fecha) {
    if (!fecha) return '—'
    return new Date(fecha).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

async function cargarTodo() {
    loading.value = true
    error.value = ''
    try {
        const [rv, rc, ru] = await Promise.all([verVehiculos(), verConductores(), obtenerUsuario()])
        vehiculos.value = rv.data
        conductores.value = rc.data
        usuarios.value = ru.data
    } catch {
        error.value = 'Error al cargar los archivados.'
    } finally {
        loading.value = false
    }
}

async function reactivarVehiculo(v) {
    reactivando.value = `v-${v.id}`
    try {
        await activarVehiculo(v.id)
        v.isActive = true
        showToast(`Vehículo ${v.matricula} reactivado. Ya aparece en la lista de vehículos.`)
    } catch { error.value = 'No se pudo reactivar el vehículo.' }
    finally { reactivando.value = null }
}

async function reactivarConductor(c) {
    reactivando.value = `c-${c.id}`
    try {
        await activarConductor(c.id)
        c.isActive = true
        showToast(`Conductor ${c.nombre} ${c.apellido} reactivado. Ya aparece en la lista de conductores.`)
    } catch { error.value = 'No se pudo reactivar el conductor.' }
    finally { reactivando.value = null }
}

async function reactivarUsuario(u) {
    reactivando.value = `u-${u.id}`
    try {
        await activarUsuario(u.id)
        u.isActive = true
        showToast(`Usuario ${u.nombre} ${u.apellido} reactivado. Ya aparece en la lista de usuarios.`)
    } catch { error.value = 'No se pudo reactivar el usuario.' }
    finally { reactivando.value = null }
}

function showToast(msg) {
    toast.value = msg
    setTimeout(() => toast.value = '', 3500)
}

onMounted(cargarTodo)
</script>

<template>
    <div class="arch-page">

        <!-- Header -->
        <div class="arch-header">
            <div class="arch-header-left">
                <button class="btn-back" @click="router.back()">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="15 18 9 12 15 6"/></svg>
                </button>
                <div>
                    <h1 class="arch-title">Archivados</h1>
                    <p class="arch-subtitle">Vista exclusiva para administradores</p>
                </div>
            </div>
        </div>

        <!-- Toast -->
        <div v-if="toast" class="arch-toast">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><polyline points="20 6 9 17 4 12"/></svg>
            {{ toast }}
        </div>

        <!-- Error -->
        <div v-if="error" class="arch-error">{{ error }}</div>

        <!-- Loading -->
        <div v-if="loading" class="arch-loading">
            <div class="spinner"></div>
            <p>Cargando archivados…</p>
        </div>

        <template v-else>

            <!-- ══ SECCIÓN 1: VEHÍCULOS ══ -->
            <section class="arch-section">
                <div class="section-header">
                    <div class="section-header-left">
                        <div class="section-icon section-icon-veh">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="1" y="3" width="15" height="13" rx="2"/><path d="M16 8h4l3 3v5h-7V8z"/><circle cx="5.5" cy="18.5" r="2.5"/><circle cx="18.5" cy="18.5" r="2.5"/></svg>
                        </div>
                        <div>
                            <h2 class="section-title">Vehículos</h2>
                            <span class="section-count">{{ vehiculosArchivados.length }} archivado{{ vehiculosArchivados.length !== 1 ? 's' : '' }}</span>
                        </div>
                    </div>
                    <div class="section-search">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
                        <input v-model="busquedaVeh" type="text" placeholder="Buscar vehículo…" class="search-input" />
                    </div>
                </div>

                <div v-if="vehiculosArchivados.length === 0" class="section-empty">
                    No hay vehículos archivados.
                </div>
                <div v-else class="cards-grid">
                    <div v-for="v in vehiculosArchivados" :key="'v'+v.id" class="arch-card">
                        <div class="arch-card-top">
                            <span class="card-tag">{{ v.matricula }}</span>
                            <span class="badge" :class="estadoBadgeVeh[v.estado]">{{ ESTADOS_VEH[v.estado] }}</span>
                        </div>
                        <p class="arch-card-nombre">{{ v.marca }} {{ v.modelo }} <span class="arch-card-year">{{ v.año }}</span></p>
                        <p class="arch-card-sub">{{ v.tipo }} · {{ v.capacidad }} pasajeros · {{ v.color ?? '—' }}</p>
                        <div class="arch-card-row">
                            <span class="arch-card-label">Últ. Mantenimiento</span>
                            <span class="arch-card-val">{{ formatFecha(v.fechaUltimoMant) }}</span>
                        </div>
                        <button class="btn-desarchivar" :disabled="reactivando === `v-${v.id}`" @click="reactivarVehiculo(v)">
                            {{ reactivando === `v-${v.id}` ? 'Reactivando…' : '↩ Desarchivar' }}
                        </button>
                    </div>
                </div>
            </section>

            <!-- ══ SECCIÓN 2: CONDUCTORES ══ -->
            <section class="arch-section">
                <div class="section-header">
                    <div class="section-header-left">
                        <div class="section-icon section-icon-cond">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
                        </div>
                        <div>
                            <h2 class="section-title">Conductores</h2>
                            <span class="section-count">{{ conductoresArchivados.length }} archivado{{ conductoresArchivados.length !== 1 ? 's' : '' }}</span>
                        </div>
                    </div>
                    <div class="section-search">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
                        <input v-model="busquedaCond" type="text" placeholder="Buscar conductor…" class="search-input" />
                    </div>
                </div>

                <div v-if="conductoresArchivados.length === 0" class="section-empty">
                    No hay conductores archivados.
                </div>
                <div v-else class="cards-grid">
                    <div v-for="c in conductoresArchivados" :key="'c'+c.id" class="arch-card">
                        <div class="arch-card-top">
                            <div class="cond-avatar">{{ initials(c.nombre, c.apellido) }}</div>
                            <span class="badge" :class="estadoBadgeCond[c.estado]">{{ ESTADOS_COND[c.estado] }}</span>
                        </div>
                        <p class="arch-card-nombre">{{ c.nombre }} {{ c.apellido }}</p>
                        <p class="arch-card-sub">Cédula: {{ c.cedula ?? '—' }}</p>
                        <div class="arch-card-row">
                            <span class="arch-card-label">Licencia</span>
                            <span class="arch-card-val">{{ c.tipoLicencia ?? '—' }}</span>
                        </div>
                        <div class="arch-card-row">
                            <span class="arch-card-label">Vence</span>
                            <span class="arch-card-val">{{ formatFecha(c.fechaVencLicencia) }}</span>
                        </div>
                        <button class="btn-desarchivar" :disabled="reactivando === `c-${c.id}`" @click="reactivarConductor(c)">
                            {{ reactivando === `c-${c.id}` ? 'Reactivando…' : '↩ Desarchivar' }}
                        </button>
                    </div>
                </div>
            </section>

            <!-- ══ SECCIÓN 3: USUARIOS ══ -->
            <section class="arch-section">
                <div class="section-header">
                    <div class="section-header-left">
                        <div class="section-icon section-icon-usr">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                        </div>
                        <div>
                            <h2 class="section-title">Usuarios</h2>
                            <span class="section-count">{{ usuariosArchivados.length }} archivado{{ usuariosArchivados.length !== 1 ? 's' : '' }}</span>
                        </div>
                    </div>
                    <div class="section-search">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2"><circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/></svg>
                        <input v-model="busquedaUsr" type="text" placeholder="Buscar usuario…" class="search-input" />
                    </div>
                </div>

                <div v-if="usuariosArchivados.length === 0" class="section-empty">
                    No hay usuarios archivados.
                </div>
                <div v-else class="cards-grid">
                    <div v-for="u in usuariosArchivados" :key="'u'+u.id" class="arch-card">
                        <div class="arch-card-top">
                            <div class="usr-avatar" :class="avatarColor(u.id)">{{ initials(u.nombre, u.apellido) }}</div>
                            <span class="badge-rol" :class="rolClase(u.rol)">{{ rolLabel(u.rol) }}</span>
                        </div>
                        <p class="arch-card-nombre">{{ u.nombre }} {{ u.apellido }}</p>
                        <p class="arch-card-sub">{{ u.email }}</p>
                        <button class="btn-desarchivar" :disabled="reactivando === `u-${u.id}`" @click="reactivarUsuario(u)">
                            {{ reactivando === `u-${u.id}` ? 'Reactivando…' : '↩ Desarchivar' }}
                        </button>
                    </div>
                </div>
            </section>

        </template>
    </div>
</template>

<style scoped>
.arch-page {
    padding: 32px 40px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
    display: flex;
    flex-direction: column;
    gap: 32px;
}

/* Header */
.arch-header { display: flex; align-items: center; justify-content: space-between; }
.arch-header-left { display: flex; align-items: center; gap: 14px; }
.btn-back { width: 36px; height: 36px; border-radius: 8px; border: 1.5px solid #e5e7eb; background: #fff; cursor: pointer; display: inline-flex; align-items: center; justify-content: center; color: #374151; }
.btn-back:hover { background: #f9fafb; }
.arch-title { font-size: 1.6rem; font-weight: 700; color: #111827; margin: 0; }
.arch-subtitle { font-size: 0.78rem; color: #6b7280; margin: 2px 0 0; }

/* Toast / Error */
.arch-toast { background: #d1fae5; color: #065f46; border-radius: 8px; padding: 10px 16px; font-size: 0.875rem; font-weight: 500; display: flex; align-items: center; gap: 8px; }
.arch-error { background: #fef2f2; color: #991b1b; border-radius: 8px; padding: 10px 16px; font-size: 0.875rem; }
.arch-loading { display: flex; flex-direction: column; align-items: center; gap: 12px; padding: 60px 0; color: #6b7280; }
.spinner { width: 36px; height: 36px; border: 3px solid #e5e7eb; border-top-color: #1a3a2a; border-radius: 50%; animation: spin 0.75s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }

/* Section */
.arch-section { background: #fff; border-radius: 16px; padding: 24px; box-shadow: 0 1px 4px rgba(0,0,0,.06); }

.section-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 20px; flex-wrap: wrap; gap: 12px; }
.section-header-left { display: flex; align-items: center; gap: 12px; }

.section-icon { width: 36px; height: 36px; border-radius: 10px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.section-icon-veh { background: #dbeafe; color: #1e40af; }
.section-icon-cond { background: #d1fae5; color: #065f46; }
.section-icon-usr { background: #ede9fe; color: #5b21b6; }

.section-title { font-size: 1rem; font-weight: 700; color: #111827; margin: 0; }
.section-count { font-size: 0.75rem; color: #6b7280; }

.section-search { display: flex; align-items: center; gap: 7px; background: #f9fafb; border: 1.5px solid #e5e7eb; border-radius: 8px; padding: 0 10px; }
.search-input { border: none; outline: none; background: transparent; font-size: 0.8rem; color: #111827; padding: 7px 0; width: 180px; font-family: inherit; }

.section-empty { text-align: center; color: #9ca3af; font-size: 0.85rem; padding: 28px 0; }

/* Cards grid */
.cards-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(230px, 1fr)); gap: 14px; }

.arch-card {
    border: 1.5px solid #e5e7eb;
    border-radius: 12px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 5px;
    background: #fafafa;
    transition: box-shadow 0.15s;
}
.arch-card:hover { box-shadow: 0 2px 10px rgba(0,0,0,.07); }

.arch-card-top { display: flex; align-items: center; justify-content: space-between; margin-bottom: 4px; }

.card-tag { font-size: 0.82rem; font-weight: 700; color: #374151; background: #f3f4f6; border-radius: 6px; padding: 2px 8px; letter-spacing: 0.04em; }

.arch-card-nombre { font-size: 0.9rem; font-weight: 700; color: #1f2937; margin: 0; }
.arch-card-year { font-weight: 400; color: #6b7280; }
.arch-card-sub { font-size: 0.75rem; color: #6b7280; margin: 0 0 4px; }
.arch-card-row { display: flex; justify-content: space-between; font-size: 0.75rem; margin-top: 2px; }
.arch-card-label { color: #9ca3af; }
.arch-card-val { color: #374151; font-weight: 500; }

/* Avatars */
.cond-avatar { width: 32px; height: 32px; border-radius: 50%; background: #d1fae5; color: #065f46; font-size: 0.7rem; font-weight: 700; display: flex; align-items: center; justify-content: center; }
.usr-avatar { width: 32px; height: 32px; border-radius: 50%; font-size: 0.7rem; font-weight: 700; display: flex; align-items: center; justify-content: center; color: #fff; }
.av-purple { background: #7c3aed; }
.av-blue { background: #2563eb; }
.av-green { background: #16a34a; }
.av-orange { background: #ea580c; }
.av-teal { background: #0d9488; }

/* Badges */
.badge { display: inline-block; padding: 2px 8px; border-radius: 20px; font-size: 0.7rem; font-weight: 600; }
.badge-disponible { background: #d1fae5; color: #065f46; }
.badge-en-viaje { background: #dbeafe; color: #1e40af; }
.badge-mantenimiento { background: #fef3c7; color: #92400e; }
.badge-fuera-servicio { background: #fee2e2; color: #991b1b; }
.badge-suspendido { background: #fef3c7; color: #92400e; }
.badge-inactivo { background: #fee2e2; color: #991b1b; }

.badge-rol { display: inline-block; padding: 2px 8px; border-radius: 20px; font-size: 0.7rem; font-weight: 600; }
.rol-admin { background: #fee2e2; color: #991b1b; }
.rol-supervisor { background: #dbeafe; color: #1e40af; }
.rol-operador { background: #d1fae5; color: #065f46; }

/* Desarchivar button */
.btn-desarchivar { margin-top: 10px; width: 100%; padding: 7px 0; border-radius: 7px; border: 1.5px solid #1a3a2a; background: #fff; color: #1a3a2a; font-size: 0.8rem; font-weight: 600; cursor: pointer; transition: background 0.13s, color 0.13s; font-family: inherit; }
.btn-desarchivar:hover:not(:disabled) { background: #1a3a2a; color: #fff; }
.btn-desarchivar:disabled { opacity: 0.5; cursor: default; }

@media (max-width: 640px) {
    .arch-page { padding: 20px 16px; gap: 20px; }
    .cards-grid { grid-template-columns: 1fr; }
    .section-header { flex-direction: column; align-items: flex-start; }
    .search-input { width: 140px; }
}
</style>