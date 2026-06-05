import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useNavbarStore = defineStore('navbar', () => {
  const titulo = ref('Dashboard')
  const acciones = ref<{ label: string; accion: string }[]>([])

  function setTitulo(nuevoTitulo: string) {
    titulo.value = nuevoTitulo
  }

  function setAcciones(nuevasAcciones: { label: string; accion: string }[]) {
    acciones.value = nuevasAcciones
  }

  return { titulo, acciones, setTitulo, setAcciones }
})