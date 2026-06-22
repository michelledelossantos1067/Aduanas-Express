<script setup>
const props = defineProps({
    modelValue: { type: Boolean, default: false },
    registro: { type: Object, default: null },
})
const emit = defineEmits(['update:modelValue', 'confirmar'])

function formatNumero(id) {
    return `#${String(id).padStart(4, '0')}`
}

function cerrar() {
    emit('update:modelValue', false)
}

function confirmar() {
    emit('update:modelValue', false)
    emit('confirmar', props.registro)
}
</script>

<template>
    <div v-if="modelValue" class="modal-overlay" @click.self="cerrar">
        <div class="modal-box">
            <div class="modal-icon">
                <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="#dc2626" stroke-width="2">
                    <polyline points="3 6 5 6 21 6" />
                    <path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                    <path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
                </svg>
            </div>
            <h3 class="modal-titulo">¿Eliminar registro?</h3>
            <p class="modal-desc">
                Estás a punto de eliminar el mantenimiento
                <strong>{{ formatNumero(registro?.id) }}</strong>
                del vehículo <strong>{{ registro?.vehiculoPlaca }}</strong>.
                Esta acción no se puede deshacer.
            </p>
            <div class="modal-acciones">
                <button class="btn-cancelar" @click="cerrar">Cancelar</button>
                <button class="btn-eliminar-confirmar" @click="confirmar">Sí, eliminar</button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay {
    position: fixed;
    inset: 0;
    background: rgba(0, 0, 0, .35);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 999;
}

.modal-box {
    background: #fff;
    border-radius: 16px;
    padding: 28px 28px 24px;
    max-width: 400px;
    width: 90%;
    box-shadow: 0 8px 30px rgba(0, 0, 0, .15);
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    gap: 10px;
}

.modal-icon {
    width: 54px;
    height: 54px;
    background: #fee2e2;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 4px;
}

.modal-titulo {
    font-size: 1rem;
    font-weight: 700;
    color: #111827;
    margin: 0;
}

.modal-desc {
    font-size: .875rem;
    color: #6b7280;
    margin: 0;
    line-height: 1.5;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    margin-top: 8px;
    width: 100%;
}

.btn-cancelar {
    flex: 1;
    padding: 10px 0;
    background: transparent;
    border: 1.5px solid #e5e7eb;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 500;
    color: #6b7280;
    cursor: pointer;
    transition: background .15s;
}

.btn-cancelar:hover {
    background: #f3f4f6;
}

.btn-eliminar-confirmar {
    flex: 1;
    padding: 10px 0;
    background: #dc2626;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-eliminar-confirmar:hover {
    background: #b91c1c;
}
</style>
