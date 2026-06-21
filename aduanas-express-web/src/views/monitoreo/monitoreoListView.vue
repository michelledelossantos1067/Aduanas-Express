<script setup>
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { verVehiculos } from '@/services/vehiculoService.js'
import { verAsignaciones } from '@/services/asignacionService.js'

const CIUDAD_REFERENCIA = 'Santo Domingo, República Dominicana'

// --- Normalización de estados que llegan del backend (soporta texto o número del enum) ---

// Orden real confirmado en EstadosVehiculo.cs: Disponible=0, EnViaje=1, EnMantenimiento=2, FueraDeServicio=3
const VEHICULO_ESTADO_POR_INDICE = ['libre', 'en_viaje', 'taller', 'fuera_servicio']
const VEHICULO_ESTADO_POR_NOMBRE = {
  disponible:      'libre',
  enviaje:         'en_viaje',
  enmantenimiento: 'taller',
  fueradeservicio: 'fuera_servicio',
}

function normalizarEstadoVehiculo(valor) {
  if (typeof valor === 'number') return VEHICULO_ESTADO_POR_INDICE[valor] ?? 'libre'
  if (typeof valor === 'string') {
    const clave = valor.trim().toLowerCase().replace(/[\s_]/g, '')
    return VEHICULO_ESTADO_POR_NOMBRE[clave] ?? 'libre'
  }
  return 'libre'
}

// EstadoAsignacion no se confirmó directamente (no se subió el enum); se infiere del orden de uso
// en AsignacionService.cs: Pendiente, Finalizada, Cancelada. Si el filtro de "viaje activo" falla,
// ajustar este array según el enum real.
const ASIGNACION_INDICES_TERMINALES = [1, 2]
const ASIGNACION_ESTADOS_TERMINALES = ['finalizada', 'cancelada']

function asignacionEstaActiva(valor) {
  if (typeof valor === 'number') return !ASIGNACION_INDICES_TERMINALES.includes(valor)
  if (typeof valor === 'string') return !ASIGNACION_ESTADOS_TERMINALES.includes(valor.trim().toLowerCase())
  return false
}

// Indexa la asignación activa (si existe) de cada vehículo, para no recorrer el arreglo por cada vehículo
function indexarAsignacionesActivas(asignaciones) {
  const mapa = new Map()
  for (const a of asignaciones) {
    if (!asignacionEstaActiva(a.estado)) continue
    mapa.set(a.vehiculoId, a)
  }
  return mapa
}

const filtroActivo        = ref('todos')
const busqueda            = ref('')
const vehiculoSelId       = ref(null)
const tipoMapa            = ref('mapa')

const vehiculos           = ref([])
const asignacionesRaw     = ref([])
const loading             = ref(false)
const error               = ref('')
const ultimaActualizacion = ref(null)

const vehiculosFiltrados = computed(() => {
  const q = busqueda.value.toLowerCase()
  return vehiculos.value.filter(v => {
    const matchF = filtroActivo.value === 'todos' || v.estado === filtroActivo.value
    const matchQ = !q || [v.modelo, v.matricula, v.conductor ?? ''].some(s => (s ?? '').toLowerCase().includes(q))
    return matchF && matchQ
  })
})

const vehiculoSel = computed(() => vehiculos.value.find(v => v.id === vehiculoSelId.value) ?? null)

const resumen = computed(() => ({
  en_viaje:       vehiculos.value.filter(v => v.estado === 'en_viaje').length,
  libres:         vehiculos.value.filter(v => v.estado === 'libre').length,
  taller:         vehiculos.value.filter(v => v.estado === 'taller').length,
  fuera_servicio: vehiculos.value.filter(v => v.estado === 'fuera_servicio').length,
}))

function estadoLabel(e) {
  return { en_viaje: 'En viaje', libre: 'Disponible', taller: 'Taller', fuera_servicio: 'Fuera de servicio' }[e] ?? e
}
function estadoClase(e) {
  return { en_viaje: 'badge-en-viaje', libre: 'badge-libre', taller: 'badge-taller', fuera_servicio: 'badge-fuera' }[e] ?? ''
}
function dotClase(e) {
  return { en_viaje: 'dot-azul', libre: 'dot-verde', taller: 'dot-naranja', fuera_servicio: 'dot-rojo' }[e] ?? 'dot-gris'
}
function seleccionar(id) {
  vehiculoSelId.value = vehiculoSelId.value === id ? null : id
}
function formatHora(h) {
  if (!h) return '—'

  return h.substring(0, 5)
}
function formatFecha(f) {
  if (!f) return '—'
  return new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: 'short' })
}

