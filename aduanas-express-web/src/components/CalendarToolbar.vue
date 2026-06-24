<template>
  <div class="calendar-toolbar">
    <button 
      :class="{ active: currentView === 'month' }" 
      @click="setView('month')"
    >
      Mes
    </button>
    <button 
      :class="{ active: currentView === 'week' }" 
      @click="setView('week')"
    >
      Semana
    </button>
    <button 
      :class="{ active: currentView === 'day' }" 
      @click="setView('day')"
    >
      Día
    </button>
  </div>
</template>

<script setup>
import { useCalendar } from '@/composables/useCalendar'

const { currentView, setView } = useCalendar()
</script>

<style scoped>
.calendar-toolbar button {
  padding: 8px 16px;
  margin-right: 5px;
  border: 1px solid #ccc;
  background: white;
  cursor: pointer;
}
.calendar-toolbar button.active {
  background-color: #42b883;
  color: white;
  border-color: #42b883;
}
</style>