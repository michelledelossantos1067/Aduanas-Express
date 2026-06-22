<template>
    <div class="usr-page">

        <div class="usr-header">
            <h1 class="usr-title">Gestión de Usuarios</h1>
            <div class="usr-header-actions">
                <button class="btn-exportar" @click="exportarPdf">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                        <polyline points="7 10 12 15 17 10" />
                        <line x1="12" y1="15" x2="12" y2="3" />
                    </svg>
                    Exportar
                </button>
                <!-- Solo Admin puede crear usuarios -->
                <button v-if="puede.crearUsuarios.value" class="btn-nuevo" @click="irANuevo">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                        stroke-width="2.5">
                        <line x1="12" y1="5" x2="12" y2="19" />
                        <line x1="5" y1="12" x2="19" y2="12" />
                    </svg>
                    Nuevo Usuario
                </button>
            </div>
        </div>

        <div class="usr-stats">
            <div class="stat-card">
                <p class="stat-num">{{ resumen.total }}</p>
                <p class="stat-label">Total</p>
            </div>
            <div class="stat-card">
                <p class="stat-num">{{ resumen.administradores }}</p>
                <p class="stat-label">Administradores</p>
            </div>
            <div class="stat-card">
                <p class="stat-num">{{ resumen.supervisores }}</p>
                <p class="stat-label">Supervisores</p>
            </div>
            <div class="stat-card">
                <p class="stat-num">{{ resumen.operadores }}</p>
                <p class="stat-label">Operadores</p>
            </div>
        </div>

        <div class="usr-filtros">
            <div class="filtro-search">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
                    <circle cx="11" cy="11" r="8" />
                    <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                <input v-model="busqueda" type="text" placeholder="Buscar por nombre o email..." class="filtro-input" />
            </div>
            <select v-model="filtroRol" class="filtro-select">
                <option value="">Todos los roles</option>
                <option v-for="r in ROLES" :key="r.value" :value="r.value">{{ r.label }}</option>
            </select>
        </div>

        <div v-if="loading" class="usr-estado">
            <div class="spinner"></div>
            <p>Cargando usuarios…</p>
        </div>

        <div v-else-if="error" class="usr-error">
            <p>{{ error }}</p>
            <button class="btn-reintentar" @click="cargarUsuarios">Reintentar</button>
        </div>

        <div v-else class="usr-tabla-wrap">
            <div class="tabla-header">
                <h2 class="tabla-titulo">Listado de usuarios</h2>
                <span class="tabla-badge">{{ usuariosFiltrados.length }} usuarios</span>
            </div>

            <div class="tabla-scroll">
                <table>
                    <thead>
                        <tr>
                            <th>Usuario</th>
                            <th>Email</th>
                            <th>Rol</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="usuariosPagina.length === 0">
                            <td colspan="4" class="td-vacio">No se encontraron usuarios</td>
                        </tr>
                        <tr v-for="u in usuariosPagina" :key="u.id">
                            <td>
                                <div class="usuario-cell">
                                    <div class="avatar" :class="avatarColor(u.id)">
                                        {{ getInitials(u.nombre, u.apellido) }}
                                    </div>
                                    <span class="usuario-nombre">{{ u.nombre }} {{ u.apellido }}</span>
                                </div>
                            </td>
                            <td class="td-email">{{ u.email }}</td>
                            <td>
                                <span class="badge-rol" :class="rolClase[u.rol]">
                                    {{ rolLabel(u.rol) }}
                                </span>
                            </td>
                            <td>
                                <div class="acciones">
                                    <button @click="verUsuario(u)" class="btn-accion btn-ver">
                                        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                                        Ver
                                    </button>
                                    <button v-if="puede.editarUsuarios.value" @click="editarUsuario(u.id)" class="btn-accion btn-editar">
                                        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                                        Editar
                                    </button>
                                    <button v-if="u.puedeEliminarse && puede.eliminarUsuarios.value" @click="confirmarEliminar(u)" class="btn-accion btn-eliminar">
                                        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6"/><path d="M10 11v6"/><path d="M14 11v6"/><path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/></svg>
                                        Eliminar
                                    </button>
                                    <button v-if="!u.puedeEliminarse && puede.eliminarUsuarios.value" @click="toggleActivo(u)" :disabled="cambiandoEstado"
                                        :class="['btn-accion', u.isActive ? 'btn-desactivar' : 'btn-activar']">
                                        <svg v-if="u.isActive" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><circle cx="12" cy="12" r="10"/><line x1="4.93" y1="4.93" x2="19.07" y2="19.07"/></svg>
                                        <svg v-else width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"><polyline points="20 6 9 17 4 12"/></svg>
                                        {{ u.isActive ? 'Desactivar' : 'Activar' }}
                                    </button>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="usr-paginacion">
                <span class="pag-info">
                    Mostrando {{ desde }}–{{ hasta }} de {{ usuariosFiltrados.length }} usuarios
                </span>
                <div class="pag-btns">
                    <button @click="pagina = Math.max(1, pagina - 1)" :disabled="pagina === 1"
                        class="btn-pag">&lt;</button>
                    <template v-for="p in paginasVisibles">
                        <span v-if="p === '...'" :key="`ellipsis-${p}`" class="pag-ellipsis">…</span>
                        <button v-else :key="p" @click="pagina = p" class="btn-pag" :class="{ activo: pagina === p }">
                            {{ p }}
                        </button>
                    </template>
                    <button @click="pagina = Math.min(totalPaginas, pagina + 1)" :disabled="pagina === totalPaginas"
                        class="btn-pag">&gt;</button>
                </div>
            </div>
        </div>

        <div v-if="mostrarVer && usuarioSeleccionado" class="modal-overlay" @click.self="mostrarVer = false">
            <div class="modal">
                <div class="modal-top">
                    <div class="avatar avatar-lg" :class="avatarColor(usuarioSeleccionado.id)">
                        {{ getInitials(usuarioSeleccionado.nombre, usuarioSeleccionado.apellido) }}
                    </div>
                    <div>
                        <h3 class="modal-titulo">{{ usuarioSeleccionado.nombre }} {{ usuarioSeleccionado.apellido }}
                        </h3>
                        <span class="badge-rol" :class="rolClase[usuarioSeleccionado.rol]">
                            {{ rolLabel(usuarioSeleccionado.rol) }}
                        </span>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="modal-fila">
                        <span class="modal-label">Email</span>
                        <span class="modal-valor">{{ usuarioSeleccionado.email }}</span>
                    </div>
                    <div class="modal-fila">
                        <span class="modal-label">ID</span>
                        <span class="modal-valor">#{{ usuarioSeleccionado.id }}</span>
                    </div>
                </div>
                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="mostrarVer = false">Cerrar</button>
                    <button v-if="puede.editarUsuarios.value" class="btn-editar-modal" @click="editarUsuario(usuarioSeleccionado.id)">Editar</button>
                </div>
            </div>
        </div>

        <div v-if="mostrarEliminar && usuarioAEliminar" class="modal-overlay" @click.self="mostrarEliminar = false">
            <div class="modal">
                <h3 class="modal-titulo">Eliminar usuario</h3>
                <p class="modal-desc">
                    ¿Estás seguro de que deseas eliminar a
                    <strong>{{ usuarioAEliminar.nombre }} {{ usuarioAEliminar.apellido }}</strong>?
                    Esta acción no se puede deshacer.
                </p>
                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="mostrarEliminar = false">Cancelar</button>
                    <button class="btn-confirmar-modal" @click="ejecutarEliminar" :disabled="eliminando">
                        {{ eliminando ? 'Eliminando…' : 'Eliminar' }}
                    </button>
                </div>
            </div>
        </div>

    </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { usePermisos } from '../../composables/usePermisos'
