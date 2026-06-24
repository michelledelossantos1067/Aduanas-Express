import { ref } from 'vue'

const currentView = ref('month') 

export function useCalendar() {
  const setView = (view) => {
    if (['month', 'week', 'day'].includes(view)) {
      currentView.value = view
    }
  }

  return {
    currentView,
    setView
  }
}