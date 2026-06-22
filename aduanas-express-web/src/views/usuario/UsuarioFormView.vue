<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { obtenerUsuarios, crearUsuario, actualizarUsuario } from '../../services/usuarioService'

const router = useRouter()
const route  = useRoute()

const loading  = ref(false)
const guardando = ref(false)
const error    = ref('')

const esEdicion = computed(() => !!route.params.id)

const form = ref({
    nombre:   '',
    apellido: '',
    email:    '',
    password: '',
    rol:      2,
})

const ROLES = [
    { label: 'Administrador', value: 0 },
    { label: 'Supervisor',    value: 1 },
    { label: 'Operador',      value: 2 },
]
async function cargarUsuario() {
    loading.value = true
    error.value   = ''
    try {
        const res = await obtenerUsuarios(route.params.id)
        const u   = res.data
        form.value = {
            nombre:   u.nombre   ?? '',
            apellido: u.apellido ?? '',
            email:    u.email    ?? '',
            password: '',
            rol:      u.rol      ?? 2,
        }
    } catch (e) {
        console.error('Error completo:', e)
        console.error('Status:', e?.response?.status)
        console.error('Data:', e?.response?.data)
        error.value = e?.response?.data?.message || 'No se pudo cargar el usuario.'
    } finally {
        loading.value = false
    }
}

function validar() {
    if (!form.value.nombre.trim())   { error.value = 'El nombre es requerido.';   return false }
    if (!form.value.apellido.trim()) { error.value = 'El apellido es requerido.'; return false }
    if (!form.value.email.trim())    { error.value = 'El email es requerido.';    return false }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(form.value.email)) { error.value = 'El email no es válido.'; return false }

    if (!esEdicion.value && form.value.password.length < 6) {
        error.value = 'La contraseña debe tener al menos 6 caracteres.'
        return false
    }
    if (esEdicion.value && form.value.password && form.value.password.length < 6) {
        error.value = 'La nueva contraseña debe tener al menos 6 caracteres.'
        return false
    }

    return true
}

async function guardar() {
    error.value = ''
    if (!validar()) return

    const payload = {
        nombre:   form.value.nombre,
        apellido: form.value.apellido,
        email:    form.value.email,
        password: form.value.password,
        rol:      Number(form.value.rol),
    }

    guardando.value = true
    try {
        if (esEdicion.value) {
            await actualizarUsuario(route.params.id, payload)
        } else {
            await crearUsuario(payload)
        }
        router.push('/usuarios')
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al guardar el usuario.'
    } finally {
        guardando.value = false
    }
}

onMounted(async () => {
    if (esEdicion.value) await cargarUsuario()
})
</script>

