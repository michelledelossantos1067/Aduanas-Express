<template>
    <div v-if="modelValue" class="modal-overlay" @click.self="$emit('update:modelValue', false)">
        <div class="modal-sp">
            <div class="modal-sp-icon">
                <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="10"/>
                    <line x1="12" y1="8" x2="12" y2="12"/>
                    <line x1="12" y1="16" x2="12.01" y2="16"/>
                </svg>
            </div>
            <h3 class="modal-sp-titulo">Acción no permitida</h3>
            <p class="modal-sp-desc">
                No tienes permisos para <strong>{{ accion }}</strong>.<br/>
                Solo los <strong>Administradores</strong> pueden realizar esta acción.
            </p>
            <button class="modal-sp-btn" @click="$emit('update:modelValue', false)">Entendido</button>
        </div>
    </div>
</template>

<script setup>
defineProps({
    modelValue: Boolean,
    accion: { type: String, default: 'realizar esta acción' }
})
defineEmits(['update:modelValue'])
</script>

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
.modal-sp {
    background: #fff;
    border-radius: 16px;
    padding: 36px 32px;
    width: 400px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 12px;
    text-align: center;
}
.modal-sp-icon {
    width: 56px;
    height: 56px;
    border-radius: 50%;
    background: #fef2f2;
    color: #dc2626;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 4px;
}
.modal-sp-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}
.modal-sp-desc {
    font-size: 0.875rem;
    color: #4b5563;
    line-height: 1.6;
    margin: 0;
}
.modal-sp-btn {
    margin-top: 8px;
    padding: 9px 28px;
    background: #1a3a2a;
    color: #fff;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    cursor: pointer;
    font-family: inherit;
    transition: background 0.15s;
}
.modal-sp-btn:hover { background: #142d20; }
</style>