const geocodeCache = new Map()

// Convierte una dirección en coordenadas mediante Nominatim y almacena el resultado en caché
async function geocodificar(direccion) {
  if (!direccion) return null
  const clave = direccion.trim().toLowerCase()
  if (geocodeCache.has(clave)) return geocodeCache.get(clave)
  try {
    const url = `https://nominatim.openstreetmap.org/search?format=json&limit=1&q=${encodeURIComponent(direccion + ', ' + CIUDAD_REFERENCIA)}`
    const res  = await fetch(url, { headers: { 'Accept-Language': 'es' } })
    const data = await res.json()
    if (data?.[0]) {
      const coord = { lat: parseFloat(data[0].lat), lng: parseFloat(data[0].lon) }
      geocodeCache.set(clave, coord)
      return coord
    }
  } catch (e) {
    console.error('Geocodificación fallida para:', direccion, e)
  }
  geocodeCache.set(clave, null)
  return null
}

// Obtiene la ruta de conducción entre dos puntos usando el servicio OSRM
async function obtenerRuta(origen, destino) {
  try {
    const url = `https://router.project-osrm.org/route/v1/driving/${origen.lng},${origen.lat};${destino.lng},${destino.lat}?overview=full&geometries=geojson`
    const res  = await fetch(url)
    const data = await res.json()
    const ruta = data?.routes?.[0]
    if (ruta) {
      return {
        coords:       ruta.geometry.coordinates.map(c => [c[1], c[0]]),
        distanciaKm:  +(ruta.distance / 1000).toFixed(1),
        duracionMin:  Math.round(ruta.duration / 60),
      }
    }
  } catch (e) {
    console.error('OSRM falló:', e)
  }
  return {
    coords:      [[origen.lat, origen.lng], [destino.lat, destino.lng]],
    distanciaKm: null,
    duracionMin: null,
  }
}

const SEDE = { lat: 18.4861, lng: -69.9312 }

// Estima la ubicación actual del vehículo en la ruta según el tiempo transcurrido desde la salida
function posicionEstimada(coords, fechaViaje, horaSalida, duracionMin) {
  if (!coords?.length || !fechaViaje || !horaSalida || !duracionMin) return null
  try {

    const dateStr = fechaViaje.substring(0, 10)
    const inicioMs = new Date(`${dateStr}T${horaSalida}`).getTime()
    if (Number.isNaN(inicioMs)) return null
    const minutosTranscurridos = (Date.now() - inicioMs) / 60000
    const fraccion = Math.min(1, Math.max(0, minutosTranscurridos / duracionMin))
    const idx = Math.min(coords.length - 1, Math.floor(fraccion * (coords.length - 1)))
    return { lat: coords[idx][0], lng: coords[idx][1] }
  } catch { return null }
}

