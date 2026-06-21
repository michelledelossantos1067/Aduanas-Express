<script setup>
import { ref, watch } from 'vue'

const props  = defineProps({ modelValue: Boolean })
const emit   = defineEmits(['update:modelValue', 'aplicar'])

const STORAGE_KEY = 'reporte_config'

const ESTILOS = [
    { key: 'light',   label: 'Light',   desc: 'Encabezado oscuro, tabla limpia' },
    { key: 'boxed',   label: 'Boxed',   desc: 'KPIs en tarjetas con borde' },
    { key: 'bold',    label: 'Bold',    desc: 'KPIs de color sólido, alto contraste' },
    { key: 'minimal', label: 'Minimal', desc: 'Sin color en la tabla, máxima legibilidad' },
]

const COLORES = [
    { key: 'verde',   label: 'Verde institucional', primary: '#1C3829', accent: '#8A6A2E', light: '#EBF2EE' },
    { key: 'azul',    label: 'Azul marino',         primary: '#1E3A5F', accent: '#C9A84C', light: '#E8EEF6' },
    { key: 'purpura', label: 'Púrpura',             primary: '#3B2261', accent: '#B07D3A', light: '#F0EBF8' },
    { key: 'slate',   label: 'Slate',               primary: '#1F2937', accent: '#6B7280', light: '#F1F2F4' },
    { key: 'custom',  label: 'Personalizado',       primary: '',        accent: '',        light: '' },
]

function cargarGuardado() {
    try {
        const raw = localStorage.getItem(STORAGE_KEY)
        return raw ? JSON.parse(raw) : null
    } catch { return null }
}

const guardado    = cargarGuardado()
const estiloSel   = ref(guardado?.estilo   ?? 'light')
const colorSel    = ref(guardado?.colorKey ?? 'verde')
const customPri   = ref(guardado?.customPrimary ?? '#1C3829')
const customAcc   = ref(guardado?.customAccent  ?? '#8A6A2E')

function colorActivo() {
    if (colorSel.value === 'custom') {
        return { primary: customPri.value, accent: customAcc.value, light: '#F4F6F8' }
    }
    return COLORES.find(c => c.key === colorSel.value) ?? COLORES[0]
}

function guardar() {
    const cfg = {
        estilo:        estiloSel.value,
        colorKey:      colorSel.value,
        customPrimary: customPri.value,
        customAccent:  customAcc.value,
        ...colorActivo(),
    }
    localStorage.setItem(STORAGE_KEY, JSON.stringify(cfg))
    emit('aplicar', cfg)
    emit('update:modelValue', false)
}

function cancelar() { emit('update:modelValue', false) }
</script>

