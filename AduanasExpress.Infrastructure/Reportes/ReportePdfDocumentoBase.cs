using AduanasExpress.Application.DTOs.Reporte;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AduanasExpress.Infrastructure.Reportes
{
    public abstract class ReportePdfDocumentoBase : IDocument
    {
        protected readonly ReporteConfigDTO Cfg;

        protected ReportePdfDocumentoBase(ReporteConfigDTO cfg)
            => Cfg = cfg ?? new ReporteConfigDTO();

        protected abstract string TituloReporte { get; }
        protected abstract string Subtitulo     { get; }
        protected abstract void   ComponerContenido(IContainer container);

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginHorizontal(40);
                page.MarginTop(32);
                page.MarginBottom(28);
                page.DefaultTextStyle(x => x
                    .FontFamily("Arial")
                    .FontSize(9)
                    .FontColor(ReporteEstilo.GrisTexto));

                page.Header().Element(ComponerEncabezado);
                page.Content().PaddingTop(18).Element(ComponerContenido);
                page.Footer().Element(ComponerPie);
            });
        }

        private void ComponerEncabezado(IContainer container)
        {
            container.Column(col =>
            {
                col.Item()
                   .Background(Cfg.ColorPrimary)
                   .PaddingVertical(14)
                   .Row(row =>
                   {
                       row.RelativeItem().PaddingLeft(18).Column(izq =>
                       {
                           izq.Item()
                              .Text(ReporteEstilo.Empresa)
                              .FontSize(15).Bold()
                              .FontColor(ReporteEstilo.Blanco);

                           izq.Item().PaddingTop(2)
                              .Text(ReporteEstilo.Lema)
                              .FontSize(7.5f)
                              .FontColor("#A8C4B4");
                       });

                       row.ConstantItem(200).PaddingRight(18).Column(der =>
                       {
                           der.Item().AlignRight()
                              .Text(TituloReporte)
                              .FontSize(11).Bold()
                              .FontColor(ReporteEstilo.Blanco);

                           der.Item().PaddingTop(3).AlignRight()
                              .Text(Subtitulo)
                              .FontSize(8)
                              .FontColor("#C8DDD4");
                       });
                   });

                col.Item()
                   .Height(3)
                   .Background(Cfg.ColorAccent);
            });
        }

        private void ComponerPie(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Height(1).Background(ReporteEstilo.GrisBorde);

                col.Item().PaddingTop(6).Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span(ReporteEstilo.Empresa + "  ·  ")
                            .FontSize(7).FontColor(ReporteEstilo.GrisClaro).Bold();
                        text.Span("Generado el ")
                            .FontSize(7).FontColor(ReporteEstilo.GrisClaro);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                            .FontSize(7).FontColor(ReporteEstilo.GrisClaro).Bold();
                    });

                    row.ConstantItem(180).AlignCenter().Text(text =>
                    {
                        text.Span("Documento de uso interno — Confidencial")
                            .FontSize(7).FontColor(ReporteEstilo.GrisClaro).Italic();
                    });

                    row.ConstantItem(70).AlignRight().Text(text =>
                    {
                        text.Span("Página ").FontSize(7).FontColor(ReporteEstilo.GrisClaro);
                        text.CurrentPageNumber().FontSize(7).FontColor(ReporteEstilo.GrisClaro).Bold();
                        text.Span(" de ").FontSize(7).FontColor(ReporteEstilo.GrisClaro);
                        text.TotalPages().FontSize(7).FontColor(ReporteEstilo.GrisClaro).Bold();
                    });
                });
            });
        }
        protected string HeaderTablaColor => Cfg.Estilo == "minimal"
            ? ReporteEstilo.GrisFondo
            : Cfg.ColorPrimary;

        protected string HeaderTablaTexto => Cfg.Estilo == "minimal"
            ? ReporteEstilo.GrisSecund
            : ReporteEstilo.Blanco;

        protected string KpiFondo => Cfg.Estilo == "bold"
            ? Cfg.ColorPrimary
            : ReporteEstilo.GrisFondo;

        protected string KpiTextoValor => Cfg.Estilo == "bold"
            ? ReporteEstilo.Blanco
            : Cfg.ColorPrimary;

        protected string KpiTextoLabel => Cfg.Estilo == "bold"
            ? "#A8C4B4"
            : ReporteEstilo.GrisClaro;
        protected string KpiBordeColor => Cfg.ColorPrimary;
    }
}
