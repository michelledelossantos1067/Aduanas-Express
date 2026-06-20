<script setup>
import { formatNumero } from './composables/useAsignacionHelpers'

defineProps({
    show: { type: Boolean, default: false },
    asignacion: { type: Object, default: null },
    loading: { type: Boolean, default: false },
})

const motivo = defineModel('motivo', { type: String, default: '' })

const emit = defineEmits(['close', 'confirmar'])
</script>

<template>
    <Teleport to="body">
        <div v-if="show" class="modal-overlay" @click.self="emit('close')">
            <div class="modal-box">
                <div class="modal-icon modal-icon-rojo">
                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                    </svg>
                </div>
                <h3 class="modal-titulo">Cancelar asignación</h3>
                <p class="modal-desc">
                    Asignación {{ formatNumero(asignacion?.id) }} —
                    {{ asignacion?.solicitud?.destino ?? '' }}.
                    El vehículo y el conductor volverán a estar disponibles.
                </p>
                <div class="modal-field">
                    <label class="modal-label">Motivo de cancelación</label>
                    <textarea
                        v-model="motivo"
                        rows="3"
                        placeholder="Describe el motivo..."
                        class="modal-textarea"
                    ></textarea>
                </div>
                <div class="modal-acciones">
                    <button class="btn-modal-sec" @click="emit('close')">Volver</button>
                    <button class="btn-modal-rojo" :disabled="loading" @click="emit('confirmar')">
                        {{ loading ? 'Cancelando…' : 'Confirmar cancelación' }}
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

.modal-icon-rojo { background: #fee2e2; color: #991b1b; }

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

.modal-field {
    display: flex;
    flex-direction: column;
    gap: 6px;
    margin-bottom: 20px;
}

.modal-label {
    font-size: .78rem;
    font-weight: 700;
    color: #374151;
    text-transform: uppercase;
    letter-spacing: .05em;
}

.modal-textarea {
    width: 100%;
    box-sizing: border-box;
    padding: 10px 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .875rem;
    color: #111827;
    font-family: inherit;
    outline: none;
    resize: vertical;
    min-height: 80px;
}

.modal-textarea:focus { border-color: #1a3a2a; box-shadow: 0 0 0 3px rgba(26,58,42,.1); }

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

.btn-modal-rojo {
    padding: 9px 18px;
    background: #dc2626;
    border: none;
    border-radius: 8px;
    font-size: .875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    font-family: inherit;
    transition: background .15s;
}

.btn-modal-rojo:hover:not(:disabled) { background: #b91c1c; }
.btn-modal-rojo:disabled { opacity: .5; cursor: default; }
</style>