<template>
    <div class="uf-page">

        <div class="uf-header">
            <div class="uf-header-left">
                <button class="btn-back" @click="router.push('/usuarios')">
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5">
                        <polyline points="15 18 9 12 15 6" />
                    </svg>
                    Usuarios
                </button>
                <div class="uf-breadcrumb-sep">/</div>
                <span class="uf-breadcrumb-current">
                    {{ esEdicion ? 'Editar usuario' : 'Nuevo usuario' }}
                </span>
            </div>
        </div>

        <div class="uf-page-title">
            <div class="uf-title-icon">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2" />
                    <circle cx="12" cy="7" r="4" />
                </svg>
            </div>
            <div>
                <h1>{{ esEdicion ? 'Editar usuario' : 'Nuevo usuario' }}</h1>
                <p>{{ esEdicion ? 'Actualice los datos de la cuenta seleccionada.' : 'Complete el formulario para crear una nueva cuenta de usuario.' }}</p>
            </div>
        </div>

        <div v-if="error" class="uf-alert">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            {{ error }}
        </div>

        <div class="uf-layout">

            <aside class="uf-aside">
                <div class="aside-section">
                    <p class="aside-label">Módulo</p>
                    <p class="aside-value">Gestión de Usuarios</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Operación</p>
                    <p class="aside-value">{{ esEdicion ? 'Modificación de cuenta' : 'Nueva cuenta' }}</p>
                </div>
                <div class="aside-divider"></div>
                <div class="aside-section">
                    <p class="aside-label">Campos obligatorios</p>
                    <ul class="aside-list">
                        <li>Nombre</li>
                        <li>Apellido</li>
                        <li>Email</li>
                        <li>Rol</li>
                        <li v-if="!esEdicion">Contraseña</li>
                    </ul>
                </div>
            </aside>

            <div class="uf-card">

                <div v-if="loading && esEdicion" class="uf-loading">
                    <div class="spinner"></div>
                </div>

                <template v-else>

                    <div class="form-section">
                        <div class="section-header">
                            <span class="section-tag">01</span>
                            <h3>Datos personales</h3>
                        </div>
                        <div class="form-grid">
                            <div class="field">
                                <label>Nombre <span class="req">*</span></label>
                                <input v-model="form.nombre" type="text" placeholder="Ej. María" autocomplete="off" />
                            </div>
                            <div class="field">
                                <label>Apellido <span class="req">*</span></label>
                                <input v-model="form.apellido" type="text" placeholder="Ej. Pérez" autocomplete="off" />
                            </div>
                            <div class="field form-full">
                                <label>Email <span class="req">*</span></label>
                                <input v-model="form.email" type="email" placeholder="correo@empresa.com" autocomplete="off" />
                            </div>
                        </div>
                    </div>

                    <div class="section-divider"></div>

                    <div class="form-section">
                        <div class="section-header">
                            <span class="section-tag">02</span>
                            <h3>Acceso</h3>
                        </div>
                        <div class="form-grid">
                            <div class="field">
                                <label>Rol <span class="req">*</span></label>
                                <select v-model.number="form.rol">
                                    <option v-for="r in ROLES" :key="r.value" :value="r.value">{{ r.label }}</option>
                                </select>
                            </div>
                            <div class="field">
                                <label>
                                    Contraseña <span v-if="!esEdicion" class="req">*</span>
                                </label>
                                <input
                                    v-model="form.password"
                                    type="password"
                                    :placeholder="esEdicion ? 'Dejar en blanco para no cambiarla' : 'Mínimo 6 caracteres'"
                                    autocomplete="new-password"
                                />
                            </div>
                        </div>
                    </div>

                    <div class="action-bar">
                        <div class="action-bar-left">
                            <button class="btn-secondary" type="button" @click="router.push('/usuarios')" :disabled="guardando">
                                Cancelar
                            </button>
                        </div>
                        <div class="action-bar-right">
                            <button class="btn-primary" type="button" @click="guardar" :disabled="guardando">
                                <span v-if="guardando" class="btn-spinner"></span>
                                {{ guardando ? 'Guardando…' : (esEdicion ? 'Guardar cambios' : 'Crear usuario') }}
                            </button>
                        </div>
                    </div>

                </template>
            </div>
        </div>
    </div>
</template>

<style scoped>
.uf-page {
    padding: 28px 32px;
    background: #f3f4f6;
    min-height: 100vh;
    font-family: 'Inter', 'Segoe UI', sans-serif;
}

.uf-header { margin-bottom: 18px; }

.uf-header-left {
    display: flex;
    align-items: center;
    gap: 10px;
    font-size: 0.85rem;
}