// Construye los datos de mapa/posición de un vehículo a partir de su estado real y, si está
// en viaje, de la asignación activa (que ya trae anidados el conductor y la solicitud)
async function procesarVehiculo(v, asignacionPorVehiculo) {
  const estado = normalizarEstadoVehiculo(v.estado)

  let destino     = null
  let conductor   = null
  let ruta        = []
  let distanciaKm = null
  let duracionMin = null
  let posicion    = null
  let estimada    = false
  let fechaViaje  = null
  let horaSalida  = null

  if (estado === 'en_viaje') {
    const asignacion = asignacionPorVehiculo.get(v.id) ?? null
    const solicitud   = asignacion?.solicitud ?? null
    const cond        = asignacion?.conductor ?? null

    destino    = solicitud?.destino ?? null
    fechaViaje = solicitud?.fechaViaje ?? null
    horaSalida = solicitud?.horaSalida ?? null

    if (cond) {
      conductor = [cond.nombre, cond.apellido].filter(Boolean).join(' ') || null
    }

    const coordDestino = await geocodificar(destino)
    if (coordDestino) {
      const r = await obtenerRuta(SEDE, coordDestino)
      ruta        = r.coords
      distanciaKm = r.distanciaKm
      duracionMin = r.duracionMin
      posicion    = posicionEstimada(ruta, fechaViaje, horaSalida, duracionMin)
      estimada    = !!posicion
      if (!posicion) posicion = SEDE
    }
  }

  return {
    id:               v.id,
    modelo:           [v.marca, v.modelo].filter(Boolean).join(' ') || 'Vehículo',
    matricula:        v.matricula,
    tipo:             v.tipo ?? null,
    capacidad:        v.capacidad ?? null,
    estado,
    conductor,
    destino,
    fechaViaje,
    horaSalida,
    distanciaKm,
    duracionMin,
    posicionEstimada: estimada,
    lat:  posicion?.lat ?? null,
    lng:  posicion?.lng ?? null,
    ruta,
  }
}

async function cargarDatos() {
  loading.value = true
  error.value   = ''
  try {
    const [resVeh, resAsig] = await Promise.all([
      verVehiculos(),
      verAsignaciones(),
    ])
    asignacionesRaw.value = resAsig.data ?? []
    const activasPorVehiculo = indexarAsignacionesActivas(asignacionesRaw.value)

    vehiculos.value = await Promise.all(
      (resVeh.data ?? []).map(v => procesarVehiculo(v, activasPorVehiculo))
    )
    ultimaActualizacion.value = new Date()
    await nextTick()
    pintarEnMapa()
  } catch (e) {
    console.error(e)
    error.value = 'No se pudieron cargar los datos de la flota.'
  } finally {
    loading.value = false
  }
}

const mapaEl = ref(null)
let mapInstance    = null
let capaCalles     = null
let capaSatelite   = null
let capaMarcadores = null
let capaRutas      = null

function initMap() {
  if (!mapaEl.value) return
  mapInstance = L.map(mapaEl.value, { zoomControl: true }).setView([SEDE.lat, SEDE.lng], 12)

  capaCalles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 19,
  }).addTo(mapInstance)

  capaSatelite = L.tileLayer(
    'https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}',
    { attribution: 'Tiles &copy; Esri', maxZoom: 19 }
  )

  capaMarcadores = L.layerGroup().addTo(mapInstance)
  capaRutas      = L.layerGroup().addTo(mapInstance)

  const iconSede = L.divIcon({
    className: '',
    html: `<div class="marker-sede">S</div>`,
    iconSize: [28, 28],
    iconAnchor: [14, 14],
  })
  L.marker([SEDE.lat, SEDE.lng], { icon: iconSede })
    .bindPopup('<b>Sede Central</b>')
    .addTo(mapInstance)
}

function cambiarTipoMapa(tipo) {
  tipoMapa.value = tipo
  if (!mapInstance) return
  if (tipo === 'satelite') {
    mapInstance.removeLayer(capaCalles)
    capaSatelite.addTo(mapInstance)
  } else {
    mapInstance.removeLayer(capaSatelite)
    capaCalles.addTo(mapInstance)
  }
}

function colorEstado(estado) {
  return { en_viaje: '#2563eb', libre: '#16a34a', taller: '#d97706', fuera_servicio: '#dc2626' }[estado] ?? '#6b7280'
}

