<script setup>
import { computed } from 'vue'

const props = defineProps({
    modelValue: { type: Boolean, default: false },
    usuario:    { type: Object,  default: null },
    modo:       { type: String,  default: 'eliminar' }, // 'eliminar' | 'desactivar'
    procesando: { type: Boolean, default: false },
})

const emit = defineEmits(['update:modelValue', 'confirmar'])

const esEliminar = computed(() => props.modo === 'eliminar')

const titulo = computed(() => esEliminar.value ? 'Eliminar usuario' : 'Desactivar usuario')

const descripcion = computed(() => {
    if (!props.usuario) return ''
    const nombre = `${props.usuario.nombre} ${props.usuario.apellido}`
    return esEliminar.value
        ? `¿Estás seguro de que deseas eliminar a ${nombre}? Esta acción no se puede deshacer.`
        : `${nombre} no podrá iniciar sesión mientras esté desactivado. Podrás reactivar la cuenta cuando quieras.`
})

const textoBoton = computed(() => {
    if (props.procesando) return esEliminar.value ? 'Eliminando…' : 'Desactivando…'
    return esEliminar.value ? 'Eliminar' : 'Desactivar'
})

function cerrar() {
    if (props.procesando) return
    emit('update:modelValue', false)
}

function confirmar() {
    emit('confirmar', props.usuario)
}
</script>

<template>
    <div v-if="modelValue && usuario" class="modal-overlay" @click.self="cerrar">
        <div class="modal">
            <div class="modal-icon" :class="esEliminar ? 'icon-peligro' : 'icon-advertencia'">
                <svg v-if="esEliminar" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <polyline points="3 6 5 6 21 6" /><path d="M19 6l-1 14a2 2 0 0 1-2 2H8a2 2 0 0 1-2-2L5 6" />
                    <path d="M10 11v6" /><path d="M14 11v6" /><path d="M9 6V4a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2" />
                </svg>
                <svg v-else width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <circle cx="12" cy="12" r="10" /><line x1="4.93" y1="4.93" x2="19.07" y2="19.07" />
                </svg>
            </div>

            <h3 class="modal-titulo">{{ titulo }}</h3>
            <p class="modal-desc">{{ descripcion }}</p>

            <div class="modal-acciones">
                <button class="btn-cancelar" @click="cerrar" :disabled="procesando">Cancelar</button>
                <button
                    class="btn-confirmar"
                    :class="esEliminar ? 'btn-peligro' : 'btn-advertencia'"
                    @click="confirmar"
                    :disabled="procesando"
                >
                    <span v-if="procesando" class="btn-spinner"></span>
                    {{ textoBoton }}
                </button>
            </div>
        </div>
    </div>
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
    padding: 28px;
    width: 420px;
    max-width: 90vw;
    box-shadow: 0 20px 60px rgba(0, 0, 0, .2);
}

.modal-icon {
    width: 44px;
    height: 44px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 16px;
}

.icon-peligro      { background: #fee2e2; color: #dc2626; }
.icon-advertencia  { background: #fef3c7; color: #b45309; }

.modal-titulo {
    font-size: 1.05rem;
    font-weight: 700;
    color: #111827;
    margin: 0 0 8px;
}

.modal-desc {
    font-size: 0.875rem;
    color: #4b5563;
    line-height: 1.55;
    margin: 0 0 22px;
}

.modal-acciones { display: flex; gap: 10px; justify-content: flex-end; }

.btn-cancelar {
    padding: 9px 18px;
    background: #f3f4f6;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    font-family: inherit;
}
.btn-cancelar:disabled { opacity: 0.6; cursor: default; }

.btn-confirmar {
    display: inline-flex;
    align-items: center;
    gap: 7px;
    padding: 9px 18px;
    border: none;
    border-radius: 8px;
    font-size: 0.875rem;
    font-weight: 600;
    color: #fff;
    cursor: pointer;
    font-family: inherit;
    transition: background 0.15s;
}
.btn-confirmar:disabled { opacity: 0.6; cursor: default; }

.btn-peligro      { background: #dc2626; }
.btn-peligro:hover:not(:disabled)      { background: #b91c1c; }

.btn-advertencia  { background: #b45309; }
.btn-advertencia:hover:not(:disabled)  { background: #92400e; }

.btn-spinner {
    width: 13px; height: 13px;
    border: 2px solid rgba(255, 255, 255, .4);
    border-top-color: #fff;
    border-radius: 50%;
    animation: spin 0.65s linear infinite;
    flex-shrink: 0;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>