<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { obtenerUsuario } from '@/services/usuarioService'
import axios from '../../utils/axiosConfig'

const iconosPorNombre = {
    Administrador: '🛡️',
    Supervisor: '👁️',
    Operador: '👤',
}

const grupos = [
    {
        key: 'vehiculos',
        nombre: 'Vehículos',
        icono: '🚌',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'crear', label: 'Crear' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },
    {
        key: 'conductores',
        nombre: 'Conductores',
        icono: '👤',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'crear', label: 'Crear' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },{
        key: 'consumo-combustible',
        nombre: 'consumo-combustible',
        icono: '⛽️',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'crear', label: 'Crear' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },
    {
        key: 'solicitudes',
        nombre: 'Solicitudes',
        icono: '📋',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'crear', label: 'Crear' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },
    {
        key: 'asignaciones',
        nombre: 'Asignaciones',
        icono: '📝',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'asignar', label: 'Asignar' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },
    {
        key: 'reportes',
        nombre: 'Reportes',
        icono: '📊',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'exportar', label: 'Exportar PDF' },
            { key: 'estadisticas', label: 'Estadísticas' },
        ],
    },
    {
        key: 'usuarios',
        nombre: 'Usuarios y Roles',
        icono: '👥',
        acciones: [
            { key: 'ver', label: 'Ver' },
            { key: 'crear', label: 'Crear' },
            { key: 'editar', label: 'Editar' },
            { key: 'cancelar', label: 'Cancelar' },
        ],
    },
]

const roles = ref([])
const seleccionado = ref(0)
const usuarios = ref([])
const cargando = ref(false)
const guardando = ref(false)
const toast = ref({ visible: false, mensaje: '', error: false })
const modalNuevo = ref(false)
const nuevoNombre = ref('')
const nuevoDesc = ref('')
const permisosActivos = reactive({})

const rolActual = computed(() => roles.value[seleccionado.value] ?? {})

function contarUsuariosPorRol(nombreRol) {
    return usuarios.value.filter(u => u.rolNombre?.toLowerCase() === nombreRol.toLowerCase()).length
}

function permisosAObjeto(permisos = []) {
    const obj = {}
    for (const grupo of grupos) {
        obj[grupo.key] = {}
        for (const accion of grupo.acciones) {
            const p = permisos.find(x => x.modulo === grupo.key && x.accion === accion.key)
            obj[grupo.key][accion.key] = p?.permitido ?? false
        }
    }
    return obj
}

function objetoAPermisos(obj) {
    const lista = []
    for (const [modulo, acciones] of Object.entries(obj)) {
        for (const [accion, permitido] of Object.entries(acciones)) {
            lista.push({ modulo, accion, permitido })
        }
    }
    return lista
}

async function cargarRoles() {
    cargando.value = true
    try {
        const { data } = await axios.get('/Rol')
        roles.value = data.map(r => ({
            ...r,
            icono: iconosPorNombre[r.nombre] ?? '⚙️',
        }))
        if (roles.value.length > 0) {
            seleccionarRol(0)
        }
    } catch {
        mostrarToast('Error al cargar roles', true)
    } finally {
        cargando.value = false
    }
}

async function cargarUsuarios() {
    try {
        const { data } = await obtenerUsuario()
        usuarios.value = data
    } catch {
        usuarios.value = []
    }
}

function seleccionarRol(idx) {
    seleccionado.value = idx
    const nuevo = permisosAObjeto(roles.value[idx]?.permisos ?? [])
    for (const modulo of Object.keys(nuevo)) {
        if (!permisosActivos[modulo]) {
            permisosActivos[modulo] = {}
        }
        Object.assign(permisosActivos[modulo], nuevo[modulo])
    }
}

async function guardarPermisos() {
    guardando.value = true
    try {
        const rolId = rolActual.value.id
        const permisos = objetoAPermisos(permisosActivos)
        await axios.put(`/Rol/${rolId}/permisos`, { permisos })
        roles.value[seleccionado.value].permisos = permisos
        mostrarToast(`Permisos de ${rolActual.value.nombre} guardados`)
    } catch {
        mostrarToast('Error al guardar permisos', true)
    } finally {
        guardando.value = false
    }
}

