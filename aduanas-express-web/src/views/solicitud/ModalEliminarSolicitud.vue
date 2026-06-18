<script setup>
const props = defineProps({
    show: { type: Boolean, default: false },
    solicitud: { type: Object, default: null }
})

const emit = defineEmits(['close', 'confirmar'])

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}
</script>

<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay" @click.self="emit('close')">
            <div class="modal">
                <div class="modal-icon">
                    <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#dc2626" stroke-width="2">
                        <polyline points="3 6 5 6 21 6" />
                        <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                        <path d="M10 11v6M14 11v6" />
                        <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
                    </svg>
                </div>
                <h2 class="modal-titulo">¿Eliminar solicitud?</h2>
                <p class="modal-desc">
                    Estás a punto de eliminar la solicitud
                    <strong>{{ formatNumero(solicitud?.id) }}</strong>
                    de <strong>{{ solicitud?.areaSolicitante }}</strong>.
                    Esta acción no se puede deshacer.
                </p>
                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="emit('close')">Cancelar</button>
                    <button class="btn-confirmar-modal" @click="emit('confirmar')">Sí, eliminar</button>
                </div>
            </div>
        </div>
    </Teleport>
</template>

<style scoped>
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
    text-align: center;
}

.modal-icon {
    width: 56px;
    height: 56px;
    background: #fef2f2;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto 16px;
}

.modal-titulo {
    font-size: 1.1rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 10px;
}

.modal-desc {
    font-size: .9rem;
    color: #4b5563;
    line-height: 1.6;
    margin: 0 0 28px;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    justify-content: center;
}

.btn-cancelar-modal {
    padding: 9px 24px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
}

.btn-cancelar-modal:hover { background: #e5e7eb; }

.btn-confirmar-modal {
    padding: 9px 24px;
    background: #dc2626;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
}

.btn-confirmar-modal:hover { background: #b91c1c; }
</style>