import { obtenerUsuario, eliminarUsuario, desactivarUsuario, activarUsuario } from '../../services/usuarioService'

const router = useRouter()
const { puede } = usePermisos()

const ROLES = [
    { label: 'Administrador', value: 0 },
    { label: 'Supervisor', value: 1 },
    { label: 'Operador', value: 2 },
]

const ITEMS_POR_PAGINA = 9

const rolClase = {
    0: 'rol-admin',
    1: 'rol-supervisor',
    2: 'rol-operador',
}

const avatarColors = ['av-purple', 'av-blue', 'av-green', 'av-orange', 'av-teal']

const usuarios = ref([])
const loading = ref(false)
const error = ref('')
const busqueda = ref('')
const filtroRol = ref('')
const pagina = ref(1)
const mostrarVer = ref(false)
const usuarioSeleccionado = ref(null)
const mostrarEliminar = ref(false)
const usuarioAEliminar = ref(null)
const eliminando = ref(false)
const cambiandoEstado = ref(false)

function getInitials(nombre = '', apellido = '') {
    return `${nombre[0] ?? ''}${apellido[0] ?? ''}`.toUpperCase()
}

function rolLabel(rol) {
    return ROLES.find(r => r.value === rol)?.label ?? rol
}

function avatarColor(id) {
    return avatarColors[id % avatarColors.length]
}

