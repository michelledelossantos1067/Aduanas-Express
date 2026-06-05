import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useNavbarStore = defineStore('navbar', () => {
  const titulo = ref('Dashboard')
  const acciones = ref<{ label: string; accion: string }[]>([])
  const mostrarBuscador = ref(false)
  const mostrarNotificaciones = ref(false)

  function setTitulo(nuevoTitulo: string) {
    titulo.value = nuevoTitulo
  }

  function setAcciones(nuevasAcciones: { label: string; accion: string }[]) {
    acciones.value = nuevasAcciones
  }

  function setExtras(buscador: boolean, notificaciones: boolean) {
    mostrarBuscador.value = buscador
    mostrarNotificaciones.value = notificaciones
  }
  

  return { titulo, acciones, mostrarBuscador, mostrarNotificaciones, setTitulo, setAcciones, setExtras }
})