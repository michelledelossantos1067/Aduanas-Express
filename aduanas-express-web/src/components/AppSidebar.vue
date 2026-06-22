<script setup>
import { computed, reactive, ref, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '../stores/authStore'
import { usePermisos } from '../composables/usePermisos'

const authStore = useAuthStore()
const { puede } = usePermisos()
const gearOpen = ref(false)
const gearWrap = ref(null)

function handleClickOutside(e) {
    if (gearWrap.value && !gearWrap.value.contains(e.target)) {
        gearOpen.value = false
    }
}

onMounted(() => document.addEventListener('mousedown', handleClickOutside))
onUnmounted(() => document.removeEventListener('mousedown', handleClickOutside))

const userInitials = computed(() => {
    const name = authStore.usuario?.nombre || ''
    return name
        .split(' ')
        .map(n => n[0])
        .join('')
        .substring(0, 2)
        .toUpperCase()
})

const openSections = reactive({
    gestion: true,
    operacion: false,
    administracion: false
})

function toggleSection(key) {
    openSections[key] = !openSections[key]
}
</script>

<template>
    <aside class="sidebar">

        <div class="sidebar-header">
            <div class="sidebar-logo">
                <div class="logo-icon">🚛</div>
                <div class="logo-text">
                    <span class="logo-title">Aduanas Express</span>
                    <span class="logo-subtitle">Sistema de transporte</span>
                </div>
            </div>
        </div>

        <nav class="sidebar-nav">

            <div class="nav-section">
                <span class="nav-section-title">PRINCIPAL</span>
                <router-link to="/dashboard" class="nav-item">
                    <span class="nav-icon">⊞</span>
                    <span>Dashboard</span>
                </router-link>
            </div>

            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('gestion')">
                    <span class="nav-section-title">GESTIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.gestion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.gestion">
                    <router-link to="/vehiculos" class="nav-item">
                        <span class="nav-icon">🚌</span>
                        <span>Vehiculos</span>
                    </router-link>
                    <router-link to="/conductores" class="nav-item">
                        <span class="nav-icon">👤</span>
                        <span>Conductores</span>
                    </router-link>
                    <router-link to="/solicitudes" class="nav-item">
                        <span class="nav-icon">📋</span>
                        <span>Solicitudes</span>
                    </router-link>
                    <router-link to="/asignaciones" class="nav-item">
                        <span class="nav-icon">📝</span>
                        <span>Asignaciones</span>
                    </router-link>
                </div>
            </div>

            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('operacion')">
                    <span class="nav-section-title">OPERACIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.operacion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.operacion">
                    <router-link to="/agenda" class="nav-item">
                        <span class="nav-icon">📅</span>
                        <span>Agenda</span>
                    </router-link>
                    <router-link to="/mantenimiento" class="nav-item">
                        <span class="nav-icon">🔧</span>
                        <span>Mantenimiento</span>
                    </router-link>
                    <!-- Solo Admin y Supervisor ven Reportes -->
                    <router-link v-if="puede.verReportes.value" to="/reportes" class="nav-item">
                        <span class="nav-icon">📊</span>
                        <span>Reportes</span>
                    </router-link>
                    <router-link to="/monitoreo" class="nav-item">
                        <span class="nav-icon">🖥️</span>
                        <span>Monitoreo</span>
                    </router-link>
                    <router-link to="/historial" class="nav-item">
                        <span class="nav-icon">🗂️</span>
                        <span>Historial</span>
                    </router-link>
                </div>
            </div>

            <div class="nav-section">
                <button class="nav-section-toggle" @click="toggleSection('administracion')">
                    <span class="nav-section-title">ADMINISTRACIÓN</span>
                    <span class="nav-chevron" :class="{ open: openSections.administracion }">▾</span>
                </button>
                <div class="nav-collapsible" v-show="openSections.administracion">
                    <!-- Solo Admin ve Usuarios -->
                    <router-link v-if="puede.verUsuarios.value" to="/usuarios" class="nav-item">
                        <span class="nav-icon">👥</span>
                        <span>Usuarios</span>
                    </router-link>
                    <!-- Solo Admin ve Roles -->
                    <router-link v-if="puede.verRoles.value" to="/roles" class="nav-item">
                        <span class="nav-icon">⚙️</span>
                        <span>Roles</span>
                    </router-link>
                </div>
            </div>
        </nav>

        <div class="sidebar-footer">
            <div class="user-avatar">{{ userInitials }}</div>
            <div class="user-info">
                <span class="user-name">{{ authStore.usuario?.nombre || 'Usuario' }}</span>
                <span class="user-role">{{ authStore.usuario?.rol || 'Sin rol' }}</span>
            </div>
            <div class="footer-gear-wrap" ref="gearWrap">
                <button class="btn-gear" @click="gearOpen = !gearOpen" title="Opciones">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="3"/>
                        <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"/>
                    </svg>
                </button>
                <div v-if="gearOpen" class="gear-dropdown">
                    <router-link to="/archivados" class="gear-item" @click="gearOpen = false">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M5 8h14M5 8a2 2 0 1 0-4 0v10a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V8m-14 0V6a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/>
                        </svg>
                        Ver archivados
                    </router-link>
                    <button class="gear-item gear-item-danger" @click="authStore.logout(); gearOpen = false">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                            <polyline points="16 17 21 12 16 7"/>
                            <line x1="21" y1="12" x2="9" y2="12"/>
                        </svg>
                        Cerrar sesión
                    </button>
                </div>
            </div>
        </div>
    </aside>
</template>

<style scoped>
.sidebar {
    position: sticky;
    top: 0;
    width: 250px;
    height: 100vh;
    background-color: #1a3a2e;
    display: flex;
    flex-direction: column;
    color: #ffffff;
    font-family: sans-serif;
    flex-shrink: 0;
    z-index: 100;
}

.sidebar-header {
    padding: 14px 14px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.sidebar-logo {
    display: flex;
    align-items: center;
    gap: 8px;
}

.logo-icon {
    font-size: 20px;
    background: rgba(255, 255, 255, 0.1);
    padding: 6px;
    border-radius: 6px;
}

.logo-text {
    display: flex;
    flex-direction: column;
}

.logo-title {
    font-size: 15px;
    font-weight: 700;
    color: #ffffff;
    line-height: 1.2;
}

.logo-subtitle {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.6);
}

.sidebar-nav {
    flex: 1;
    padding: 6px 0;
    overflow-y: auto;
}

.nav-section {
    margin-bottom: 2px;
}

.nav-section-title {
    display: block;
    font-size: 10.5px;
    font-weight: 600;
    color: rgba(255, 255, 255, 0.4);
    letter-spacing: 0.5px;
}

.nav-section-toggle {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    background: none;
    border: none;
    cursor: pointer;
    padding: 9px 16px 5px;
    color: inherit;
    font-family: inherit;
}

.nav-section-toggle .nav-section-title {
    padding: 0;
}

.nav-chevron {
    font-size: 10px;
    color: rgba(255, 255, 255, 0.4);
    transition: transform 0.2s;
}

.nav-chevron.open {
    transform: rotate(180deg);
}

.nav-collapsible {
    display: flex;
    flex-direction: column;
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 10px;
    padding: 7px 16px;
    color: rgba(255, 255, 255, 0.75);
    text-decoration: none;
    font-size: 14px;
    transition: background 0.2s, color 0.2s;
}

.nav-item:hover {
    background: rgba(255, 255, 255, 0.08);
    color: #ffffff;
}

.nav-item.router-link-active {
    background: rgba(255, 255, 255, 0.12);
    color: #ffffff;
    border-left: 3px solid #4caf82;
}

.nav-icon {
    font-size: 16px;
    width: 18px;
    text-align: center;
}

.sidebar-footer {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 10px 14px;
    border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.user-avatar {
    width: 32px;
    height: 32px;
    background: #4caf82;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 700;
    font-size: 13px;
    flex-shrink: 0;
}

.user-info {
    display: flex;
    flex-direction: column;
}

.user-name {
    font-size: 13px;
    font-weight: 600;
    color: #ffffff;
}

.user-role {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.55);
}

.footer-gear-wrap {
    position: relative;
    margin-left: auto;
    flex-shrink: 0;
}

.btn-gear {
    width: 30px;
    height: 30px;
    border-radius: 7px;
    border: 1px solid rgba(255,255,255,0.15);
    background: rgba(255,255,255,0.07);
    color: rgba(255,255,255,0.65);
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: background 0.15s, color 0.15s;
}

.btn-gear:hover {
    background: rgba(255,255,255,0.14);
    color: #fff;
}

.gear-dropdown {
    position: absolute;
    bottom: calc(100% + 8px);
    right: 0;
    background: #fff;
    border-radius: 10px;
    box-shadow: 0 8px 24px rgba(0,0,0,0.18);
    min-width: 170px;
    overflow: hidden;
    z-index: 200;
}

.gear-item {
    display: flex;
    align-items: center;
    gap: 9px;
    width: 100%;
    padding: 11px 14px;
    font-size: 0.85rem;
    font-weight: 500;
    color: #374151;
    background: none;
    border: none;
    cursor: pointer;
    text-decoration: none;
    transition: background 0.13s;
    font-family: inherit;
    text-align: left;
}

.gear-item:hover {
    background: #f3f4f6;
}

.gear-item-danger {
    color: #dc2626;
}

.gear-item-danger:hover {
    background: #fef2f2;
}
</style>