const resumen = computed(() => ({
    total: usuarios.value.length,
    administradores: usuarios.value.filter(u => u.rol === 0).length,
    supervisores: usuarios.value.filter(u => u.rol === 1).length,
    operadores: usuarios.value.filter(u => u.rol === 2).length,
}))

const usuariosFiltrados = computed(() =>
    usuarios.value.filter(u => {
        const q = busqueda.value.toLowerCase()
        const matchBusqueda =
            !q ||
            u.nombre.toLowerCase().includes(q) ||
            u.apellido.toLowerCase().includes(q) ||
            u.email.toLowerCase().includes(q)
        const matchRol = filtroRol.value === '' || u.rol === Number(filtroRol.value)
        return matchBusqueda && matchRol
    })
)

const totalPaginas = computed(() =>
    Math.max(1, Math.ceil(usuariosFiltrados.value.length / ITEMS_POR_PAGINA))
)

const usuariosPagina = computed(() =>
    usuariosFiltrados.value.slice(
        (pagina.value - 1) * ITEMS_POR_PAGINA,
        pagina.value * ITEMS_POR_PAGINA
    )
)

const desde = computed(() =>
    usuariosFiltrados.value.length === 0 ? 0 : (pagina.value - 1) * ITEMS_POR_PAGINA + 1
)

const hasta = computed(() =>
    Math.min(pagina.value * ITEMS_POR_PAGINA, usuariosFiltrados.value.length)
)

const paginasVisibles = computed(() => {
    const total = totalPaginas.value
    const actual = pagina.value
    const pages = []

    if (total <= 5) {
        for (let i = 1; i <= total; i++) pages.push(i)
    } else {
        pages.push(1)
        if (actual > 3) pages.push('...')
        for (let i = Math.max(2, actual - 1); i <= Math.min(total - 1, actual + 1); i++) {
            pages.push(i)
        }
        if (actual < total - 2) pages.push('...')
        pages.push(total)
    }

    return pages
})

async function cargarUsuarios() {
    loading.value = true
    error.value = ''
    try {
        const res = await obtenerUsuario()
        usuarios.value = res.data
    } catch (e) {
        error.value = 'No se pudieron cargar los usuarios.'
    } finally {
        loading.value = false
    }
}

function irANuevo() {
    router.push('/usuarios/nuevo')
}

function verUsuario(u) {
    usuarioSeleccionado.value = u
    mostrarVer.value = true
}

function editarUsuario(id) {
    router.push(`/usuario/${id}/editar`)
}

function confirmarEliminar(u) {
    usuarioAEliminar.value = u
    mostrarEliminar.value = true
}

async function ejecutarEliminar() {
    eliminando.value = true
    try {
        await eliminarUsuario(usuarioAEliminar.value.id)
        usuarios.value = usuarios.value.filter(u => u.id !== usuarioAEliminar.value.id)
        mostrarEliminar.value = false
    } catch (e) {
        error.value = 'Error al eliminar el usuario.'
    } finally {
        eliminando.value = false
    }
}

async function toggleActivo(usuario) {
    cambiandoEstado.value = true
    try {
        if (usuario.isActive) {
            await desactivarUsuario(usuario.id)
        } else {
            await activarUsuario(usuario.id)
        }
        usuario.isActive = !usuario.isActive
    } catch (e) {
        error.value = 'No se pudo cambiar el estado del usuario.'
    } finally {
        cambiandoEstado.value = false
    }
}

