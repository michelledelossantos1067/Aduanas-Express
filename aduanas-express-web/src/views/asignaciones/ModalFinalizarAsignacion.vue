<script setup>
import { formatNumero } from './composables/useAsignacionHelpers'

defineProps({
    show: { type: Boolean, default: false },
    asignacion: { type: Object, default: null },
    loading: { type: Boolean, default: false },
})

const emit = defineEmits(['close', 'confirmar'])
</script>

<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay" @click.self="emit('close')">
            <div class="modal-box">
                <div class="modal-icon modal-icon-verde">
                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="20 6 9 17 4 12"/>
                    </svg>
                </div>
                <h3 class="modal-titulo">Finalizar viaje</h3>
                <p class="modal-desc">
                    Asignación {{ formatNumero(asignacion?.id) }} —
                    {{ asignacion?.solicitud?.destino ?? '' }}.<br>
                    Esta acción no se puede deshacer.
                </p>
                <div class="modal-acciones">
                    <button class="btn-modal-sec" @click="emit('close')">Cancelar</button>
                    <button class="btn-modal-verde" :disabled="loading" @click="emit('confirmar')">
                        {{ loading ? 'Finalizando…' : 'Sí, finalizar viaje' }}
                    </button>
                </div>
            </div>
        </div>
    </Teleport>
</template>

<style scoped>
.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,.45);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 200;
}

.modal-box {
    background: #fff;
    border-radius: 14px;
    padding: 32px;
    width: 440px;
    max-width: 92vw;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
}

.modal-icon {
    width: 48px;
    height: 48px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto 16px;
}

.modal-icon-verde { background: #d1fae5; color: #065f46; }

.modal-titulo {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    text-align: center;
    margin: 0 0 6px;
}

.modal-desc {
    font-size: .875rem;
    color: #6b7280;
    text-align: center;
    margin: 0 0 20px;
    line-height: 1.5;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: flex-end;
}

.btn-modal-sec {
    padding: 9px 18px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    font-family: inherit;
    transition: background .15s;
}

.btn-modal-sec:hover { background: #e5e7eb; }

.btn-modal-verde {
    padding: 9px 18px;
    background: #1a3a2a;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    font-family: inherit;
    transition: background .15s;
}

.btn-modal-verde:hover:not(:disabled) { background: #14532d; }
.btn-modal-verde:disabled { opacity: .5; cursor: default; }
</style>
