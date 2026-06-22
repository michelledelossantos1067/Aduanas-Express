import { ref, readonly } from 'vue'

const STORAGE_KEY = 'reporte_config'

const DEFAULTS = {
    estilo:        'light',
    colorKey:      'verde',
    primary:       '#1C3829',
    accent:        '#8A6A2E',
    light:         '#EBF2EE',
    customPrimary: '#1C3829',
    customAccent:  '#8A6A2E',
}

function leer() {
    try {
        const raw = localStorage.getItem(STORAGE_KEY)
        return raw ? { ...DEFAULTS, ...JSON.parse(raw) } : { ...DEFAULTS }
    } catch {
        return { ...DEFAULTS }
    }
}

const config = ref(leer())

export function useReporteConfig() {

    function aplicar(nuevaCfg) {
        config.value = { ...config.value, ...nuevaCfg }
    }
    function queryParams() {
        return {
            estilo:       config.value.estilo,
            colorPrimary: config.value.primary,
            colorAccent:  config.value.accent,
        }
    }

    return {
        config: readonly(config),
        aplicar,
        queryParams,
    }
}