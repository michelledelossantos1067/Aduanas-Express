<script setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { usePermisos } from '../composables/usePermisos'
import { useAuthStore } from '../stores/authStore'
import { useRouter } from 'vue-router'
const router = useRouter()

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
function cerrarSesion() {
    authStore.logout()
    router.push('/login')
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
                <span class="nav-section-title">GESTIÓN</span>

                <router-link v-if="puede.verVehiculos.value" to="/vehiculos" class="nav-item">
                    <span class="nav-icon">🚌</span>
                    <span>Vehículos</span>
                </router-link>

                <router-link v-if="puede.verConductores.value" to="/conductores" class="nav-item">
                    <span class="nav-icon">👤</span>
                    <span>Conductores</span>
                </router-link>
                <router-link v-if="puede.verConsumoCombustible.value" to="/consumo-combustible" class="nav-item">
                    <span class="nav-icon">⛽️</span>
                    <span>Consumo Combustible</span>
                </router-link>

                <router-link v-if="puede.verSolicitudes.value" to="/solicitudes" class="nav-item">
                    <span class="nav-icon">📋</span>
                    <span>Solicitudes</span>
                </router-link>

                <router-link v-if="puede.verAsignaciones.value" to="/asignaciones" class="nav-item">
                    <span class="nav-icon">📝</span>
                    <span>Asignaciones</span>
                </router-link>
            </div>

            <div class="nav-section">
                <span class="nav-section-title">OPERACIÓN</span>

                <router-link to="/agenda" class="nav-item">
                    <span class="nav-icon">📅</span>
                    <span>Agenda</span>
                </router-link>

                <router-link to="/mantenimiento" class="nav-item">
                    <span class="nav-icon">🔧</span>
                    <span>Mantenimiento</span>
                </router-link>

                <router-link v-if="puede.verReportes.value" to="/reportes" class="nav-item">
                    <span class="nav-icon">📊</span>
                    <span>Reportes</span>
                </router-link>

                <router-link to="/monitoreo" class="nav-item">
                    <span class="nav-icon">🖥️</span>
                    <span>Monitoreo</span>
                </router-link>

            
            </div>

            <div class="nav-section">
                <span class="nav-section-title">ADMINISTRACIÓN</span>

                <router-link v-if="puede.verUsuarios.value" to="/usuarios" class="nav-item">
                    <span class="nav-icon">👥</span>
                    <span>Usuarios</span>
                </router-link>
            </div>

        </nav>

        <div class="sidebar-footer">
            <div class="user-avatar">{{ userInitials }}</div>
            <div class="user-info">
                <span class="user-name">{{ authStore.usuario?.nombre || 'Usuario' }}</span>
                <span class="user-role">{{ authStore.usuario?.rolId || 'Sin rol' }}</span>
            </div>
            <div class="footer-gear-wrap" ref="gearWrap">
                <button class="btn-gear" @click="gearOpen = !gearOpen" title="Opciones">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="3" />
                        <path
                            d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z" />
                    </svg>
                </button>
                <div v-if="gearOpen" class="gear-dropdown">
                    <router-link to="/archivados" class="gear-item" @click="gearOpen = false">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2">
                            <path
                                d="M5 8h14M5 8a2 2 0 1 0-4 0v10a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V8m-14 0V6a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" />
                        </svg>
                        Ver archivados
                    </router-link>
                    <router-link v-if="puede.verRoles.value" to="/roles" class="gear-item" @click="gearOpen = false">
                        <span class="nav-icon">⚙️</span>
                        <span>Roles</span>
                    </router-link>
                    <button class="gear-item gear-item-danger" @click="cerrarSesion(); gearOpen = false">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor"
                            stroke-width="2">
                            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                            <polyline points="16 17 21 12 16 7" />
                            <line x1="21" y1="12" x2="9" y2="12" />
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

    background: linear-gradient(180deg,
            #16382d 0%,
            #103126 50%,
            #0d2a20 100%);

    display: flex;
    flex-direction: column;

    color: #ffffff;
    font-family: sans-serif;

    flex-shrink: 0;
    z-index: 100;
}

.sidebar-header {
    padding: 10px 12px;
    border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.sidebar-logo {
    display: flex;
    align-items: center;
    gap: 10px;
}

.logo-icon {
    width: 52px;
    height: 52px;

    display: flex;
    align-items: center;
    justify-content: center;

    font-size: 24px;

    background: rgba(255, 255, 255, 0.1);
    border-radius: 12px;
}

.logo-text {
    display: flex;
    flex-direction: column;
}

.logo-title {
    font-size: 14px;
    font-weight: 700;
}

.logo-subtitle {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.6);
}

.sidebar-nav {
    flex: 1;
    overflow-y: auto;

    scrollbar-width: none;
    -ms-overflow-style: none;

    padding: 4px 0;
}

.sidebar-nav::-webkit-scrollbar {
    display: none;
}

.nav-section {
    margin-bottom: 2px;
}

.nav-section-title {
    padding: 8px 16px 4px;

    font-size: 10px;
    font-weight: 700;

    text-transform: uppercase;
    letter-spacing: 1px;

    color: rgba(255, 255, 255, 0.45);
}

.nav-item {
    display: flex;
    align-items: center;
    gap: 12px;

    margin: 1px 8px;
    padding: 8px 14px;

    border-radius: 10px;

    text-decoration: none;
    color: rgba(255, 255, 255, 0.8);

    font-size: 14px;
    font-weight: 500;

    transition: all 0.2s ease;
}

.nav-item:hover {
    background: rgba(255, 255, 255, 0.08);
    color: #fff;
    transform: translateX(2px);
}

.nav-item.router-link-active {
    background: rgba(76, 175, 130, 0.18);
    color: #fff;
}

.nav-icon {
    width: 18px;
    text-align: center;
    font-size: 16px;
}

.sidebar-footer {
    display: flex;
    align-items: center;
    gap: 10px;

    padding: 12px;

    border-top: 1px solid rgba(255, 255, 255, 0.08);

    background: rgba(255, 255, 255, 0.03);
}

.user-avatar {
    width: 42px;
    height: 42px;

    background: #4caf82;
    border-radius: 50%;

    display: flex;
    align-items: center;
    justify-content: center;

    font-size: 16px;
    font-weight: 700;
}

.user-info {
    display: flex;
    flex-direction: column;
}

.user-name {
    font-size: 14px;
    font-weight: 700;
}

.user-role {
    font-size: 11px;
    color: rgba(255, 255, 255, 0.55);
}

.footer-gear-wrap {
    position: relative;
    margin-left: auto;
}

.btn-gear {
    width: 34px;
    height: 34px;

    border-radius: 10px;
    border: 1px solid rgba(255, 255, 255, 0.12);

    background: rgba(255, 255, 255, 0.06);
    color: rgba(255, 255, 255, 0.7);

    display: flex;
    align-items: center;
    justify-content: center;

    cursor: pointer;

    transition: all 0.2s ease;
}

.btn-gear:hover {
    background: rgba(255, 255, 255, 0.12);
    color: #fff;
}

.gear-dropdown {
    position: absolute;
    bottom: calc(100% + 8px);
    right: 0;

    min-width: 180px;

    background: #fff;
    border-radius: 12px;

    overflow: hidden;

    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.gear-item {
    display: flex;
    align-items: center;
    gap: 10px;

    width: 100%;
    padding: 12px 14px;

    border: none;
    background: none;

    text-decoration: none;
    text-align: left;

    color: #374151;
    cursor: pointer;
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