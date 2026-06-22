using AduanasExpress.Application.DTOs.Reporte;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    public class ViajesReporteDocumento : ReportePdfDocumentoBase
    {
        private static readonly string[] Meses =
        {
            "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
        };

        private readonly ReporteViajesDTO _r;

        public ViajesReporteDocumento(ReporteViajesDTO reporte, ReporteConfigDTO cfg)
        : base(cfg) => _r = reporte;

        protected override string TituloReporte => "Reporte de Viajes";
        protected override string Subtitulo
            => $"Período: {Meses[_r.Mes]} {_r.Anio}";

        protected override void ComponerContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(14);

                col.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Total de viajes", _r.TotalViajes.ToString(), ReporteEstilo.AcentoViajes));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Finalizados", _r.Completados.ToString(), ReporteEstilo.VerdeEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Pendientes / Aprobados", _r.Pendientes.ToString(), ReporteEstilo.AmbarEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Cancelados", _r.Cancelados.ToString(), ReporteEstilo.RojoEstado));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Pasajeros transportados", _r.TotalPasajeros.ToString(), ReporteEstilo.AcentoViajes));
                });

                col.Item().Element(c => ReportePdfHelpers.TituloSeccion(c, "Detalle de viajes"));

                if (_r.Detalles.Count == 0)
                {
                    col.Item().Element(c => ReportePdfHelpers.SinDatos(
                        c, $"No hay viajes registrados para {Meses[_r.Mes]} {_r.Anio}."));
                }
                else
                {
                    col.Item().Table(table =>
                    {
                        var anchos = new[] { 0.55f, 1.15f, 1.15f, 0.85f, 1.25f, 0.95f, 0.55f, 1f };
                        table.ColumnsDefinition(cols =>
                        {
                            foreach (var a in anchos) cols.RelativeColumn(a);
                        });

                        string[] headers = { "#", "Área", "Destino", "Fecha", "Conductor", "Vehículo", "Pas.", "Estado" };
                        table.Header(header =>
                        {
                            foreach (var h in headers)
                            {
                                header.Cell()
                                      .Background(ReporteEstilo.VerdeInstitucional)
                                      .PaddingVertical(7).PaddingHorizontal(8)
                                      .AlignMiddle()
                                      .Text(h)
                                      .FontSize(7.5f).Bold()
                                      .FontColor(ReporteEstilo.Blanco);
                            }
                        });

                        for (int i = 0; i < _r.Detalles.Count; i++)
                        {
                            var d    = _r.Detalles[i];
                            bool par = i % 2 == 0;
                            string bg = par ? ReporteEstilo.Blanco : ReporteEstilo.GrisFondo;
                            var (badgeFondo, badgeTexto) = ReportePdfHelpers.ColoresEstado(d.Estado);

                            void Celda(IContainer c, string val) =>
                                c.Background(bg)
                                 .BorderBottom(0.4f).BorderColor(ReporteEstilo.GrisBorde)
                                 .PaddingVertical(6).PaddingHorizontal(8)
                                 .AlignMiddle()
                                 .Text(val ?? "—")
                                 .FontSize(8.2f)
                                 .FontColor(ReporteEstilo.GrisTexto);

                            table.Cell().Element(c => Celda(c, $"#{d.Id:D4}"));
                            table.Cell().Element(c => Celda(c, d.AreaSolicitante));
                            table.Cell().Element(c => Celda(c, d.Destino));
                            table.Cell().Element(c => Celda(c, d.FechaViaje?.ToString("dd/MM/yyyy") ?? "—"));
                            table.Cell().Element(c => Celda(c, d.NombreConductor ?? "Sin asignar"));
                            table.Cell().Element(c => Celda(c, d.VehiculoPlaca ?? "Sin asignar"));
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
                            f.Cell().ColumnSpan(8).Height(1.5f).Background(ReporteEstilo.VerdeInstitucional));
                    });

                    col.Item().PaddingTop(4).AlignRight()
                       .Text($"Total de registros: {_r.Detalles.Count}  ·  Pasajeros: {_r.TotalPasajeros}")
                       .FontSize(7.5f).FontColor(ReporteEstilo.GrisClaro).Italic();
                }
            });
        }
    }
}