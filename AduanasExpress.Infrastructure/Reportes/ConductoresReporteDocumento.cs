using AduanasExpress.Application.DTOs.Reporte;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    public class ConductoresReporteDocumento : ReportePdfDocumentoBase
    {
        private readonly ReporteConductoresDTO _r;

        public ConductoresReporteDocumento(ReporteConductoresDTO reporte, ReporteConfigDTO cfg)
        : base(cfg) => _r = reporte;
        protected override string TituloReporte => "Reporte de Conductores";
        protected override string Subtitulo      => "Histórico general";

        protected override void ComponerContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(14);

                // ── KPIs ──────────────────────────────────────
                col.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Conductores activos", _r.TotalConductores.ToString(), ReporteEstilo.AcentoConductores));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Total de viajes", _r.TotalViajes.ToString(), ReporteEstilo.AcentoConductores));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Pasajeros transportados", _r.TotalPasajeros.ToString(), ReporteEstilo.AcentoConductores));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Pasajeros / viaje (prom.)", _r.PromedioPasajerosPorViaje.ToString("N1"), ReporteEstilo.AcentoConductores));
                });

                // ── Título sección ────────────────────────────
                col.Item().Element(c => ReportePdfHelpers.TituloSeccion(c, "Detalle por conductor"));

                // ── Tabla ─────────────────────────────────────
                if (_r.Detalles.Count == 0)
                {
                    col.Item().Element(c => ReportePdfHelpers.SinDatos(
                        c, "No hay conductores con viajes asignados."));
                }
                else
                {
                    col.Item().Element(c => ReportePdfHelpers.Tabla(
                        c,
                        new[] { "Conductor", "Núm. licencia", "Viajes", "Pasajeros", "Último viaje" },
                        _r.Detalles.Select(d => new[]
                        {
                            d.NombreConductor             ?? "—",
                            d.Licencia                    ?? "—",
                            d.TotalViajes.ToString(),
                            d.TotalPasajeros.ToString(),
                            d.UltimoViaje?.ToString("dd/MM/yyyy") ?? "—",
                        }).ToList(),
                        anchos: new[] { 1.6f, 1.1f, 0.7f, 0.8f, 0.95f },
                        columnasDerecha: new[] { 2, 3 }
                    ));

                    col.Item().PaddingTop(4).AlignRight()
                       .Text($"Total de conductores: {_r.TotalConductores}  ·  Total viajes: {_r.TotalViajes}")
                       .FontSize(7.5f).FontColor(ReporteEstilo.GrisClaro).Italic();
                }
            });
        }
    }
}