function descartarCambios() {
    const nuevo = permisosAObjeto(roles.value[seleccionado.value]?.permisos ?? [])
    for (const modulo of Object.keys(nuevo)) {
        if (!permisosActivos[modulo]) {
            permisosActivos[modulo] = {}
        }
        Object.assign(permisosActivos[modulo], nuevo[modulo])
    }
    mostrarToast('Cambios descartados')
}

async function crearRol() {
    if (!nuevoNombre.value.trim()) return
    try {
        await axios.post('/Rol', {
            nombre: nuevoNombre.value.trim(),
            descripcion: nuevoDesc.value.trim() || 'Sin descripción',
        })
        modalNuevo.value = false
        nuevoNombre.value = ''
        nuevoDesc.value = ''
        mostrarToast('Rol creado correctamente')
        await cargarRoles()
        seleccionarRol(roles.value.length - 1)
    } catch {
        mostrarToast('Error al crear el rol', true)
    }
}

function mostrarToast(msg, error = false) {
    toast.value = { visible: true, mensaje: msg, error }
    setTimeout(() => { toast.value.visible = false }, 2500)
}

onMounted(() => {
    cargarRoles()
    cargarUsuarios()
})
</script>

<template>
    <div class="roles-page">

        <div class="page-header">
            <h1 class="page-title">Gestión de roles y permisos</h1>
            <button class="btn-nuevo" @click="modalNuevo = true">
                <span>＋</span> Nuevo Rol
            </button>
        </div>

        <div v-if="cargando" class="estado-carga">Cargando roles...</div>

        <div v-else class="roles-layout">

            <aside class="roles-panel">
                <div v-for="(rol, idx) in roles" :key="rol.id" class="role-card"
                    :class="{ active: idx === seleccionado }" @click="seleccionarRol(idx)">
                    <div class="role-card-header">
                        <div class="role-icon-wrap">
                            <span class="role-icon">{{ rol.icono }}</span>
                        </div>
                        <div>
                            <p class="role-nombre">{{ rol.nombre }}</p>
                            <p class="role-desc">{{ rol.descripcion }}</p>
                        </div>
                    </div>

                    <div class="role-meta">
                        <span class="tag-usuarios">
                            👥 {{ contarUsuariosPorRol(rol.nombre) }} usuarios
                        </span>
                        <span class="tag-permisos">{{ rol.esSistema ? 'Sistema' : 'Personalizado' }}</span>
                    </div>

                    <div class="role-card-footer">
                        <label class="check-label" @click.stop>
                            <input type="checkbox" :checked="idx === seleccionado" @change="seleccionarRol(idx)" />
                            Ver
                        </label>
                        <button class="btn-editar-perms" @click.stop="seleccionarRol(idx)">
                            Editar Permisos
                        </button>
                    </div>
                </div>
            </aside>

            <section class="permisos-panel" v-if="rolActual.nombre">
                <div class="permisos-header">
                    <p class="permisos-titulo">{{ rolActual.nombre }}</p>
                    <p class="permisos-subtitulo">{{ rolActual.descripcion }}</p>
                </div>

                <div class="grupos-lista">
                    <div v-for="grupo in grupos" :key="grupo.key" class="grupo-card">
                        <div class="grupo-header">
                            <span class="grupo-icono">{{ grupo.icono }}</span>
                            <span class="grupo-nombre">{{ grupo.nombre }}</span>
                        </div>
                        <div class="acciones-grid">
                            <label v-for="accion in grupo.acciones" :key="accion.key" class="accion-item"
                                :class="{ checked: permisosActivos[grupo.key]?.[accion.key] }">
                                <input type="checkbox" v-model="permisosActivos[grupo.key][accion.key]" />
                                <span class="accion-icono">
                                    <span v-if="accion.key === 'ver'">👁</span>
                                    <span v-else-if="accion.key === 'crear'">＋</span>
                                    <span v-else-if="accion.key === 'editar'">✏️</span>
                                    <span v-else-if="accion.key === 'cancelar'">✕</span>
                                    <span v-else-if="accion.key === 'asignar'">🔗</span>
                                    <span v-else-if="accion.key === 'exportar'">📄</span>
                                    <span v-else-if="accion.key === 'estadisticas'">📈</span>
                                </span>
                                {{ accion.label }}
                            </label>
                        </div>
                    </div>
                </div>

                <div class="acciones-bar">
                    <button class="btn-descartar" @click="descartarCambios">Descartar</button>
                    <button class="btn-guardar" @click="guardarPermisos" :disabled="guardando">
                        {{ guardando ? 'Guardando...' : '✔ Guardar permisos' }}
                    </button>
                </div>
            </section>
        </div>

        <Teleport to="body">
            <div v-if="modalNuevo" class="modal-overlay" @click.self="modalNuevo = false">
                <div class="modal">
                    <h2 class="modal-titulo">Nuevo rol</h2>

                    <label class="modal-label">Nombre del rol</label>
                    <input v-model="nuevoNombre" class="modal-input" placeholder="Ej: Auditor"
                        @keyup.enter="crearRol" />

                    <label class="modal-label">Descripción</label>
                    <input v-model="nuevoDesc" class="modal-input" placeholder="Ej: Acceso de solo lectura"
                        @keyup.enter="crearRol" />

                    <div class="modal-footer">
                        <button class="btn-descartar" @click="modalNuevo = false">Cancelar</button>
                        <button class="btn-guardar" @click="crearRol">Crear rol</button>
                    </div>
                </div>
            </div>
        </Teleport>

        <Teleport to="body">
            <div class="toast" :class="{ visible: toast.visible, error: toast.error }">
                {{ toast.error ? '✖' : '✔' }} {{ toast.mensaje }}
            </div>
        </Teleport>
    </div>
