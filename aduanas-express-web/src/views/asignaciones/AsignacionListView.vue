<script setup>
import { ref } from 'vue'
import AsignacionFormView from './AsignacionFormView.vue'
import AsignacionHistorialView from './AsignacionHistorialView.vue'

const vistaActiva = ref('asignar')
const exitoMsg = ref('')
const errorMsg = ref('')

const formRef = ref(null)
const historialRef = ref(null)

function mostrarMensaje(tipo, mensaje) {
    if (tipo === 'exito') {
        exitoMsg.value = mensaje
        errorMsg.value = ''
    } else {
        errorMsg.value = mensaje
        exitoMsg.value = ''
    }
    setTimeout(() => { exitoMsg.value = ''; errorMsg.value = '' }, 3500)
}

function onExito(mensaje) {
    mostrarMensaje('exito', mensaje)
}

function onError(mensaje) {
    mostrarMensaje('error', mensaje)
}

function onAsignacionCreada() {
    historialRef.value?.cargarHistorial()
}

function onCancelada() {
    formRef.value?.cargarSolicitudes()
}

function actualizarTodo() {
    formRef.value?.actualizar()
    historialRef.value?.cargarHistorial()
}

function irAHistorial() {
    vistaActiva.value = 'historial'
}
</script>

<template>
    <div class="asig-page">
        <div class="asig-header">
            <h1 class="asig-title">Asignación de vehículos y conductores</h1>
            <div class="asig-header-actions">
                <button
                    class="btn-historial"
                    :class="{ 'btn-activo': vistaActiva === 'historial' }"
                    @click="vistaActiva === 'historial' ? vistaActiva = 'asignar' : irAHistorial()"
                >
                    Historial
                </button>
                <button class="btn-actualizar" @click="actualizarTodo">
                    <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2">
                        <polyline points="1 4 1 10 7 10" />
                        <path d="M3.51 15a9 9 0 1 0 .49-4.95" />
                    </svg>
                    Actualizar
                </button>
            </div>
        </div>

        <div v-if="exitoMsg" class="notif notif-exito">{{ exitoMsg }}</div>
        <div v-if="errorMsg" class="notif notif-error">{{ errorMsg }}</div>

        <AsignacionHistorialView
            v-if="vistaActiva === 'historial'"
            ref="historialRef"
            @exito="onExito"
            @error="onError"
            @cancelada="onCancelada"
        />

        <AsignacionFormView
            v-else
            ref="formRef"
            @exito="onExito"
            @error="onError"
            @asignacion-creada="onAsignacionCreada"
        />
    </div>
</template>

<style scoped>
.asig-page {
    padding: 28px 32px;
    max-width: 1400px;
    margin: 0 auto;
    font-family: inherit;
}

.asig-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 24px;
}

.asig-title {
    font-size: 1.35rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.asig-header-actions {
    display: flex;
    gap: 10px;
}

.btn-historial {
    padding: 8px 16px;
    background: #f3f4f6;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: background .15s;
    font-family: inherit;
}

.btn-historial:hover { background: #e5e7eb; }

.btn-historial.btn-activo {
    background: #1a3a2a;
    color: #fff;
    border-color: #1a3a2a;
}

.btn-actualizar {
    display: flex;
    align-items: center;
    gap: 6px;
    padding: 8px 14px;
    background: #fff;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    transition: background .15s;
    font-family: inherit;
}

.btn-actualizar:hover { background: #f9fafb; }

.notif {
    padding: 12px 16px;
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

@media (max-width: 700px) {
    .asig-page { padding: 16px; }

    .asig-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
    }
}
</style>