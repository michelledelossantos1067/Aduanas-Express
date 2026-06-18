import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'

const estadosVehiculo = [
    { label: 'Disponible', value: 0 },
    { label: 'En Viaje', value: 1 },
    { label: 'En Mantenimiento', value: 2 },
    { label: 'Fuera de Servicio', value: 3 },
]

const estadoLabel = (valor) =>
    estadosVehiculo.find((e) => e.value === valor)?.label ?? valor

function formatFecha(fecha) {
    if (!fecha) return '—'
    return new Date(fecha).toLocaleDateString('es-DO', {
        day: '2-digit', month: '2-digit', year: 'numeric',
    })
}

/**
 * Genera y descarga un PDF con el reporte de vehículos.
 * @param {Array} vehiculos - Lista de vehículos a incluir en el reporte.
 * @param {Object} resumen - Resumen con totales { total, disponibles, enViaje, mantenim, fuera }.
 */
export function generarReporteVehiculosPdf(vehiculos = [], resumen = null) {
    const doc = new jsPDF({ orientation: 'landscape', unit: 'pt', format: 'a4' })
    const pageWidth = doc.internal.pageSize.getWidth()
    const margin = 40

    // ── Encabezado ──
    doc.setFillColor(26, 58, 42) // #1a3a2a
    doc.rect(0, 0, pageWidth, 70, 'F')

    doc.setTextColor(255, 255, 255)
    doc.setFont('helvetica', 'bold')
    doc.setFontSize(18)
    doc.text('Reporte de Vehículos', margin, 35)

    doc.setFont('helvetica', 'normal')
    doc.setFontSize(10)
    const fechaGeneracion = new Date().toLocaleDateString('es-DO', {
        day: '2-digit', month: 'long', year: 'numeric',
    })
    doc.text(`Generado el ${fechaGeneracion}`, margin, 53)

    // ── Resumen ──
    let cursorY = 95

    if (resumen) {
        const cards = [
            { label: 'Total', valor: resumen.total },
            { label: 'Disponibles', valor: resumen.disponibles },
            { label: 'En Viaje', valor: resumen.enViaje },
            { label: 'En Mantenimiento', valor: resumen.mantenim },
            { label: 'Fuera de Servicio', valor: resumen.fuera },
        ]

        const cardWidth = (pageWidth - margin * 2 - (cards.length - 1) * 10) / cards.length
        const cardHeight = 50

        cards.forEach((card, i) => {
            const x = margin + i * (cardWidth + 10)

            doc.setFillColor(243, 244, 246) // #f3f4f6
            doc.roundedRect(x, cursorY, cardWidth, cardHeight, 6, 6, 'F')

            doc.setTextColor(17, 24, 39) // #111827
            doc.setFont('helvetica', 'bold')
            doc.setFontSize(16)
            doc.text(String(card.valor), x + 14, cursorY + 24)

            doc.setTextColor(107, 114, 128) // #6b7280
            doc.setFont('helvetica', 'normal')
            doc.setFontSize(8)
            doc.text(card.label, x + 14, cursorY + 38)
        })

        cursorY += cardHeight + 25
    }

    // ── Tabla de vehículos ──
    const head = [[
        'Matrícula', 'Marca', 'Modelo', 'Año', 'Tipo', 'Color',
        'Capacidad', 'Kilometraje', 'Estado', 'Últ. Mantenimiento'
    ]]

    const body = vehiculos.map((v) => [
        v.matricula ?? '—',
        v.marca ?? '—',
        v.modelo ?? '—',
        v.año ?? '—',
        v.tipo ?? '—',
        v.color ?? '—',
        v.capacidad != null ? `${v.capacidad} pas.` : '—',
        v.kilometraje != null ? `${v.kilometraje.toLocaleString('es-DO')} km` : '—',
        estadoLabel(v.estado),
        formatFecha(v.fechaUltimoMant),
    ])

    autoTable(doc, {
        head,
        body,
        startY: cursorY,
        margin: { left: margin, right: margin },
        theme: 'grid',
        headStyles: {
            fillColor: [26, 58, 42],
            textColor: [255, 255, 255],
            fontStyle: 'bold',
            fontSize: 9,
        },
        bodyStyles: {
            fontSize: 8.5,
            textColor: [55, 65, 81],
        },
        alternateRowStyles: {
            fillColor: [249, 250, 251],
        },
        columnStyles: {
            0: { fontStyle: 'bold' },
        },
        didParseCell: (data) => {
            // Resaltar la columna de Estado con color según valor
            if (data.section === 'body' && data.column.index === 8) {
                const estado = data.row.raw[8]
                const colores = {
                    'Disponible': [209, 250, 229],
                    'En Viaje': [219, 234, 254],
                    'En Mantenimiento': [254, 243, 199],
                    'Fuera de Servicio': [254, 226, 226],
                }
                const textoColores = {
                    'Disponible': [6, 95, 70],
                    'En Viaje': [30, 64, 175],
                    'En Mantenimiento': [146, 64, 14],
                    'Fuera de Servicio': [153, 27, 27],
                }
                if (colores[estado]) {
                    data.cell.styles.fillColor = colores[estado]
                    data.cell.styles.textColor = textoColores[estado]
                    data.cell.styles.fontStyle = 'bold'
                }
            }
        },
        didDrawPage: () => {
            // Pie de página con número de página
            const pageCount = doc.internal.getNumberOfPages()
            const currentPage = doc.internal.getCurrentPageInfo().pageNumber
            doc.setFontSize(8)
            doc.setTextColor(156, 163, 175)
            doc.text(
                `Página ${currentPage} de ${pageCount}`,
                pageWidth - margin,
                doc.internal.pageSize.getHeight() - 20,
                { align: 'right' }
            )
            doc.text(
                'Sistema de Gestión de Flota',
                margin,
                doc.internal.pageSize.getHeight() - 20
            )
        },
    })

    // ── Descargar ──
    const nombreArchivo = `reporte-vehiculos-${new Date().toISOString().slice(0, 10)}.pdf`
    doc.save(nombreArchivo)
}