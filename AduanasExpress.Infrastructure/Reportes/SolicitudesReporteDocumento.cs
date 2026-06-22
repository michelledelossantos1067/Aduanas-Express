using AduanasExpress.Application.DTOs.Reporte;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    public class SolicitudesReporteDocumento : ReportePdfDocumentoBase
    {
        private readonly ReporteSolicitudesDTO _r;

        public SolicitudesReporteDocumento(ReporteSolicitudesDTO reporte, ReporteConfigDTO cfg)
        : base(cfg) => _r = reporte;
        protected override string TituloReporte => "Reporte de Solicitudes";
        protected override string Subtitulo      => "Histórico general";

        protected override void ComponerContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(14);

                col.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Total solicitudes", _r.Total.ToString(), ReporteEstilo.AcentoSolicitudes));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Aprobadas", _r.Aprobadas.ToString(), ReporteEstilo.VerdeEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Pendientes", _r.Pendientes.ToString(), ReporteEstilo.AmbarEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Rechazadas", _r.Rechazadas.ToString(), ReporteEstilo.RojoEstado));
                });

                col.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Canceladas", _r.Canceladas.ToString(), ReporteEstilo.RojoEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Finalizadas", _r.Finalizadas.ToString(), ReporteEstilo.VerdeEstado));
                    row.RelativeItem();
                    row.RelativeItem();
                });

                col.Item().Element(c => ReportePdfHelpers.TituloSeccion(c, "Detalle de solicitudes"));

                if (_r.Detalles.Count == 0)
                {
                    col.Item().Element(c => ReportePdfHelpers.SinDatos(
                        c, "No hay solicitudes registradas."));
                }
                else
                {
                    col.Item().Table(table =>
                    {
                        var anchos = new[] { 0.6f, 1.3f, 1.5f, 0.95f, 0.55f, 1f };
                        table.ColumnsDefinition(cols =>
                        {
                            foreach (var a in anchos) cols.RelativeColumn(a);
                        });

                        string[] headers = { "#", "Área", "Destino", "Fecha de viaje", "Pas.", "Estado" };
                        table.Header(header =>
                        {
                            foreach (var h in headers)
                                header.Cell()
                                      .Background(ReporteEstilo.VerdeInstitucional)
                                      .PaddingVertical(7).PaddingHorizontal(8)
                                      .Text(h).FontSize(7.5f).Bold()
                                      .FontColor(ReporteEstilo.Blanco);
                        });

                        for (int i = 0; i < _r.Detalles.Count; i++)
                        {
                            var d = _r.Detalles[i];
                            bool par = i % 2 == 0;
                            string bg = par ? ReporteEstilo.Blanco : ReporteEstilo.GrisFondo;
                            var (badgeFondo, badgeTexto) = ReportePdfHelpers.ColoresEstado(d.Estado);

                            void Celda(IContainer c, string val) =>
                                c.Background(bg)
                                 .BorderBottom(0.4f).BorderColor(ReporteEstilo.GrisBorde)
                                 .PaddingVertical(6).PaddingHorizontal(8)
                                 .AlignMiddle()
                                 .Text(val ?? "—")
                                 .FontSize(8.2f).FontColor(ReporteEstilo.GrisTexto);

                            table.Cell().Element(c => Celda(c, $"#{d.Id:D4}"));
                            table.Cell().Element(c => Celda(c, d.AreaSolicitante));
                            table.Cell().Element(c => Celda(c, d.Destino));
                            table.Cell().Element(c => Celda(c, d.FechaViaje?.ToString("dd/MM/yyyy") ?? "—"));
                            table.Cell().Element(c => Celda(c, d.CantidadPasajeros.ToString()));

                            table.Cell()
                                 .Background(bg)
                                 .BorderBottom(0.4f).BorderColor(ReporteEstilo.GrisBorde)
                                 .PaddingVertical(5).PaddingHorizontal(6)
                                 .AlignMiddle()
                                 .Element(cel =>
                                     cel.Background(badgeFondo)
                                        .PaddingVertical(3).PaddingHorizontal(6)
                                        .Text(d.Estado ?? "—")
                                        .FontSize(7.5f).Bold()
                                        .FontColor(badgeTexto));
                        }

                        table.Footer(f =>
                            f.Cell().ColumnSpan(6).Height(1.5f).Background(ReporteEstilo.VerdeInstitucional));
                    });

                    col.Item().PaddingTop(4).AlignRight()
                       .Text($"Total de registros: {_r.Detalles.Count}")
                       .FontSize(7.5f).FontColor(ReporteEstilo.GrisClaro).Italic();
                }
            });
        }
    }
}