function pintarEnMapa() {
  if (!mapInstance) return
  capaMarcadores.clearLayers()
  capaRutas.clearLayers()

  vehiculosFiltrados.value.forEach(v => {
    if (v.lat == null || v.lng == null) return

    const color = colorEstado(v.estado)
    const icon = L.divIcon({
      className: '',
      html: `<div class="marker-pin" style="background:${color};border:2.5px solid white;width:18px;height:18px;border-radius:50%;box-shadow:0 2px 6px rgba(0,0,0,.35)"></div>`,
      iconSize: [18, 18],
      iconAnchor: [9, 9],
    })

    const marker = L.marker([v.lat, v.lng], { icon })
    marker.bindPopup(`
      <div style="font-family:Inter,sans-serif;font-size:13px;min-width:170px">
        <b style="font-size:14px">${v.modelo}</b><br>
        <span style="color:#6b7280;font-size:11px">${v.matricula}</span><br><br>
        <span style="color:${color};font-weight:700">${estadoLabel(v.estado)}</span><br>
        ${v.conductor ? `<span>👤 ${v.conductor}</span><br>` : ''}
        ${v.destino   ? `<span>📍 ${v.destino}</span><br>` : ''}
        ${v.distanciaKm ? `<span>🛣 ${v.distanciaKm} km</span>` : ''}
        ${v.posicionEstimada ? '<br><span style="color:#9ca3af;font-size:10px">📡 Posición estimada</span>' : ''}
      </div>
    `)
    marker.on('click', () => seleccionar(v.id))
    marker.addTo(capaMarcadores)

    if (v.ruta.length > 1) {
      L.polyline(v.ruta, { color: '#2563eb', weight: 4, opacity: 0.75, dashArray: '8 4' })
        .addTo(capaRutas)
    }
  })
}

watch(vehiculosFiltrados, () => pintarEnMapa())

watch(vehiculoSelId, id => {
  const v = vehiculos.value.find(x => x.id === id)
  if (v?.lat != null && mapInstance) mapInstance.flyTo([v.lat, v.lng], 14, { duration: 1 })
})

let intervalo = null
onMounted(async () => {
  await nextTick()
  initMap()
  await cargarDatos()
  intervalo = setInterval(cargarDatos, 60000)
})
onUnmounted(() => {
  clearInterval(intervalo)
  mapInstance?.remove()
})
</script>