function exportarPdf() {
    // Lógica de exportación
    console.log('Exportar PDF')
}

watch(pagina, () => {
    window.scrollTo({ top: 0, behavior: 'smooth' })
})

onMounted(cargarUsuarios)
</script>

<style scoped>
.usr-page {
    padding: 24px;
    max-width: 1200px;
    margin: 0 auto;
}

.usr-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 28px;
}

.usr-title {
    font-size: 1.875rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.usr-header-actions {
    display: flex;
    gap: 12px;
}

.btn-exportar, .btn-nuevo {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 16px;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    cursor: pointer;
    transition: background 0.13s;
    font-family: inherit;
}

.btn-exportar {
    background: #f3f4f6;
    color: #374151;
}

.btn-exportar:hover {
    background: #e5e7eb;
}

.btn-nuevo {
    background: #1a3a2a;
    color: #fff;
}

.btn-nuevo:hover {
    background: #0f2818;
}

.usr-stats {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 16px;
    margin-bottom: 28px;
}

.stat-card {
    background: #fff;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    padding: 20px 16px;
    text-align: center;
}

.stat-num {
    font-size: 1.75rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 4px;
}

.stat-label {
    font-size: 0.875rem;
    color: #6b7280;
    margin: 0;
}

.usr-filtros {
    display: flex;
    gap: 12px;
    margin-bottom: 20px;
}

.filtro-search {
    flex: 1;
    display: flex;
    align-items: center;
    padding: 0 12px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    gap: 8px;
}

.filtro-search svg {
    flex-shrink: 0;
}

.filtro-input {
    flex: 1;
    border: none;
    outline: none;
    padding: 8px 0;
    font-size: 0.875rem;
    font-family: inherit;
}

.filtro-select {
    min-width: 160px;
    padding: 8px 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    background: #fff;
    font-size: 0.875rem;
    cursor: pointer;
    font-family: inherit;
}

.usr-estado, .usr-error {
    text-align: center;
    padding: 60px 20px;
}

.spinner {
    width: 40px;
    height: 40px;
    border: 3px solid #f3f4f6;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin 0.6s linear infinite;
    margin: 0 auto 12px;
}

@keyframes spin {
    to { transform: rotate(360deg); }
}

.usr-error {
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 12px;
    color: #991b1b;
}

.usr-error p {
    margin: 0 0 12px;
}

.btn-reintentar {
    padding: 8px 16px;
    background: #991b1b;
    color: #fff;
    border: none;
    border-radius: 6px;
    cursor: pointer;
}

.usr-tabla-wrap {
    background: #fff;
    border: 1px solid #e5e7eb;
    border-radius: 12px;
    overflow: hidden;
}

.tabla-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 24px;
    border-bottom: 1px solid #f3f4f6;
}

.tabla-titulo {
    font-size: 1rem;
    font-weight: 600;
    color: #111827;
    margin: 0;
}

.tabla-badge {
    font-size: 0.75rem;
    color: #6b7280;
    background: #f3f4f6;
    padding: 4px 10px;
    border-radius: 6px;
}