</template>

<style scoped>
.roles-page {
    display: flex;
    flex-direction: column;
    height: 100%;
    background: #f5f7f5;
    font-family: sans-serif;
}

.page-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 18px 28px;
    background: #ffffff;
    border-bottom: 1px solid #e4e9e4;
}

.page-title {
    font-size: 20px;
    font-weight: 700;
    color: #1a2e1a;
}

.btn-nuevo {
    display: flex;
    align-items: center;
    gap: 6px;
    background: #1a3a2e;
    color: #fff;
    border: none;
    border-radius: 8px;
    padding: 9px 16px;
    font-size: 14px;
    font-weight: 600;
    cursor: pointer;
    transition: background 0.15s;
}

.btn-nuevo:hover {
    background: #254d3d;
}

.estado-carga {
    display: flex;
    align-items: center;
    justify-content: center;
    flex: 1;
    font-size: 15px;
    color: #6b7c6b;
}

.roles-layout {
    display: flex;
    flex: 1;
    overflow: hidden;
}

.roles-panel {
    width: 270px;
    flex-shrink: 0;
    border-right: 1px solid #e4e9e4;
    background: #fff;
    overflow-y: auto;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.role-card {
    border: 1.5px solid #e4e9e4;
    border-radius: 12px;
    padding: 14px;
    cursor: pointer;
    transition: border-color 0.15s, background 0.15s;
}

.role-card:hover {
    border-color: #4caf82;
}

.role-card.active {
    border-color: #1a3a2e;
    background: #f0f7f3;
}

.role-card-header {
    display: flex;
    align-items: flex-start;
    gap: 10px;
    margin-bottom: 10px;
}

.role-icon-wrap {
    width: 36px;
    height: 36px;
    border-radius: 8px;
    background: #e8f5ee;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.role-icon {
    font-size: 18px;
}

.role-nombre {
    font-size: 14px;
    font-weight: 700;
    color: #1a2e1a;
    margin: 0;
}

.role-desc {
    font-size: 11px;
    color: #6b7c6b;
    margin: 2px 0 0;
}

.role-meta {
    display: flex;
    flex-wrap: wrap;
    gap: 6px;
    margin-bottom: 10px;
}

.tag-usuarios {
    background: #e8f5ee;
    color: #1a4a2e;
    font-size: 11px;
    padding: 3px 9px;
    border-radius: 99px;
    font-weight: 600;
}

.tag-permisos {
    background: #1a3a2e;
    color: #fff;
    font-size: 11px;
    padding: 3px 9px;
    border-radius: 99px;
    font-weight: 500;
}

.role-card-footer {
    display: flex;
    align-items: center;
    gap: 8px;
}

.check-label {
    display: flex;
    align-items: center;
    gap: 5px;
    font-size: 12px;
    color: #4a6a4a;
    cursor: pointer;
}

.check-label input {
    accent-color: #1a3a2e;
    width: 14px;
    height: 14px;
}

.btn-editar-perms {
    background: transparent;
    border: 1px solid #c5d5c5;
    border-radius: 6px;
    font-size: 12px;
    color: #4a6a4a;
    padding: 4px 10px;
    cursor: pointer;
    transition: background 0.12s;
}

.btn-editar-perms:hover {
    background: #e8f5ee;
}

.permisos-panel {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
}

.permisos-header {
    padding: 18px 24px 12px;
    border-bottom: 1px solid #e4e9e4;
    background: #fff;
}

.permisos-titulo {
    font-size: 16px;
    font-weight: 700;
    color: #1a2e1a;
    margin: 0;
}

.permisos-subtitulo {
    font-size: 13px;
    color: #6b7c6b;
    margin: 3px 0 0;
}

.grupos-lista {
    flex: 1;
    overflow-y: auto;
    padding: 16px 24px;
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.grupo-card {
    background: #fff;
    border: 1px solid #e4e9e4;
    border-radius: 10px;
    overflow: hidden;
}

.grupo-header {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 11px 16px;
    background: #f8faf8;
    border-bottom: 1px solid #e4e9e4;
}

.grupo-icono {
    font-size: 18px;
}

.grupo-nombre {
    font-size: 14px;
    font-weight: 600;
    color: #1a2e1a;
}

.acciones-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    padding: 12px 16px;
    gap: 8px 0;
}

.accion-item {
    display: flex;
    align-items: center;
    gap: 7px;
    font-size: 13px;
    color: #6b7c6b;
    cursor: pointer;
    padding: 5px 0;
    transition: color 0.12s;
}

.accion-item:hover {
    color: #1a2e1a;
}

.accion-item.checked {
    color: #1a3a2e;
    font-weight: 600;
}

.accion-item input[type='checkbox'] {
    accent-color: #1a3a2e;
    width: 15px;
    height: 15px;
    cursor: pointer;
}

.accion-icono {
    font-size: 14px;
}

.acciones-bar {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    padding: 14px 24px;
    background: #fff;
    border-top: 1px solid #e4e9e4;
}

.btn-descartar {
    background: transparent;
    border: 1px solid #c5d5c5;
    border-radius: 8px;
    color: #4a6a4a;
    font-size: 13px;
    padding: 8px 18px;
    cursor: pointer;
    transition: background 0.12s;
}

.btn-descartar:hover {
    background: #f0f7f3;
}

.btn-guardar {
    background: #1a3a2e;
    color: #fff;
    border: none;
    border-radius: 8px;
    font-size: 13px;
    font-weight: 600;
    padding: 8px 18px;
    cursor: pointer;
    transition: background 0.15s;
}

.btn-guardar:hover:not(:disabled) {
    background: #254d3d;
}

.btn-guardar:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, 0.45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
}