.btn-back {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    border: none;
    background: none;
    color: #6b7280;
    font-size: 0.85rem;
    font-weight: 600;
    cursor: pointer;
    padding: 0;
    font-family: inherit;
}
.btn-back:hover { color: #111827; }

.uf-breadcrumb-sep { color: #d1d5db; }
.uf-breadcrumb-current { color: #111827; font-weight: 600; }

.uf-page-title {
    display: flex;
    align-items: flex-start;
    gap: 14px;
    margin-bottom: 22px;
}

.uf-title-icon {
    width: 42px;
    height: 42px;
    border-radius: 10px;
    background: #1a3a2a;
    color: #fff;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.uf-page-title h1 {
    font-size: 1.3rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 3px;
}

.uf-page-title p {
    font-size: 0.85rem;
    color: #6b7280;
    margin: 0;
}

.uf-alert {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 11px 16px;
    background: #fee2e2;
    border: 1px solid #fca5a5;
    color: #991b1b;
    border-radius: 9px;
    font-size: 0.85rem;
    margin-bottom: 18px;
}

.uf-layout {
    display: grid;
    grid-template-columns: 260px 1fr;
    gap: 20px;
}

.uf-aside {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    padding: 20px;
    align-self: start;
    position: sticky;
    top: 20px;
}

.aside-divider { height: 1px; background: #f3f4f6; margin: 14px 0; }

.aside-label {
    font-size: 0.68rem;
    font-weight: 700;
    color: #9ca3af;
    text-transform: uppercase;
    letter-spacing: 0.06em;
    margin: 0 0 4px;
}

.aside-value { font-size: 0.85rem; font-weight: 600; color: #111827; margin: 0; }

.aside-list {
    margin: 0;
    padding-left: 18px;
    font-size: 0.8rem;
    color: #4b5563;
    line-height: 1.7;
}

.uf-card {
    background: #fff;
    border-radius: 14px;
    box-shadow: 0 1px 4px rgba(0, 0, 0, .07);
    overflow: hidden;
}

.uf-loading {
    display: flex;
    justify-content: center;
    padding: 60px 0;
}

.spinner {
    width: 32px; height: 32px;
    border: 3px solid #e5e7eb;
    border-top-color: #1a3a2a;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

.form-section { padding: 28px 32px; }

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

.section-divider { height: 1px; background: #f3f4f6; margin: 0 32px; }

.form-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 18px;
}

.form-full { grid-column: 1 / -1; }

.field { display: flex; flex-direction: column; gap: 6px; }

.field label {
    font-size: 0.78rem;
    font-weight: 700;
    color: #374151;
    text-transform: uppercase;
    letter-spacing: 0.05em;
}

.req { color: #dc2626; margin-left: 2px; }

.field input, .field select {
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
    width: 100%;
    box-sizing: border-box;
}

.field input:focus, .field select:focus {
    border-color: #1a3a2a;
    box-shadow: 0 0 0 3px rgba(26, 58, 42, 0.1);
}

.field input::placeholder { color: #9ca3af; }

.action-bar {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 20px 32px;
    border-top: 1px solid #f3f4f6;
    background: #fafafa;
}

.action-bar-left, .action-bar-right { display: flex; gap: 10px; align-items: center; }

.btn-primary, .btn-secondary {
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

.btn-primary:disabled, .btn-secondary:disabled { opacity: 0.6; cursor: not-allowed; }

.btn-primary { background: #1a3a2a; color: #fff; }
.btn-primary:hover:not(:disabled) { background: #14532d; }

.btn-secondary { background: #f3f4f6; color: #374151; border: 1.5px solid #e5e7eb; }
.btn-secondary:hover:not(:disabled) { background: #e5e7eb; }

.btn-spinner {
    width: 13px; height: 13px;
    border: 2px solid rgba(255, 255, 255, .4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin 0.65s linear infinite;
    flex-shrink: 0;
}

@media (max-width: 900px) {
    .uf-layout { grid-template-columns: 1fr; }
    .uf-aside  { position: static; }
}

@media (max-width: 640px) {
    .uf-page      { padding: 20px 16px; }
    .form-section { padding: 20px 16px; }
    .section-divider { margin: 0 16px; }
    .action-bar   { padding: 16px; flex-direction: column; gap: 12px; }
    .action-bar-right { width: 100%; justify-content: flex-end; }
    .form-grid    { grid-template-columns: 1fr; }
}
</style>