<template>
    <Teleport to="body">
        <div v-if="modelValue" class="cfg-overlay" @click.self="cancelar">
            <div class="cfg-modal">

                <!-- Cabecera -->
                <div class="cfg-head">
                    <div>
                        <p class="cfg-head-sub">AduanasExpress</p>
                        <h2 class="cfg-head-title">Configuración del documento</h2>
                    </div>
                    <button class="cfg-close" @click="cancelar" aria-label="Cerrar">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                        </svg>
                    </button>
                </div>

                <div class="cfg-body">

                    <!-- ── Estilo ── -->
                    <section class="cfg-section">
                        <h3 class="cfg-section-title">Estilo del reporte</h3>
                        <div class="estilo-grid">
                            <label
                                v-for="e in ESTILOS"
                                :key="e.key"
                                class="estilo-card"
                                :class="{ 'estilo-card--on': estiloSel === e.key }"
                            >
                                <input type="radio" :value="e.key" v-model="estiloSel" class="sr-only" />

                                <!-- Miniatura del estilo -->
                                <div class="estilo-thumb">
                                    <!-- Light -->
                                    <template v-if="e.key === 'light'">
                                        <div class="th-header" :style="{ background: colorActivo().primary }"></div>
                                        <div class="th-bar" :style="{ background: colorActivo().accent }"></div>
                                        <div class="th-kpis">
                                            <div v-for="i in 4" :key="i" class="th-kpi th-kpi--border"
                                                :style="{ borderColor: colorActivo().primary }"></div>
                                        </div>
                                        <div class="th-table">
                                            <div class="th-thead" :style="{ background: '#E5E7EB' }"></div>
                                            <div v-for="i in 3" :key="i" class="th-trow"
                                                :style="{ background: i%2===0 ? '#F4F6F8':'#fff' }"></div>
                                        </div>
                                    </template>
                                    <!-- Boxed -->
                                    <template v-if="e.key === 'boxed'">
                                        <div class="th-header" :style="{ background: colorActivo().primary }"></div>
                                        <div class="th-bar" :style="{ background: colorActivo().accent }"></div>
                                        <div class="th-kpis">
                                            <div v-for="i in 4" :key="i" class="th-kpi th-kpi--box"
                                                :style="{ borderColor: '#D1D9E0' }"></div>
                                        </div>
                                        <div class="th-table">
                                            <div class="th-thead" :style="{ background: '#E5E7EB' }"></div>
                                            <div v-for="i in 3" :key="i" class="th-trow"
                                                :style="{ background: i%2===0 ? '#F4F6F8':'#fff' }"></div>
                                        </div>
                                    </template>
                                    <!-- Bold -->
                                    <template v-if="e.key === 'bold'">
                                        <div class="th-header" :style="{ background: colorActivo().primary }"></div>
                                        <div class="th-bar" :style="{ background: colorActivo().accent }"></div>
                                        <div class="th-kpis">
                                            <div v-for="i in 4" :key="i" class="th-kpi th-kpi--solid"
                                                :style="{ background: colorActivo().primary }"></div>
                                        </div>
                                        <div class="th-table">
                                            <div class="th-thead" :style="{ background: colorActivo().primary }"></div>
                                            <div v-for="i in 3" :key="i" class="th-trow"
                                                :style="{ background: i%2===0 ? '#F4F6F8':'#fff' }"></div>
                                        </div>
                                    </template>
                                    <!-- Minimal -->
                                    <template v-if="e.key === 'minimal'">
                                        <div class="th-header th-header--light"
                                            :style="{ background: '#F4F6F8', borderBottom: `2px solid ${colorActivo().primary}` }"></div>
                                        <div class="th-kpis th-kpis--minimal">
                                            <div v-for="i in 4" :key="i" class="th-kpi th-kpi--minimal"
                                                :style="{ borderTop: `2px solid ${colorActivo().primary}` }"></div>
                                        </div>
                                        <div class="th-table">
                                            <div class="th-thead th-thead--minimal"></div>
                                            <div v-for="i in 3" :key="i" class="th-trow"
                                                :style="{ background: i%2!==0 ? colorActivo().light:'#fff' }"></div>
                                        </div>
                                    </template>
                                </div>

                                <div class="estilo-info">
                                    <span class="estilo-name">{{ e.label }}</span>
                                    <span class="estilo-desc">{{ e.desc }}</span>
                                </div>
                                <div class="estilo-check" v-if="estiloSel === e.key">
                                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                                        <polyline points="20 6 9 17 4 12"/>
                                    </svg>
                                </div>
                            </label>
                        </div>
                    </section>

                    <!-- ── Color ── -->
                    <section class="cfg-section">
                        <h3 class="cfg-section-title">Color institucional</h3>
                        <div class="color-lista">
                            <label
                                v-for="c in COLORES"
                                :key="c.key"
                                class="color-row"
                                :class="{ 'color-row--on': colorSel === c.key }"
                            >
                                <input type="radio" :value="c.key" v-model="colorSel" class="sr-only" />
                                <span
                                    class="color-dot"
                                    :style="c.key !== 'custom'
                                        ? { background: c.primary }
                                        : { background: `linear-gradient(135deg, ${customPri} 50%, ${customAcc} 50%)` }"
                                ></span>
                                <span class="color-label">{{ c.label }}</span>
                                <div class="color-check" v-if="colorSel === c.key">
                                    <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3">
                                        <polyline points="20 6 9 17 4 12"/>
                                    </svg>
                                </div>
                            </label>
                        </div>

                        <!-- Color personalizado -->
                        <div v-if="colorSel === 'custom'" class="custom-color-row">
                            <div class="custom-color-field">
                                <label class="custom-lbl">Color principal</label>
                                <div class="custom-input-wrap">
                                    <input type="color" v-model="customPri" class="color-picker" />
                                    <input type="text"  v-model="customPri" class="color-hex" maxlength="7" placeholder="#1C3829" />
                                </div>
                            </div>
                            <div class="custom-color-field">
                                <label class="custom-lbl">Color de acento</label>
                                <div class="custom-input-wrap">
                                    <input type="color" v-model="customAcc" class="color-picker" />
                                    <input type="text"  v-model="customAcc" class="color-hex" maxlength="7" placeholder="#8A6A2E" />
                                </div>
                            </div>
                        </div>
                    </section>

                </div>

                <!-- Pie -->
                <div class="cfg-foot">
                    <span class="cfg-foot-note">La configuración se guarda en este navegador y se aplica a todos los reportes.</span>
                    <div class="cfg-foot-btns">
                        <button class="btn-cancel" @click="cancelar">Cancelar</button>
                        <button class="btn-save"   @click="guardar"  :style="{ background: colorActivo().primary }">
                            Guardar configuración
                        </button>
                    </div>
                </div>

            </div>
        </div>
    </Teleport>
