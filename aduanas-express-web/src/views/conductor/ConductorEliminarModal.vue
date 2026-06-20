<script setup>
import { ref } from 'vue'
import { eliminarConductor } from '@/services/conductorService'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    conductor: { type: Object, default: null }
})

const emit = defineEmits(['update:modelValue', 'eliminado'])

const loading = ref(false)
const error = ref('')

function cancelar() {
    if (loading.value) return
    error.value = ''
    emit('update:modelValue', false)
}

async function confirmar() {
    if (!props.conductor?.id) return
    loading.value = true
    error.value = ''
    try {
        await eliminarConductor(props.conductor.id)
        emit('eliminado', props.conductor.id)
        emit('update:modelValue', false)
    } catch (e) {
        error.value = e?.response?.data?.message || e?.message || 'Error al eliminar el conductor.'
    } finally {
        loading.value = false
    }
}
</script>

<template>
    <Teleport to="body">
        <div v-if="modelValue" class="modal-overlay" @click.self="cancelar">
            <div class="modal">
                <div class="modal-icon">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z" />
                        <line x1="12" y1="9" x2="12" y2="13" /><line x1="12" y1="17" x2="12.01" y2="17" />
                    </svg>
                </div>

                <h2 class="modal-titulo">¿Eliminar este conductor?</h2>
                <p class="modal-desc">
                    Estás a punto de eliminar
                    <strong v-if="conductor">{{ conductor.nombre }} {{ conductor.apellido }} — {{ conductor.cedula }}</strong>.
                    Esta acción es permanente y no podrá deshacerse.
                </p>

                <div v-if="error" class="modal-alert">
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
                    </svg>
                    {{ error }}
                </div>

                <div class="modal-acciones">
                    <button class="btn-cancelar-modal" @click="cancelar" :disabled="loading">Cancelar</button>
                    <button class="btn-confirmar-modal" @click="confirmar" :disabled="loading">
                        <span v-if="loading" class="btn-spinner"></span>
                        {{ loading ? 'Eliminando…' : 'Confirmar eliminación' }}
                    </button>
                </div>
            </div>
        </div>
    </Teleport>
</template>

<style scoped>
.modal-overlay {
    position: fixed; inset: 0;
    background: rgba(0,0,0,.45);
    display: flex; align-items: center; justify-content: center;
    z-index: 100;
    font-family: 'Inter', 'Segoe UI', system-ui, sans-serif;
}
.modal {
    background: #fff; border-radius: 14px; padding: 32px;
    width: 420px; max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0,0,0,.2); text-align: center;
}
.modal-icon {
    width: 52px; height: 52px; background: #fef3c7;
    border-radius: 50%; display: flex; align-items: center;
    justify-content: center; color: #92400e; margin: 0 auto 16px;
}
.modal-titulo { font-size: 1.05rem; font-weight: 700; color: #111827; margin: 0 0 8px; }
.modal-desc { font-size: 0.875rem; color: #4b5563; line-height: 1.55; margin: 0 0 20px; }
.modal-desc strong { color: #111827; }
.modal-alert {
    display: flex; align-items: center; gap: 10px;
    background: #fef2f2; border: 1px solid #fecaca;
    border-radius: 8px; padding: 10px 14px;
    font-size: 0.8rem; color: #991b1b; margin: 0 0 16px; text-align: left;
}
.modal-acciones { display: flex; gap: 10px; justify-content: center; }
.btn-cancelar-modal {
    padding: 9px 20px; background: #f3f4f6; border: none;
    border-radius: 8px; font-size: 0.875rem; font-weight: 500;
    color: #374151; cursor: pointer; font-family: inherit; transition: background 0.15s;
}
.btn-cancelar-modal:hover:not(:disabled) { background: #e5e7eb; }
.btn-cancelar-modal:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-confirmar-modal {
    display: inline-flex; align-items: center; gap: 7px;
    padding: 9px 20px; background: #dc2626; border: none;
    border-radius: 8px; font-size: 0.875rem; font-weight: 600;
    color: #fff; cursor: pointer; font-family: inherit;
    transition: background 0.15s, opacity 0.15s;
}
.btn-confirmar-modal:hover:not(:disabled) { background: #b91c1c; }
.btn-confirmar-modal:disabled { opacity: 0.6; cursor: not-allowed; }
.btn-spinner {
    width: 13px; height: 13px;
    border: 2px solid rgba(255,255,255,.4); border-top-color: #fff;
    border-radius: 50%; animation: spin 0.65s linear infinite; flex-shrink: 0;
}
@keyframes spin { to { transform: rotate(360deg); } }
@media (max-width: 480px) { .modal { width: 92vw; padding: 24px; } }
</style>
