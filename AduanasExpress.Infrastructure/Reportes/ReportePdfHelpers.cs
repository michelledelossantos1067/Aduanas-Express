using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    /// <summary>
    /// Componentes reutilizables para todos los reportes PDF:
    ///  · TarjetaKpi     — indicador con acento de color izquierdo
    ///  · SectionTitle   — título de sección con línea verde
    ///  · Tabla          — tabla con encabezado verde y bandas alternadas
    ///  · SinDatos       — estado vacío
    ///  · BadgeEstado    — pastilla de estado coloreada inline
    /// </summary>
    public static class ReportePdfHelpers
    {
        // ── Tarjeta KPI ───────────────────────────────────────

        public static void TarjetaKpi(
            IContainer container,
            string etiqueta,
            string valor,
            string colorAcento = null)
        {
            var acento = colorAcento ?? ReporteEstilo.VerdeInstitucional;

            container
                .Border(0.5f).BorderColor(ReporteEstilo.GrisBorde)
                .Background(ReporteEstilo.Blanco)
                .Row(row =>
                {
                    // Borde izquierdo de color — único toque de color
                    row.ConstantItem(4).Background(acento);

                    row.RelativeItem()
                       .PaddingVertical(10)
                       .PaddingHorizontal(12)
                       .Column(col =>
                       {
                           col.Spacing(3);

                           col.Item()
                              .Text(valor)
                              .FontSize(18).Bold()
                              .FontColor(acento);

                           col.Item()
                              .Text(etiqueta)
                              .FontSize(7.5f)
                              .FontColor(ReporteEstilo.GrisClaro);
                       });
                });
        }

        // ── Título de sección ─────────────────────────────────

        public static void TituloSeccion(IContainer container, string texto)
        {
            container.Column(col =>
            {
                col.Item()
                   .PaddingBottom(4)
                   .Text(texto)
                   .FontSize(10).Bold()
                   .FontColor(ReporteEstilo.VerdeInstitucional)
                   .LetterSpacing(0.3f);

                col.Item()
                   .Height(1.5f)
                   .Background(ReporteEstilo.VerdeInstitucional);
            });
        }

        // ── Tabla ─────────────────────────────────────────────

        public static void Tabla(
            IContainer container,
            string[] encabezados,
            IReadOnlyList<string[]> filas,
            float[] anchos = null,
            int[] columnasDerecha = null)   // índices de columnas alineadas a la derecha
        {
            container.Table(table =>
            {
                // Definir columnas
                table.ColumnsDefinition(cols =>
                {
                    if (anchos != null)
                        foreach (var a in anchos) cols.RelativeColumn(a);
                    else
                        foreach (var _ in encabezados) cols.RelativeColumn();
                });

                // Encabezados
                table.Header(header =>
                {
                    for (int i = 0; i < encabezados.Length; i++)
                    {
                        bool esDer = columnasDerecha?.Contains(i) == true;
                        header.Cell()
                              .Background(ReporteEstilo.VerdeInstitucional)
                              .PaddingVertical(7)
                              .PaddingHorizontal(8)
                              .AlignMiddle()
                              .Element(c => esDer ? c.AlignRight() : c.AlignLeft())
                              .Text(encabezados[i])
                              .FontSize(7.5f).Bold()
                              .FontColor(ReporteEstilo.Blanco)
                              .LetterSpacing(0.3f);
                    }
                });

                // Filas de datos
                for (int i = 0; i < filas.Count; i++)
                {
                    bool esPar   = i % 2 == 0;
                    string fondo = esPar ? ReporteEstilo.Blanco : ReporteEstilo.GrisFondo;

                    for (int j = 0; j < filas[i].Length; j++)
                    {
                        bool esDer  = columnasDerecha?.Contains(j) == true;
                        var  valor  = filas[i][j] ?? "—";

                        table.Cell()
                             .Background(fondo)
                             .BorderBottom(0.4f).BorderColor(ReporteEstilo.GrisBorde)
                             .PaddingVertical(6)
                             .PaddingHorizontal(8)
                             .AlignMiddle()
                             .Element(c => esDer ? c.AlignRight() : c.AlignLeft())
                             .Text(valor)
                             .FontSize(8.3f)
                             .FontColor(ReporteEstilo.GrisTexto);
                    }
                }

                // Fila final con borde verde reforzado
                if (filas.Count > 0)
                {
                    table.Footer(footer =>
                    {
                        footer.Cell()
                              .ColumnSpan((uint)encabezados.Length)
                              .Height(1.5f)
                              .Background(ReporteEstilo.VerdeInstitucional);
                    });
                }
            });
        }

        // ── Sin datos ─────────────────────────────────────────

        public static void SinDatos(IContainer container, string mensaje)
        {
            container
                .Border(0.5f).BorderColor(ReporteEstilo.GrisBorde)
                .Background(ReporteEstilo.GrisFondo)
                .PaddingVertical(32)
                .AlignCenter()
                .Column(col =>
                {
                    col.Spacing(6);
                    col.Item().AlignCenter()
                       .Text("—")
                       .FontSize(20)
                       .FontColor(ReporteEstilo.GrisBorde);
                    col.Item().AlignCenter()
                       .Text(mensaje)
                       .FontSize(9)
                       .FontColor(ReporteEstilo.GrisClaro)
                       .Italic();
                });
        }

        // ── Badge de estado ───────────────────────────────────

        /// <summary>
        /// Devuelve texto coloreado para una celda de estado.
        /// Úsalo en la lambda de cada celda cuando la columna es "Estado".
        /// </summary>
        public static (string fondo, string texto) ColoresEstado(string estado)
        {
            var v = (estado ?? "").ToLower();

            if (v.Contains("finaliz") || v.Contains("complet") || v.Contains("aprobad"))
                return (ReporteEstilo.VerdeBadgeFondo, ReporteEstilo.VerdeEstado);

            if (v.Contains("pendiente") || v.Contains("espera"))
                return (ReporteEstilo.AmbarBadgeFondo, ReporteEstilo.AmbarEstado);

            if (v.Contains("cancel") || v.Contains("rechaz"))
                return (ReporteEstilo.RojoBadgeFondo, ReporteEstilo.RojoEstado);

            if (v.Contains("viaje") || v.Contains("proceso") || v.Contains("asign"))
                return (ReporteEstilo.AzulBadgeFondo, ReporteEstilo.AzulEstado);

            return (ReporteEstilo.GrisFondo, ReporteEstilo.GrisClaro);
        }
    }
}