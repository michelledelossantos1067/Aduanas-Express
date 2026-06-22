<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    registro: { type: Object, default: null },
    guardando: { type: Boolean, default: false },
})
const emit = defineEmits(['update:modelValue', 'confirmar'])

const hoy = new Date().toISOString().substring(0, 10)

const fechaRealizada = ref(hoy)
const kilometrajeFinal = ref('')
const huboCambioCosto = ref(false)
const costoFinal = ref('')
const reporteFinal = ref('')
const error = ref('')

watch(() => props.modelValue, (abierto) => {
    if (abierto && props.registro) {
        fechaRealizada.value = hoy
        kilometrajeFinal.value = props.registro.kilometraje ?? ''
        huboCambioCosto.value = false
        costoFinal.value = props.registro.costo ?? ''
        reporteFinal.value = ''
        error.value = ''
    }
})

function cerrar() {
    if (props.guardando) return
    emit('update:modelValue', false)
}

function confirmar() {
    if (!fechaRealizada.value) {
        error.value = 'La fecha realizada es obligatoria.'
        return
    }
    error.value = ''
    emit('confirmar', {
        fechaRealizada: fechaRealizada.value,
        kilometraje: kilometrajeFinal.value !== '' ? Number(kilometrajeFinal.value) : null,
        costo: huboCambioCosto.value ? Number(costoFinal.value || 0) : null,
        reporteFinal: reporteFinal.value,
    })
}
</script>

<template>
    <div v-if="modelValue" class="modal-overlay" @click.self="cerrar">
        <div class="modal-box-form">
            <h3 class="modal-titulo">Finalizar mantenimiento</h3>
            <p class="modal-desc">
                Vehículo <strong>{{ registro?.vehiculoPlaca }}</strong> — {{ registro?.tipo }}: {{ registro?.descripcion
                }}
            </p>

            <div v-if="error" class="notif notif-error">{{ error }}</div>

            <div class="campo">
                <label>Fecha en que se realizó <span class="req">*</span></label>
                <input type="date" v-model="fechaRealizada" :max="hoy" />
            </div>

            <div class="campo">
                <label>Kilometraje al momento de la entrega</label>
                <input type="number" min="0" v-model="kilometrajeFinal" placeholder="Opcional" />
            </div>

            <div class="campo campo-checkbox">
                <label class="check-label">
                    <input type="checkbox" v-model="huboCambioCosto" />
                    Hubo cambios en el costo respecto al estimado
                </label>
            </div>

            <div class="campo" v-if="huboCambioCosto">
                <label>Costo final</label>
                <input type="number" min="0" step="0.01" v-model="costoFinal" placeholder="0.00" />
            </div>

            <div class="campo">
                <label>Reporte final <span class="opcional">(qué se encontró, qué se hizo además de lo
                        previsto)</span></label>
                <textarea v-model="reporteFinal" rows="3" placeholder="Opcional"></textarea>
            </div>

            <div class="modal-acciones">
                <button class="btn-cancelar" @click="cerrar" :disabled="guardando">Cancelar</button>
                <button class="btn-finalizar-confirmar" @click="confirmar" :disabled="guardando">
                    <span v-if="guardando" class="spinner-btn"></span>
                    {{ guardando ? 'Guardando...' : 'Finalizar mantenimiento' }}
                </button>
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
    padding: 20px;
}

.modal-box-form {
    background: #fff;
    border-radius: 16px;
    padding: 26px 26px 22px;
    max-width: 440px;
    width: 100%;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 8px 30px rgba(0, 0, 0, .15);
}

.modal-titulo {
    font-size: 1.05rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 4px;
}

.modal-desc {
    font-size: .82rem;
    color: #6b7280;
    margin: 0 0 16px;
    line-height: 1.4;
}

.notif {
    padding: 10px 14px;
    border-radius: 8px;
    font-size: .8rem;
    margin-bottom: 14px;
}

.notif-error {
    background: #fee2e2;
    color: #991b1b;
    border: 1px solid #fca5a5;
}

.campo {
    display: flex;
    flex-direction: column;
    gap: 5px;
    margin-bottom: 14px;
}

.campo label {
    font-size: .78rem;
    font-weight: 600;
    color: #374151;
}

.req {
    color: #dc2626;
}

.opcional {
    font-weight: 400;
    color: #9ca3af;
}

.campo input,
.campo textarea {
    padding: 9px 12px;
    background: #f9fafb;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: .875rem;
    color: #111827;
    font-family: inherit;
    outline: none;
    transition: border-color .15s;
}

.campo input:focus,
.campo textarea:focus {
    border-color: #1a3a2a;
    background: #fff;
}

.campo textarea {
    resize: vertical;
}

.campo-checkbox {
    flex-direction: row;
    align-items: center;
}

.check-label {
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
    font-size: .82rem !important;
    font-weight: 500 !important;
    color: #374151;
}

.check-label input[type="checkbox"] {
    width: 16px;
    height: 16px;
    cursor: pointer;
}

.modal-acciones {
    display: flex;
    gap: 10px;
    margin-top: 6px;
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

.btn-cancelar:hover:not(:disabled) {
    background: #f3f4f6;
}

.btn-cancelar:disabled {
    opacity: .5;
    cursor: default;
}

.btn-finalizar-confirmar {
    flex: 1.4;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 7px;
    padding: 10px 0;
    background: #1a3a2a;
    border: none;
    border-radius: 9px;
    font-size: .875rem;
    font-weight: 700;
    color: #fff;
    cursor: pointer;
    transition: background .15s;
}

.btn-finalizar-confirmar:hover:not(:disabled) {
    background: #14532d;
}

.btn-finalizar-confirmar:disabled {
    opacity: .6;
    cursor: default;
}

.spinner-btn {
    width: 14px;
    height: 14px;
    border: 2px solid rgba(255, 255, 255, .4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin .75s linear infinite;
}

@keyframes spin {
    to {
        transform: rotate(360deg);
    }
}
</style>