.tabla-scroll {
    overflow-x: auto;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead {
    background: #f9fafb;
}

th {
    padding: 12px 24px;
    text-align: left;
    font-size: 0.75rem;
    font-weight: 600;
    color: #6b7280;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

td {
    padding: 14px 24px;
    border-bottom: 1px solid #f3f4f6;
    font-size: 0.875rem;
    color: #374151;
}

.usuario-cell {
    display: flex;
    align-items: center;
    gap: 10px;
}

.avatar {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
    font-size: 0.75rem;
    color: #fff;
    flex-shrink: 0;
}

.avatar-lg {
    width: 64px;
    height: 64px;
    font-size: 1.5rem;
}

.av-purple { background: #8b5cf6; }
.av-blue { background: #3b82f6; }
.av-green { background: #10b981; }
.av-orange { background: #f97316; }
.av-teal { background: #14b8a6; }

.usuario-nombre {
    font-weight: 500;
    color: #111827;
}

.td-email {
    color: #6b7280;
}

.td-vacio {
    text-align: center;
    color: #9ca3af;
}

.badge-rol {
    display: inline-block;
    padding: 4px 10px;
    border-radius: 6px;
    font-size: 0.75rem;
    font-weight: 600;
}

.rol-admin { background: #fef3c7; color: #92400e; }
.rol-supervisor { background: #dbeafe; color: #1e40af; }
.rol-operador { background: #d1fae5; color: #065f46; }

.acciones {
    display: flex;
    gap: 6px;
    flex-wrap: wrap;
}

.btn-accion {
    display: inline-flex;
    align-items: center;
    gap: 5px;
    padding: 5px 11px;
    border-radius: 7px;
    border: 1.5px solid #e5e7eb;
    background: #fff;
    font-size: 0.78rem;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.13s, border-color 0.13s, color 0.13s;
    white-space: nowrap;
    font-family: inherit;
}

.btn-ver { color: #374151; }
.btn-ver:hover { background: #f3f4f6; border-color: #d1d5db; }

.btn-editar { color: #92400e; border-color: #fde68a; background: #fffbeb; }
.btn-editar:hover { background: #fef3c7; border-color: #f59e0b; }

.btn-eliminar { color: #991b1b; border-color: #fecaca; background: #fff5f5; }
.btn-eliminar:hover { background: #fee2e2; border-color: #f87171; }

.btn-desactivar { color: #6b7280; border-color: #e5e7eb; background: #f9fafb; }
.btn-desactivar:hover { background: #f3f4f6; border-color: #9ca3af; }
.btn-desactivar:disabled { opacity: 0.5; cursor: default; }

.btn-activar { color: #065f46; border-color: #a7f3d0; background: #f0fdf4; }
.btn-activar:hover { background: #d1fae5; border-color: #34d399; }
.btn-activar:disabled { opacity: 0.5; cursor: default; }

.usr-paginacion {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 14px 24px;
    border-top: 1px solid #f3f4f6;
}

.pag-info {
    font-size: 0.75rem;
    color: #6b7280;
}

.pag-btns {
    display: flex;
    gap: 4px;
    align-items: center;
}

.btn-pag {
    min-width: 30px;
    height: 30px;
    padding: 0 8px;
    border: 1.5px solid #e5e7eb;
    border-radius: 6px;
    background: #fff;
    font-size: 0.8rem;
    color: #6b7280;
    cursor: pointer;
    transition: background 0.15s, border-color 0.15s, color 0.15s;
}

.btn-pag:hover:not(:disabled) {
    background: #f3f4f6;
}

.btn-pag:disabled {
    opacity: 0.4;
    cursor: default;
}

.btn-pag.activo {
    background: #1f2937;
    color: #fff;
    border-color: #1f2937;
}

.pag-ellipsis {
    color: #9ca3af;
    font-size: 0.8rem;
    padding: 0 4px;
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
    width: 440px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
}

.modal-top {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 20px;
}

.modal-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 6px;
}

.modal-body {
    display: flex;
    flex-direction: column;
    gap: 10px;
    margin-bottom: 24px;
}

.modal-fila {
    display: flex;
    justify-content: space-between;
    font-size: 0.875rem;
    padding: 8px 0;
    border-bottom: 1px solid #f3f4f6;
}

.modal-label {
    color: #6b7280;
}

.modal-valor {
    color: #111827;
    font-weight: 500;
}

.modal-desc {
    font-size: 0.9rem;
    color: #4b5563;
    line-height: 1.55;
    margin: 8px 0 24px;
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

.btn-editar-modal {
    padding: 9px 18px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
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
    transition: background 0.15s;
}

.btn-confirmar-modal:hover:not(:disabled) {
    background: #b91c1c;
}

.btn-confirmar-modal:disabled {
    opacity: 0.6;
    cursor: default;
}

@media (max-width: 1024px) {
    .usr-stats {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 640px) {
    .usr-page {
        padding: 20px 16px;
    }

    .usr-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 14px;
    }

    .usr-stats {
        grid-template-columns: repeat(2, 1fr);
    }

    .usr-filtros {
        flex-direction: column;
    }
}
</style>