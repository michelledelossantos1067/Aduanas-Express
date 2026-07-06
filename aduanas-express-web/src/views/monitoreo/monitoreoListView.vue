<script setup>
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { verVehiculos } from '@/services/vehiculoService.js'
import { verAsignaciones } from '@/services/asignacionService.js'

const CIUDAD_REFERENCIA = 'Santo Domingo, República Dominicana'

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

const ASIGNACION_INDICES_TERMINALES = [1, 2]
const ASIGNACION_ESTADOS_TERMINALES = ['finalizada', 'cancelada']

function asignacionEstaActiva(valor) {
  if (typeof valor === 'number') return !ASIGNACION_INDICES_TERMINALES.includes(valor)
  if (typeof valor === 'string') return !ASIGNACION_ESTADOS_TERMINALES.includes(valor.trim().toLowerCase())
  return false
}

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

  if (vehiculoSelId.value) {
    const v = vehiculos.value.find(x => x.id === id)
    if (v?.lat && v?.lng && mapInstance) {
      mapInstance.flyTo([v.lat, v.lng], 13, { duration: 1.2 })
    }
  }
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
async function procesarVehiculo(v, asignacionPorVehiculo) {
  const estado = normalizarEstadoVehiculo(v.estado)

  let destino     = null
  let origenTexto = null
  let destCord    = null
  let origen      = SEDE
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
    const solicitud = asignacion?.solicitud ?? null
    const cond = asignacion?.conductor ?? null
    origenTexto = solicitud?.puntoOrigen ?? null

    destino = solicitud?.destino ?? null
    fechaViaje = solicitud?.fechaViaje ?? null
    horaSalida = solicitud?.horaSalida ?? null
    

    if (cond) {
      conductor = [cond.nombre, cond.apellido].filter(Boolean).join(' ') || null
    }

    const coordDestino = await geocodificar(destino)
    if (coordDestino) {
      destCord = coordDestino
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
    destCord,
    origen,
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
  <div style="font-family:Inter,sans-serif;font-size:13px;min-width:210px;padding:4px 0">
    <b style="font-size:14px">${v.modelo}</b>
    <span style="color:#6b7280;font-size:11px;margin-left:6px">${v.matricula}</span>
    <div style="margin:5px 0 4px">
      <span style="color:${color};font-weight:700;font-size:12px">${estadoLabel(v.estado)}</span>
    </div>
    ${v.conductor ? `<div style="font-size:12px;color:#374151;margin-bottom:6px">👤 ${v.conductor}</div>` : ''}
    ${v.estado === 'en_viaje' ? `
      <div style="background:#f9fafb;border-radius:8px;padding:8px;margin-top:4px">
        <div style="display:flex;align-items:stretch;gap:10px">
          <div style="display:flex;flex-direction:column;align-items:center;padding-top:3px">
            <div style="width:10px;height:10px;border-radius:50%;background:#10b981;flex-shrink:0"></div>
            <div style="width:2px;flex:1;min-height:16px;background:#d1d5db;margin:3px 0"></div>
            <div style="width:10px;height:10px;border-radius:50%;background:#ef4444;flex-shrink:0"></div>
          </div>
          <div style="flex:1;display:flex;flex-direction:column;justify-content:space-between;gap:8px">
            <div>
              <div style="font-size:10px;color:#9ca3af;font-weight:600;text-transform:uppercase;letter-spacing:.04em">Origen</div>
              <div style="font-size:12px;color:#111827;font-weight:500">${v.origenTexto ?? 'Sede Central'}</div>
            </div>
            <div>
              <div style="font-size:10px;color:#9ca3af;font-weight:600;text-transform:uppercase;letter-spacing:.04em">Destino</div>
              <div style="font-size:12px;color:#111827;font-weight:500">${v.destino ?? '—'}</div>
            </div>
          </div>
        </div>
        ${(v.distanciaKm || v.duracionMin) ? `
        <div style="display:flex;gap:12px;border-top:1px solid #e5e7eb;padding-top:7px;margin-top:8px">
          ${v.distanciaKm ? `<span style="font-size:11px;color:#6b7280">📏 ${v.distanciaKm} km</span>` : ''}
          ${v.duracionMin ? `<span style="font-size:11px;color:#6b7280">⏱️ ${v.duracionMin} min</span>` : ''}
        </div>` : ''}
      </div>` : ''}
    ${v.posicionEstimada ? `<div style="margin-top:6px;font-size:10px;color:#9ca3af;background:#f3f4f6;display:inline-block;padding:2px 7px;border-radius:4px">Posición estimada</div>` : ''}
  </div>
`, { maxWidth: 270 })
    marker.addTo(capaMarcadores)

    // Mostrar ruta y marcadores de origen/destino si está en viaje
    if (v.estado === 'en_viaje' && v.ruta.length > 0) {
      // Dibujar la ruta
      const ruta = L.polyline(v.ruta, {
        color: color,
        weight: 3,
        opacity: 0.7,
        dashArray: '5, 5',
        className: 'ruta-viaje'
      })
      ruta.addTo(capaRutas)

      // Marcador de origen (SEDE)
      const iconOrigen = L.divIcon({
        className: '',
        html: `<div class="marker-origen" title="Origen">
          <div class="marker-origen-inner">A</div>
        </div>`,
        iconSize: [32, 32],
        iconAnchor: [16, 16],
      })
      L.marker([v.origen.lat, v.origen.lng], { icon: iconOrigen })
        .bindPopup(`<b>Punto de Origen</b><br><small>Sede Central</small>`)
        .addTo(capaMarcadores)

      // Marcador de destino
      if (v.destCord) {
        const iconDestino = L.divIcon({
          className: '',
          html: `<div class="marker-destino" title="Destino">
            <div class="marker-destino-inner">B</div>
          </div>`,
          iconSize: [32, 32],
          iconAnchor: [16, 16],
        })
        L.marker([v.destCord.lat, v.destCord.lng], { icon: iconDestino })
          .bindPopup(`<b>Punto de Destino</b><br><small>${v.destino}</small>`)
          .addTo(capaMarcadores)
      }
    }
  })
}

onMounted(() => {
  initMap()
  cargarDatos()
})

onUnmounted(() => {
  if (mapInstance) mapInstance.remove()
})

watch(filtroActivo, () => {
  pintarEnMapa()
})

watch(busqueda, () => {
  pintarEnMapa()
})
</script>

<template>
  <div class="mon-page">
    <div class="mon-header">
      <div>
        <h1 class="mon-titulo">Monitoreo de flota</h1>
        <p class="mon-subtitulo" v-if="ultimaActualizacion">Actualizado {{ formatHora(ultimaActualizacion.toLocaleTimeString('es-DO')) }}</p>
      </div>
      <button class="btn-actualizar" @click="cargarDatos" :disabled="loading">
        {{ loading ? 'Cargando...' : 'Actualizar' }}
      </button>
    </div>

    <div class="mon-layout">
      <!-- Sidebar izquierdo -->
      <div class="sidebar">
        <div class="search-box">
          <input type="text" v-model="busqueda" placeholder="Buscar matrícula, modelo o conductor..." class="search-input">
        </div>

        <div class="filtros">
          <button
            v-for="f in ['todos', 'en_viaje', 'libre', 'taller', 'fuera_servicio']"
            :key="f"
            :class="['filtro-btn', { activo: filtroActivo === f }]"
            @click="filtroActivo = f"
          >
            {{ { todos: 'Todos', en_viaje: 'En viaje', libre: 'Libre', taller: 'Taller', fuera_servicio: 'Fuera de servicio' }[f] }}
          </button>
        </div>

        <div class="vehiculos-lista">
          <div v-if="vehiculosFiltrados.length === 0" class="sin-resultados">
            <p v-if="loading">Cargando...</p>
            <p v-else-if="error">{{ error }}</p>
            <p v-else>Sin resultados</p>
          </div>

          <div v-for="v in vehiculosFiltrados" :key="v.id" :class="['vehiculo-card', { seleccionado: vehiculoSelId === v.id }]" @click="seleccionar(v.id)">
            <div class="card-header">
              <div class="card-title">{{ v.modelo }}</div>
              <span :class="['badge', estadoClase(v.estado)]">{{ estadoLabel(v.estado) }}</span>
            </div>
            <div class="card-matricula">{{ v.matricula }}</div>
            <div v-if="v.estado === 'en_viaje'" class="card-info">
              <span v-if="v.conductor">👤 {{ v.conductor }}</span>
              <span v-if="v.destino">📍 {{ v.destino }}</span>
              <div v-if="v.estado === 'en_viaje'" class="card-ruta">
  <div class="ruta-linea">
    <div class="ruta-punto origen"></div>
    <div class="ruta-barra"></div>
    <div class="ruta-punto destino"></div>
  </div>
  <div class="ruta-textos">
    <div class="ruta-dir">{{ v.origenTexto ?? 'Sede Central' }}</div>
    <div class="ruta-dir">{{ v.destino ?? '—' }}</div>
  </div>
</div>
              <span v-if="v.distanciaKm">📏 {{ v.distanciaKm }} km</span>
              <span v-if="v.duracionMin">⏱️ {{ v.duracionMin }} min</span>
            </div>
            <div v-if="v.posicionEstimada" class="estimada-tag">Posición estimada</div>
          </div>
        </div>

        <div class="resumen-pie">
          <div class="res-item">
            <div class="res-num azul">{{ resumen.en_viaje }}</div>
            <div class="res-lbl">En viaje</div>
          </div>
          <div class="res-sep"></div>
          <div class="res-item">
            <div class="res-num verde">{{ resumen.libres }}</div>
            <div class="res-lbl">Libres</div>
          </div>
          <div class="res-sep"></div>
          <div class="res-item">
            <div class="res-num naranja">{{ resumen.taller }}</div>
            <div class="res-lbl">Taller</div>
          </div>
          <div class="res-sep"></div>
          <div class="res-item">
            <div class="res-num rojo">{{ resumen.fuera_servicio }}</div>
            <div class="res-lbl">Fuera</div>
          </div>
        </div>
      </div>

      <!-- Mapa -->
      <div class="mapa-wrap">
        <div class="mapa-controles">
          <div class="mapa-tabs">
            <button class="mapa-tab" :class="{ 'tab-activo': tipoMapa === 'mapa' }" @click="cambiarTipoMapa('mapa')">Mapa</button>
            <button class="mapa-tab" :class="{ 'tab-activo': tipoMapa === 'satelite' }" @click="cambiarTipoMapa('satelite')">Satélite</button>
          </div>
          <div class="leyenda">
            <div class="leyenda-item">
              <div class="dot-leyenda" style="background: #2563eb;"></div>
              <span>En viaje</span>
            </div>
            <div class="leyenda-item">
              <div class="dot-leyenda" style="background: #16a34a;"></div>
              <span>Libre</span>
            </div>
            <div class="leyenda-item">
              <div class="dot-leyenda" style="background: #d97706;"></div>
              <span>Taller</span>
            </div>
            <div class="leyenda-item">
              <div class="dot-leyenda" style="background: #dc2626;"></div>
              <span>Fuera de servicio</span>
            </div>
            <div class="leyenda-item">
              <div class="dot-leyenda" style="background: #111827;"></div>
              <span>Sede</span>
            </div>
          </div>
        </div>
        <div ref="mapaEl" class="leaflet-container"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.mon-page {
  padding: 20px;
  background: #f9fafb;
  min-height: 100vh;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

.mon-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.mon-titulo {
  margin: 0;
  font-size: 1.75rem;
  font-weight: 700;
  color: #111827;
}

.mon-subtitulo {
  margin: 0;
  font-size: .85rem;
  color: #9ca3af;
  margin-top: 4px;
}

.btn-actualizar {
  padding: 8px 20px;
  background: #1a3a2a;
  color: #fff;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  font-size: .875rem;
  transition: background .2s;
}
.btn-actualizar:hover:not(:disabled) { background: #0f2818; }
.btn-actualizar:disabled { opacity: .6; cursor: not-allowed; }

.mon-layout {
  display: grid;
  grid-template-columns: 380px 1fr;
  gap: 16px;
}

.sidebar {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0,0,0,.06);
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.search-box {
  padding: 12px 14px;
  border-bottom: 1px solid #f3f4f6;
}

.search-input {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: .875rem;
  font-family: inherit;
  transition: border .15s;
}
.search-input:focus {
  outline: none;
  border-color: #2563eb;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, .1);
}

.filtros {
  display: flex;
  flex-direction: column;
  gap: 6px;
  padding: 12px 14px;
  border-bottom: 1px solid #f3f4f6;
}

.filtro-btn {
  padding: 8px 12px;
  background: #f3f4f6;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: .8rem;
  font-weight: 500;
  color: #6b7280;
  transition: all .15s;
  text-align: left;
}
.filtro-btn.activo {
  background: #1a3a2a;
  color: #fff;
}
.filtro-btn:hover:not(.activo) { background: #e5e7eb; }

.vehiculos-lista {
  flex: 1;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.sin-resultados {
  padding: 20px 14px;
  text-align: center;
  color: #9ca3af;
  font-size: .85rem;
}

.vehiculo-card {
  padding: 12px 14px;
  border-bottom: 1px solid #f3f4f6;
  cursor: pointer;
  transition: background .15s;
}
.vehiculo-card:hover { background: #f9fafb; }
.vehiculo-card.seleccionado { background: #eff6ff; border-left: 3px solid #2563eb; }

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
  margin-bottom: 4px;
}

.card-title {
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
.card-ruta {
  display: flex;
  gap: 8px;
  margin: 6px 0 4px;
  align-items: stretch;
}
.ruta-linea {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
  flex-shrink: 0;
  padding-top: 2px;
}
.ruta-punto {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.ruta-punto.origen  { background: #10b981; }
.ruta-punto.destino { background: #ef4444; }
.ruta-barra {
  width: 2px;
  flex: 1;
  min-height: 14px;
  background: #d1d5db;
}
.ruta-textos {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 6px;
  flex: 1;
}
.ruta-dir {
  font-size: .72rem;
  color: #374151;
  line-height: 1.3;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
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

:global(.marker-origen) {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 3px solid white;
  box-shadow: 0 3px 10px rgba(16, 185, 129, 0.4);
  position: relative;
}

:global(.marker-origen-inner) {
  width: 20px;
  height: 20px;
  background: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #10b981;
  font-weight: 700;
  font-size: 12px;
  font-family: 'Inter', sans-serif;
}

:global(.marker-destino) {
  width: 32px;
  height: 32px;
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 3px solid white;
  box-shadow: 0 3px 10px rgba(239, 68, 68, 0.4);
  position: relative;
}

:global(.marker-destino-inner) {
  width: 20px;
  height: 20px;
  background: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ef4444;
  font-weight: 700;
  font-size: 12px;
  font-family: 'Inter', sans-serif;
}

:global(.ruta-viaje) {
  z-index: 100;
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