</template>

<style scoped>
/* ── Overlay ── */
.cfg-overlay {
    position: fixed; inset: 0; z-index: 999;
    background: rgba(0,0,0,.45);
    display: flex; align-items: center; justify-content: center;
    padding: 20px;
}
.cfg-modal {
    background: #fff; border-radius: 10px;
    width: 100%; max-width: 680px;
    max-height: 90vh; display: flex; flex-direction: column;
    box-shadow: 0 20px 60px rgba(0,0,0,.2);
    overflow: hidden;
}

/* ── Cabecera ── */
.cfg-head {
    display: flex; justify-content: space-between; align-items: flex-start;
    padding: 20px 24px 16px;
    border-bottom: 1px solid #E5E7EB;
    flex-shrink: 0;
}
.cfg-head-sub   { font-size: 11px; color: #6B7280; margin-bottom: 2px; }
.cfg-head-title { font-size: 16px; font-weight: 500; color: #111827; }
.cfg-close {
    width: 30px; height: 30px; border-radius: 6px;
    border: 1px solid #E5E7EB; background: #fff;
    display: flex; align-items: center; justify-content: center;
    cursor: pointer; color: #6B7280; flex-shrink: 0;
}
.cfg-close:hover { background: #F3F4F6; }

/* ── Body ── */
.cfg-body { overflow-y: auto; padding: 20px 24px; flex: 1; display: flex; flex-direction: column; gap: 24px; }
.cfg-section-title { font-size: 11px; font-weight: 500; color: #6B7280; text-transform: uppercase; letter-spacing: .5px; margin-bottom: 12px; }

/* ── Estilos grid ── */
.estilo-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }
.estilo-card {
    border: 1.5px solid #E5E7EB; border-radius: 8px;
    padding: 10px 10px 12px; cursor: pointer;
    transition: border-color .15s; position: relative;
    background: #FAFAFA;
}
.estilo-card:hover      { border-color: #9CA3AF; }
.estilo-card--on        { border-color: #1C3829 !important; background: #fff; }
.estilo-info            { margin-top: 8px; }
.estilo-name            { display: block; font-size: 12px; font-weight: 500; color: #111827; }
.estilo-desc            { display: block; font-size: 10px; color: #9CA3AF; margin-top: 2px; line-height: 1.4; }
.estilo-check {
    position: absolute; top: 7px; right: 7px;
    width: 18px; height: 18px; border-radius: 50%;
    background: #1C3829; color: #fff;
    display: flex; align-items: center; justify-content: center;
}

/* ── Miniaturas ── */
.estilo-thumb { height: 72px; border: 1px solid #E5E7EB; border-radius: 4px; overflow: hidden; background: #fff; display: flex; flex-direction: column; }
.th-header    { height: 16px; flex-shrink: 0; }
.th-header--light { border-bottom: 2px solid; }
.th-bar       { height: 2px; flex-shrink: 0; }
.th-kpis      { display: flex; gap: 2px; padding: 4px 4px 3px; flex-shrink: 0; }
.th-kpis--minimal { gap: 2px; padding: 3px 4px; }
.th-kpi       { flex: 1; height: 12px; border-radius: 1px; }
.th-kpi--border  { border-left: 2px solid; background: #F4F6F8; }
.th-kpi--box     { border: 1px solid; background: #fff; }
.th-kpi--solid   { }
.th-kpi--minimal { background: #F4F6F8; }
.th-table     { flex: 1; display: flex; flex-direction: column; gap: 1px; padding: 0 4px 4px; }
.th-thead     { height: 8px; border-radius: 1px; flex-shrink: 0; }
.th-thead--minimal { height: 7px; background: #E5E7EB; }
.th-trow      { flex: 1; border-radius: 1px; }

/* ── Color lista ── */
.color-lista { display: flex; flex-direction: column; gap: 6px; }
.color-row {
    display: flex; align-items: center; gap: 10px;
    padding: 9px 12px; border-radius: 7px;
    border: 1.5px solid #E5E7EB; cursor: pointer;
    transition: border-color .12s; position: relative;
    background: #FAFAFA;
}
.color-row:hover   { border-color: #9CA3AF; }
.color-row--on     { border-color: #1C3829 !important; background: #fff; }
.color-dot         { width: 20px; height: 20px; border-radius: 50%; flex-shrink: 0; border: 1px solid rgba(0,0,0,.08); }
.color-label       { font-size: 13px; color: #374151; flex: 1; }
.color-check {
    width: 18px; height: 18px; border-radius: 50%;
    background: #1C3829; color: #fff;
    display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}

/* ── Color personalizado ── */
.custom-color-row   { display: flex; gap: 16px; margin-top: 12px; }
.custom-color-field { flex: 1; display: flex; flex-direction: column; gap: 6px; }
.custom-lbl         { font-size: 11px; color: #6B7280; }
.custom-input-wrap  { display: flex; align-items: center; gap: 8px; }
.color-picker       { width: 32px; height: 32px; border: 1px solid #E5E7EB; border-radius: 5px; cursor: pointer; padding: 2px; }
.color-hex {
    flex: 1; height: 32px; border: 1px solid #E5E7EB; border-radius: 5px;
    padding: 0 10px; font-size: 12px; font-family: monospace; color: #374151;
}
.color-hex:focus { outline: none; border-color: #1C3829; }

/* ── Pie ── */
.cfg-foot {
    display: flex; align-items: center; justify-content: space-between;
    padding: 14px 24px; border-top: 1px solid #E5E7EB;
    background: #F9FAFB; flex-shrink: 0; gap: 12px; flex-wrap: wrap;
}
.cfg-foot-note { font-size: 11px; color: #9CA3AF; flex: 1; }
.cfg-foot-btns { display: flex; gap: 8px; }
.btn-cancel {
    padding: 8px 16px; border-radius: 6px;
    border: 1px solid #E5E7EB; background: #fff;
    font-size: 13px; color: #374151; cursor: pointer;
}
.btn-cancel:hover { background: #F3F4F6; }
.btn-save {
    padding: 8px 18px; border-radius: 6px; border: none;
    font-size: 13px; color: #fff; cursor: pointer; font-weight: 500;
    transition: opacity .12s;
}
.btn-save:hover { opacity: .88; }

.sr-only { position:absolute; width:1px; height:1px; overflow:hidden; clip:rect(0,0,0,0); }
</style>