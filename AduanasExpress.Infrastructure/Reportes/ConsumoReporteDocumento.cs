using AduanasExpress.Application.DTOs.Reporte;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    public class ConsumoReporteDocumento : ReportePdfDocumentoBase
    {
        private static readonly string[] Meses =
        {
            "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
        };

        private readonly ReporteConsumoDTO _r;

        public ConsumoReporteDocumento(ReporteConsumoDTO reporte, ReporteConfigDTO cfg)
        : base(cfg) => _r = reporte;

        protected override string TituloReporte => "Reporte de Consumo de Combustible";
        protected override string Subtitulo      => $"Período: {Meses[_r.Mes]} {_r.Anio}";

        protected override void ComponerContenido(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(14);

                col.Item().Row(row =>
                {
                    row.Spacing(8);
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Costo total", _r.CostoTotal.ToString("C0"), ReporteEstilo.AcentoConsumo));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Galones consumidos", _r.TotalGalones.ToString("N0"), ReporteEstilo.AcentoConsumo));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Costo prom. / galón", _r.CostoPromedioGalon.ToString("C2"), ReporteEstilo.AcentoConsumo));
                    row.RelativeItem().Element(c => ReportePdfHelpers.TarjetaKpi(
                        c, "Vehículos con consumo", _r.TotalVehiculos.ToString(), ReporteEstilo.AcentoConsumo));
                });

                col.Item().Element(c => ReportePdfHelpers.TituloSeccion(c, "Detalle por vehículo"));

                if (_r.Detalles.Count == 0)
                {
                    col.Item().Element(c => ReportePdfHelpers.SinDatos(
                        c, $"No hay registros de consumo para {Meses[_r.Mes]} {_r.Anio}."));
                }
                else
                {
                    col.Item().Element(c => ReportePdfHelpers.Tabla(
                        c,
                        new[] { "Vehículo", "Placa", "Galones", "Costo total", "Registros" },
                        _r.Detalles.Select(d => new[]
                        {
                            d.VehiculoMarca    ?? "—",
                            d.VehiculoPlaca    ?? "—",
                            d.TotalGalones.ToString("N1"),
                            d.CostoTotal.ToString("C0"),
                            d.TotalRegistros.ToString(),
                        }).ToList(),
                        anchos: new[] { 1.6f, 1f, 0.9f, 1.1f, 0.8f },
                        columnasDerecha: new[] { 2, 3, 4 }
                    ));

                    col.Item()
                       .Border(0.5f).BorderColor(ReporteEstilo.VerdeInstitucional)
                       .Background(ReporteEstilo.VerdeMuyClaro)
                       .PaddingVertical(7).PaddingHorizontal(8)
                       .Row(row =>
                       {
                           row.RelativeItem()
                              .Text("TOTAL")
                              .FontSize(8.5f).Bold()
                              .FontColor(ReporteEstilo.VerdeInstitucional);

                           row.ConstantItem(80).AlignRight()
                              .Text(_r.TotalGalones.ToString("N1") + " gal.")
                              .FontSize(8.5f).Bold()
                              .FontColor(ReporteEstilo.VerdeInstitucional);

                           row.ConstantItem(100).AlignRight()
                              .Text(_r.CostoTotal.ToString("C0"))
                              .FontSize(8.5f).Bold()
                              .FontColor(ReporteEstilo.VerdeInstitucional);
                       });
                }
            });
        }
    }
}