<template>
<div class="mon-page">

  <div class="mon-header">
    <div class="mon-header-left">
      <h1 class="mon-title">Monitoreo de flota</h1>
      <span v-if="ultimaActualizacion" class="ultima-act">
        Actualizado {{ ultimaActualizacion.toLocaleTimeString('es-DO', { hour: '2-digit', minute: '2-digit' }) }}
      </span>
    </div>
    <div class="mon-header-right">
      <span class="badge-live">
        <span class="live-dot"></span>
        EN VIVO
      </span>
      <button class="btn-dark" :disabled="loading" @click="cargarDatos">
        <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2"
             :class="{ 'spin': loading }">
          <polyline points="1 4 1 10 7 10"/>
          <path d="M3.51 15a9 9 0 1 0 .49-4.95"/>
        </svg>
        {{ loading ? 'Cargando…' : 'Actualizar' }}
      </button>
    </div>
  </div>

  <div v-if="error" class="error-banner">
    ⚠️ {{ error }}
  </div>

  <div class="mon-layout">

    <div class="panel-izq">

      <div class="search-wrap">
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" stroke-width="2">
          <circle cx="11" cy="11" r="8"/><line x1="21" y1="21" x2="16.65" y2="16.65"/>
        </svg>
        <input v-model="busqueda" type="text" placeholder="Buscar matrícula, modelo o conductor…" class="search-input"/>
      </div>

      <div class="filtros-row">
        <button class="filtro-btn"            :class="{ activo: filtroActivo === 'todos' }"    @click="filtroActivo = 'todos'">Todos</button>
        <button class="filtro-btn filtro-vj"  :class="{ activo: filtroActivo === 'en_viaje' }" @click="filtroActivo = 'en_viaje'">En viaje</button>
        <button class="filtro-btn filtro-lb"  :class="{ activo: filtroActivo === 'libre' }"    @click="filtroActivo = 'libre'">Libre</button>
        <button class="filtro-btn filtro-tl"  :class="{ activo: filtroActivo === 'taller' }"   @click="filtroActivo = 'taller'">Taller</button>
        <button class="filtro-btn filtro-fs"  :class="{ activo: filtroActivo === 'fuera_servicio' }" @click="filtroActivo = 'fuera_servicio'">Fuera de servicio</button>
      </div>

      <p class="flota-label">
        {{ vehiculosFiltrados.length }} de {{ vehiculos.length }} vehículos
      </p>

      <div class="vehiculos-lista">

        <template v-if="loading && vehiculos.length === 0">
          <div v-for="n in 4" :key="n" class="vehiculo-card skeleton">
            <div class="sk-line sk-w70"></div>
            <div class="sk-line sk-w40 sk-sm"></div>
            <div class="sk-line sk-w90 sk-sm"></div>
          </div>
        </template>

        <div v-else-if="!loading && vehiculosFiltrados.length === 0" class="empty-state">
          <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#d1d5db" stroke-width="1.5">
            <rect x="3" y="3" width="18" height="18" rx="2"/><path d="M9 9h6M9 13h4"/>
          </svg>
          <p>Sin vehículos para mostrar</p>
        </div>

        <div
          v-for="v in vehiculosFiltrados"
          :key="v.id"
          class="vehiculo-card"
          :class="{ 'card-sel': vehiculoSelId === v.id }"
          @click="seleccionar(v.id)"
        >
          <div class="card-top">
            <span class="card-modelo">{{ v.modelo }}</span>
            <span class="badge" :class="estadoClase(v.estado)">{{ estadoLabel(v.estado) }}</span>
          </div>
          <p class="card-matricula">{{ v.matricula }}</p>

          <div v-if="v.destino" class="card-info">
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/><circle cx="12" cy="10" r="3"/>
            </svg>
            {{ v.destino }}
          </div>

          <div class="card-meta">
            <span v-if="v.conductor" class="meta-item">
              <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/>
              </svg>
              {{ v.conductor }}
            </span>
            <span v-if="v.horaSalida" class="meta-item">
              <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
              </svg>
              Salida {{ formatHora(v.horaSalida) }}
            </span>
            <span v-if="v.distanciaKm" class="meta-item">
              🛣 {{ v.distanciaKm }} km
            </span>
          </div>

          <div v-if="v.posicionEstimada" class="estimada-tag">
            📡 Posición estimada
          </div>
        </div>
      </div>

      <div class="resumen-pie">
        <div class="res-item">
          <span class="res-num azul">{{ resumen.en_viaje }}</span>
          <span class="res-lbl">En viaje</span>
        </div>
        <div class="res-sep"></div>
        <div class="res-item">
          <span class="res-num verde">{{ resumen.libres }}</span>
          <span class="res-lbl">Libres</span>
        </div>
        <div class="res-sep"></div>
        <div class="res-item">
          <span class="res-num naranja">{{ resumen.taller }}</span>
          <span class="res-lbl">Taller</span>
        </div>
        <div class="res-sep"></div>
        <div class="res-item">
          <span class="res-num rojo">{{ resumen.fuera_servicio }}</span>
          <span class="res-lbl">Fuera</span>
        </div>
      </div>
    </div>

    <div class="mapa-wrap">

      <div class="mapa-controles">
        <div class="mapa-tabs">
          <button class="mapa-tab" :class="{ 'tab-activo': tipoMapa === 'mapa' }"     @click="cambiarTipoMapa('mapa')">Mapa</button>
          <button class="mapa-tab" :class="{ 'tab-activo': tipoMapa === 'satelite' }" @click="cambiarTipoMapa('satelite')">Satélite</button>
        </div>

        <div class="leyenda">
          <div class="leyenda-item"><span class="dot-leyenda" style="background:#2563eb"></span>En viaje</div>
          <div class="leyenda-item"><span class="dot-leyenda" style="background:#16a34a"></span>Libre</div>
          <div class="leyenda-item"><span class="dot-leyenda" style="background:#d97706"></span>Taller</div>
          <div class="leyenda-item"><span class="dot-leyenda" style="background:#dc2626"></span>Fuera de servicio</div>
          <div class="leyenda-item"><span class="dot-leyenda" style="background:#111827"></span>Sede</div>
        </div>
      </div>

      <div ref="mapaEl" class="leaflet-container"></div>

      <transition name="popup-fade">
        <div v-if="vehiculoSel" class="popup-vehiculo">
          <div class="popup-header">
            <span class="popup-dot" :class="dotClase(vehiculoSel.estado)"></span>
            <span class="popup-modelo">{{ vehiculoSel.modelo }}</span>
            <span class="popup-matricula">{{ vehiculoSel.matricula }}</span>
            <button class="popup-close" @click="vehiculoSelId = null">✕</button>
          </div>
          <div class="popup-grid">
            <div class="popup-row">
              <span class="popup-lbl">Estado</span>
              <span class="popup-val">
                <span class="badge" :class="estadoClase(vehiculoSel.estado)">{{ estadoLabel(vehiculoSel.estado) }}</span>
              </span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.conductor">
              <span class="popup-lbl">Conductor</span>
              <span class="popup-val">{{ vehiculoSel.conductor }}</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.destino">
              <span class="popup-lbl">Destino</span>
              <span class="popup-val">{{ vehiculoSel.destino }}</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.fechaViaje">
              <span class="popup-lbl">Fecha viaje</span>
              <span class="popup-val">{{ formatFecha(vehiculoSel.fechaViaje) }}</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.horaSalida">
              <span class="popup-lbl">Hora salida</span>
              <span class="popup-val">{{ formatHora(vehiculoSel.horaSalida) }}</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.distanciaKm">
              <span class="popup-lbl">Distancia ruta</span>
              <span class="popup-val">{{ vehiculoSel.distanciaKm }} km</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.duracionMin">
              <span class="popup-lbl">Duración estimada</span>
              <span class="popup-val">{{ vehiculoSel.duracionMin }} min</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.tipo">
              <span class="popup-lbl">Tipo</span>
              <span class="popup-val">{{ vehiculoSel.tipo }}</span>
            </div>
            <div class="popup-row" v-if="vehiculoSel.capacidad">
              <span class="popup-lbl">Capacidad</span>
              <span class="popup-val">{{ vehiculoSel.capacidad }} personas</span>
            </div>
            <div v-if="vehiculoSel.posicionEstimada" class="popup-estimada">
              📡 La posición mostrada es estimada según hora de salida
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>