.modal {
    background: #fff;
    border-radius: 14px;
    padding: 28px;
    width: 380px;
    box-shadow: 0 12px 40px rgba(0, 0, 0, 0.18);
}

.modal-titulo {
    font-size: 17px;
    font-weight: 700;
    color: #1a2e1a;
    margin: 0 0 16px;
}

.modal-label {
    display: block;
    font-size: 12px;
    color: #6b7c6b;
    margin: 12px 0 4px;
    font-weight: 600;
}

.modal-input {
    width: 100%;
    padding: 9px 12px;
    border: 1px solid #c5d5c5;
    border-radius: 8px;
    font-size: 14px;
    color: #1a2e1a;
    outline: none;
    transition: border-color 0.15s;
    box-sizing: border-box;
}

.modal-input:focus {
    border-color: #1a3a2e;
}

.modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 22px;
}

.toast {
    position: fixed;
    bottom: 28px;
    right: 28px;
    background: #1a3a2e;
    color: #fff;
    padding: 11px 18px;
    border-radius: 10px;
    font-size: 13px;
    font-weight: 600;
    pointer-events: none;
    opacity: 0;
    transform: translateY(8px);
    transition: opacity 0.2s, transform 0.2s;
    z-index: 2000;
}

.toast.visible {
    opacity: 1;
    transform: translateY(0);
}

.toast.error {
    background: #a83232;
}
</style>