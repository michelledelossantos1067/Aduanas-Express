<script setup>
import { computed } from 'vue'

const props = defineProps({
    viajes: { type: Array, required: true },
    fecha: { type: Date, required: true }
})

const emit = defineEmits(['seleccionarViaje'])

// Generamos horas de 06:00 a 22:00
const horas = Array.from({ length: 17 }, (_, i) => i + 6)

function obtenerViajesEnHora(hora) {
    return props.viajes.filter(v => parseInt(v.horaInicio.split(':')[0]) === hora)
}

function formatearHora(h) { return `${String(h).padStart(2, '0')}:00` }
</script>

<template>
    <div class="dia-view-container">
        <div class="timeline">
            <div v-for="hora in horas" :key="hora" class="timeline-row">
                <div class="time-label">{{ formatearHora(hora) }}</div>
                <div class="time-slot">
                    <div v-for="viaje in obtenerViajesEnHora(hora)" 
                         :key="viaje.id" 
                         class="viaje-item" 
                         @click="emit('seleccionarViaje', viaje)">
                        <span class="v-title">{{ viaje.titulo }}</span>
                        <span class="v-meta">{{ viaje.conductor }} • {{ viaje.vehiculo }}</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.dia-view-container {
    padding: 20px;
    background: #fff;
    border-radius: 14px;
}
.timeline { display: flex; flex-direction: column; }
.timeline-row { display: flex; border-bottom: 1px solid #f3f4f6; min-height: 60px; }
.time-label { width: 60px; font-size: 0.75rem; color: #9ca3af; font-weight: 600; padding: 10px 0; }
.time-slot { flex: 1; padding: 4px; display: flex; flex-direction: column; gap: 4px; }
.viaje-item { 
    background: #e0f2fe; border-left: 4px solid #0284c7; padding: 6px 10px; 
    border-radius: 4px; cursor: pointer; transition: 0.2s;
}
.viaje-item:hover { background: #bae6fd; }
.v-title { display: block; font-size: 0.85rem; font-weight: 700; color: #0c4a6e; }
.v-meta { font-size: 0.7rem; color: #0369a1; }
</style>