</div>
</template>

<style scoped>

* { box-sizing: border-box; }

.mon-page {
  padding: 22px 28px;
  background: #f3f4f6;
  min-height: 100vh;
  font-family: 'Inter', 'Segoe UI', sans-serif;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.mon-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 10px;
}

.mon-header-left {
  display: flex;
  align-items: baseline;
  gap: 12px;
}

.mon-title {
  font-size: 1.4rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
  letter-spacing: -.02em;
}

.ultima-act {
  font-size: .72rem;
  color: #9ca3af;
  font-weight: 500;
}

.mon-header-right {
  display: flex;
  align-items: center;
  gap: 10px;
}

.badge-live {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  background: #fff;
  border: 1.5px solid #fca5a5;
  color: #991b1b;
  font-size: .72rem;
  font-weight: 800;
  letter-spacing: .08em;
  padding: 5px 13px;
  border-radius: 20px;
}

.live-dot {
  width: 7px;
  height: 7px;
  background: #dc2626;
  border-radius: 50%;
  animation: pulsar 1.4s ease-in-out infinite;
}

@keyframes pulsar {
  0%, 100% { opacity: 1; transform: scale(1); }
  50%       { opacity: .35; transform: scale(.65); }
}

.btn-dark {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 18px;
  background: #1a3a2a;
  border: none;
  border-radius: 8px;
  font-size: .85rem;
  font-weight: 600;
  color: #fff;
  cursor: pointer;
  transition: background .15s;
}
.btn-dark:hover:not(:disabled) { background: #14532d; }
.btn-dark:disabled { opacity: .55; cursor: not-allowed; }

.spin {
  animation: girar .8s linear infinite;
}
@keyframes girar {
  from { transform: rotate(0deg); }
  to   { transform: rotate(360deg); }
}

.error-banner {
  background: #fef2f2;
  border: 1px solid #fca5a5;
  color: #991b1b;
  padding: 10px 16px;
  border-radius: 8px;
  font-size: .85rem;
  font-weight: 500;
}

.mon-layout {
  display: grid;
  grid-template-columns: 320px 1fr;
  gap: 14px;
  flex: 1;
  min-height: 0;
}

.panel-izq {
  background: #fff;
  border-radius: 14px;
  box-shadow: 0 1px 4px rgba(0,0,0,.07);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.search-wrap {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 14px;
  border-bottom: 1px solid #f3f4f6;
}

.search-input {
  flex: 1;
  border: none;
  outline: none;
  font-size: .84rem;
  color: #111827;
  font-family: inherit;
  background: transparent;
}
.search-input::placeholder { color: #9ca3af; }

.filtros-row {
  display: flex;
  gap: 5px;
  padding: 10px 12px 8px;
  border-bottom: 1px solid #f3f4f6;
  flex-wrap: wrap;
}

.filtro-btn {
  padding: 4px 13px;
  border-radius: 20px;
  font-size: .72rem;
  font-weight: 600;
  cursor: pointer;
  border: 1.5px solid #e5e7eb;
  background: transparent;
  color: #374151;
  transition: all .15s;
}
.filtro-btn.activo,
.filtro-btn:hover { background: #111827; color: #fff; border-color: #111827; }
.filtro-vj.activo { background: #1e40af; border-color: #1e40af; }
.filtro-lb.activo { background: #15803d; border-color: #15803d; }
.filtro-tl.activo { background: #b45309; border-color: #b45309; }
.filtro-fs.activo { background: #b91c1c; border-color: #b91c1c; }

.flota-label {
  font-size: .7rem;
  color: #9ca3af;
  font-weight: 600;
  padding: 6px 14px 4px;
  margin: 0;
  letter-spacing: .04em;
  text-transform: uppercase;
}

.vehiculos-lista {
  flex: 1;
  overflow-y: auto;
  padding: 6px 10px;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.skeleton { pointer-events: none; }
.sk-line {
  height: 11px;
  background: linear-gradient(90deg, #f3f4f6 25%, #e9eaeb 50%, #f3f4f6 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
  border-radius: 4px;
  margin-bottom: 8px;
}
.sk-w70 { width: 70%; }
.sk-w40 { width: 40%; }
.sk-w90 { width: 90%; }
.sk-sm  { height: 9px; }

@keyframes shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 40px 20px;
  color: #9ca3af;
  font-size: .82rem;
  text-align: center;
}
.empty-state p { margin: 0; }

.vehiculo-card {
  background: #fafafa;
  border: 1.5px solid #f3f4f6;
  border-radius: 10px;
  padding: 10px 12px;
  cursor: pointer;
  transition: border-color .15s, background .15s;
}
.vehiculo-card:hover { border-color: #d1d5db; }
.card-sel { border-color: #1a3a2a !important; background: #f0fdf4 !important; }

.card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 2px;
  gap: 6px;
}

.card-modelo {
  font-size: .85rem;
  font-weight: 700;
  color: #111827;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-matricula {
  font-size: .75rem;
  color: #6b7280;
  margin: 0 0 5px;
  font-family: 'Courier New', monospace;
  font-weight: 600;
}

.card-info {
  display: flex;
  align-items: flex-start;
  gap: 5px;
  font-size: .74rem;
  color: #6b7280;
  margin-bottom: 5px;
  line-height: 1.4;
}

.card-meta {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  align-items: center;
}

.meta-item {
  display: inline-flex;
  align-items: center;
  gap: 3px;
  font-size: .7rem;
  color: #9ca3af;
}

.estimada-tag {
  margin-top: 5px;
  font-size: .66rem;
  color: #6b7280;
  background: #f3f4f6;
  display: inline-block;
  padding: 2px 7px;
  border-radius: 4px;
}

.badge {
  display: inline-block;
  padding: 2px 9px;
  border-radius: 20px;
  font-size: .67rem;
  font-weight: 700;
  white-space: nowrap;
}
.badge-en-viaje { background: #dbeafe; color: #1e40af; }
.badge-libre    { background: #d1fae5; color: #065f46; }
.badge-taller   { background: #fef3c7; color: #92400e; }
.badge-fuera    { background: #fee2e2; color: #991b1b; }

.resumen-pie {
  display: flex;
  align-items: center;
  justify-content: space-around;
  padding: 12px 14px;
  border-top: 1px solid #f3f4f6;
  background: #fafafa;
}

.res-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1px;
}

.res-num {
  font-size: 1.3rem;
  font-weight: 800;
  line-height: 1;
}
.res-lbl {
  font-size: .66rem;
  color: #9ca3af;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: .04em;
}
.azul    { color: #2563eb; }
.verde   { color: #16a34a; }
.naranja { color: #d97706; }
.rojo    { color: #dc2626; }

.res-sep {
  width: 1px;
  height: 28px;
  background: #f3f4f6;
}

.mapa-wrap {
  background: #fff;
  border-radius: 14px;
  box-shadow: 0 1px 4px rgba(0,0,0,.07);
  position: relative;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  min-height: 560px;
}

.mapa-controles {
  position: absolute;
  top: 12px;
  left: 12px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.mapa-tabs {
  display: flex;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0,0,0,.15);
}

.mapa-tab {
  padding: 6px 16px;
  border: none;
  background: #fff;
  font-size: .78rem;
  font-weight: 600;
  color: #374151;
  cursor: pointer;
  transition: all .15s;
  font-family: inherit;
}
.mapa-tab:not(:last-child) { border-right: 1px solid #e5e7eb; }
.tab-activo { background: #1a3a2a; color: #fff; }

.leyenda {
  background: rgba(255,255,255,.95);
  border-radius: 8px;
  padding: 8px 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,.12);
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.leyenda-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: .72rem;
  color: #374151;
  font-weight: 500;
}

.dot-leyenda {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  flex-shrink: 0;
  border: 1.5px solid rgba(255,255,255,.8);
  box-shadow: 0 0 0 1px rgba(0,0,0,.1);
}

.leaflet-container {
  flex: 1;
  width: 100%;
  min-height: 0;
}

:deep(.leaflet-control-zoom) {
  margin-top: 60px !important;
}

:global(.marker-sede) {
  width: 28px;
  height: 28px;
  background: #111827;
  color: #fff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 800;
  font-family: 'Inter', sans-serif;
  border: 2.5px solid white;
  box-shadow: 0 2px 8px rgba(0,0,0,.35);
}

.popup-vehiculo {
  position: absolute;
  bottom: 16px;
  right: 14px;
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 14px 16px;
  width: 280px;
  box-shadow: 0 4px 24px rgba(0,0,0,.13);
  z-index: 1000;
}

.popup-fade-enter-active,
.popup-fade-leave-active { transition: opacity .2s, transform .2s; }
.popup-fade-enter-from,
.popup-fade-leave-to { opacity: 0; transform: translateY(8px); }

.popup-header {
  display: flex;
  align-items: center;
  gap: 7px;
  margin-bottom: 10px;
  padding-bottom: 9px;
  border-bottom: 1px solid #f3f4f6;
}

.popup-dot {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  flex-shrink: 0;
}
.dot-azul    { background: #2563eb; }
.dot-verde   { background: #16a34a; }
.dot-naranja { background: #d97706; }
.dot-rojo    { background: #dc2626; }
.dot-gris    { background: #9ca3af; }

.popup-modelo {
  font-size: .875rem;
  font-weight: 700;
  color: #111827;
  flex: 1;
}

.popup-matricula {
  font-size: .72rem;
  color: #9ca3af;
  font-family: monospace;
  font-weight: 600;
}

.popup-close {
  background: none;
  border: none;
  cursor: pointer;
  color: #9ca3af;
  font-size: .9rem;
  padding: 2px 4px;
  border-radius: 4px;
  transition: color .15s;
}
.popup-close:hover { color: #374151; }

.popup-grid {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.popup-row {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 10px;
}

.popup-lbl {
  font-size: .72rem;
  color: #9ca3af;
  font-weight: 500;
  white-space: nowrap;
  flex-shrink: 0;
}

.popup-val {
  font-size: .8rem;
  color: #111827;
  font-weight: 600;
  text-align: right;
}

.popup-estimada {
  margin-top: 6px;
  padding: 6px 9px;
  background: #f3f4f6;
  border-radius: 6px;
  font-size: .68rem;
  color: #6b7280;
  line-height: 1.5;
}

@media (max-width: 960px) {
  .mon-layout { grid-template-columns: 1fr; }
  .mapa-wrap  { min-height: 420px; }
}

@media (max-width: 600px) {
  .mon-page     { padding: 12px; }
  .mon-header   { flex-direction: column; align-items: flex-start; }
  .popup-vehiculo { width: calc(100% - 28px); right: 14px; }
}
</style>