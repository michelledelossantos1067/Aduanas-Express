using AduanasExpress.Application.DTOs.Reporte;
using ClosedXML.Excel;
namespace AduanasExpress.Infrastructure.Reportes
{
    public static class ReporteExcelBuilder
    {
        private static readonly XLColor ColVerde = XLColor.FromHtml(ReporteEstilo.VerdeInstitucional);
        private static readonly XLColor ColBronce = XLColor.FromHtml(ReporteEstilo.Bronce);
        private static readonly XLColor ColBanda = XLColor.FromHtml(ReporteEstilo.GrisFondo);
        private static readonly XLColor ColBorde = XLColor.FromHtml(ReporteEstilo.GrisBorde);
        private static readonly XLColor ColGris = XLColor.FromHtml(ReporteEstilo.GrisClaro);
        private static readonly XLColor ColTexto = XLColor.FromHtml(ReporteEstilo.GrisTexto);
        private static readonly XLColor ColVMClaro = XLColor.FromHtml(ReporteEstilo.VerdeMuyClaro);
        private static readonly XLColor ColBlancoXL = XLColor.White;

        private static readonly string[] Meses =
        {
            "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre",
        };

        private static (XLColor fondo, XLColor texto) ColoresEstado(string estado)
        {
            var v = (estado ?? "").ToLower();
            if (v.Contains("finaliz") || v.Contains("complet") || v.Contains("aprobad"))
                return (XLColor.FromHtml("#DCFCE7"), XLColor.FromHtml(ReporteEstilo.VerdeEstado));
            if (v.Contains("pendiente") || v.Contains("espera"))
                return (XLColor.FromHtml("#FEF3C7"), XLColor.FromHtml(ReporteEstilo.AmbarEstado));
            if (v.Contains("cancel") || v.Contains("rechaz"))
                return (XLColor.FromHtml("#FEE2E2"), XLColor.FromHtml(ReporteEstilo.RojoEstado));
            if (v.Contains("viaje") || v.Contains("proceso"))
                return (XLColor.FromHtml("#DBEAFE"), XLColor.FromHtml(ReporteEstilo.AzulEstado));
            return (XLColor.FromHtml(ReporteEstilo.GrisFondo), ColGris);
        }

        public static byte[] GenerarViajes(ReporteViajesDTO r)
        {
            using var libro = new XLWorkbook();
            var ws = libro.Worksheets.Add("Viajes");
            var cols = new[] { "#", "Área solicitante", "Destino", "Fecha de viaje",
                               "Conductor", "Vehículo", "Pasajeros", "Estado" };

            int fila = EscribirEncabezado(ws, "Reporte de Viajes",
                $"Período: {Meses[r.Mes]} {r.Anio}", cols.Length);

            fila = EscribirKpis(ws, fila, cols.Length, new[]
            {
                ("Total de viajes",    r.TotalViajes.ToString()),
                ("Finalizados",        r.Completados.ToString()),
                ("Pendientes",         r.Pendientes.ToString()),
                ("Cancelados",         r.Cancelados.ToString()),
                ("Pasajeros totales",  r.TotalPasajeros.ToString()),
            });

            fila = EscribirEncabezadosCols(ws, fila, cols);
            int inicio = fila;

            foreach (var d in r.Detalles)
            {
                EscribirFila(ws, fila, new object[]
                {
                    $"#{d.Id:D4}",
                    d.AreaSolicitante ?? "—",
                    d.Destino         ?? "—",
                    d.FechaViaje,
                    d.NombreConductor ?? "Sin asignar",
                    d.VehiculoPlaca   ?? "Sin asignar",
                    d.CantidadPasajeros,
                    d.Estado          ?? "—",
                });
                if (d.FechaViaje.HasValue)
                    ws.Cell(fila, 4).Style.DateFormat.Format = "dd/MM/yyyy";

                var (bf, bt) = ColoresEstado(d.Estado);
                ws.Cell(fila, 8).Style.Fill.BackgroundColor = bf;
                ws.Cell(fila, 8).Style.Font.FontColor = bt;
                ws.Cell(fila, 8).Style.Font.Bold = true;
                ws.Cell(fila, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                fila++;
            }

            AplicarBandas(ws, inicio, fila - 1, cols.Length, estadoCol: 8);
            BordeFinalVerde(ws, fila, cols.Length);
            Finalizar(ws, fila + 1, cols.Length, "Reporte de viajes — uso interno");
            return Bytes(libro);
        }

        public static byte[] GenerarConsumo(ReporteConsumoDTO r)
        {
            using var libro = new XLWorkbook();
            var ws = libro.Worksheets.Add("Consumo");
            var cols = new[] { "Vehículo", "Placa", "Galones", "Costo total", "Registros" };

            int fila = EscribirEncabezado(ws, "Reporte de Consumo de Combustible",
                $"Período: {Meses[r.Mes]} {r.Anio}", cols.Length);

            fila = EscribirKpis(ws, fila, cols.Length, new[]
            {
                ("Costo total",          r.CostoTotal.ToString("C0")),
                ("Galones consumidos",   r.TotalGalones.ToString("N0")),
                ("Costo prom. / galón",  r.CostoPromedioGalon.ToString("C2")),
                ("Vehículos con consumo",r.TotalVehiculos.ToString()),
            });

            fila = EscribirEncabezadosCols(ws, fila, cols);
            int inicio = fila;

            foreach (var d in r.Detalles)
            {
                EscribirFila(ws, fila, new object[]
                {
                    d.VehiculoMarca ?? "—",
                    d.VehiculoPlaca ?? "—",
                    d.TotalGalones,
                    d.CostoTotal,
                    d.TotalRegistros,
                });
                ws.Cell(fila, 3).Style.NumberFormat.Format = "#,##0.0";
                ws.Cell(fila, 4).Style.NumberFormat.Format = "RD$ #,##0.00";
                ws.Cell(fila, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(fila, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(fila, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                fila++;
            }

            AplicarBandas(ws, inicio, fila - 1, cols.Length);

            ws.Range(fila, 1, fila, 2).Merge();
            ws.Cell(fila, 1).Value = "TOTAL";
            ws.Cell(fila, 1).Style.Font.Bold = true;
            ws.Cell(fila, 1).Style.Font.FontColor = ColVerde;
            ws.Cell(fila, 3).Value = r.TotalGalones;
            ws.Cell(fila, 3).Style.NumberFormat.Format = "#,##0.0";
            ws.Cell(fila, 4).Value = r.CostoTotal;
            ws.Cell(fila, 4).Style.NumberFormat.Format = "RD$ #,##0.00";
            ws.Range(fila, 1, fila, cols.Length).Style.Fill.BackgroundColor = ColVMClaro;
            ws.Range(fila, 1, fila, cols.Length).Style.Font.Bold = true;
            ws.Range(fila, 1, fila, cols.Length).Style.Font.FontColor = ColVerde;
            ws.Range(fila, 1, fila, cols.Length).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            ws.Range(fila, 1, fila, cols.Length).Style.Border.TopBorderColor = ColVerde;
            fila++;

            BordeFinalVerde(ws, fila, cols.Length);
            Finalizar(ws, fila + 1, cols.Length, "Reporte de consumo de combustible — uso interno");
            return Bytes(libro);
        }

        public static byte[] GenerarSolicitudes(ReporteSolicitudesDTO r)
        {
            using var libro = new XLWorkbook();
            var ws = libro.Worksheets.Add("Solicitudes");
            var cols = new[] { "#", "Área solicitante", "Destino", "Fecha de viaje", "Pasajeros", "Estado" };

            int fila = EscribirEncabezado(ws, "Reporte de Solicitudes", "Histórico general", cols.Length);

            fila = EscribirKpis(ws, fila, cols.Length, new[]
            {
                ("Total",      r.Total.ToString()),
                ("Aprobadas",  r.Aprobadas.ToString()),
                ("Pendientes", r.Pendientes.ToString()),
                ("Rechazadas", r.Rechazadas.ToString()),
                ("Canceladas", r.Canceladas.ToString()),
                ("Finalizadas",r.Finalizadas.ToString()),
            });

            fila = EscribirEncabezadosCols(ws, fila, cols);
            int inicio = fila;

            foreach (var d in r.Detalles)
            {
                EscribirFila(ws, fila, new object[]
                {
                    $"#{d.Id:D4}",
                    d.AreaSolicitante ?? "—",
                    d.Destino         ?? "—",
                    d.FechaViaje,
                    d.CantidadPasajeros,
                    d.Estado          ?? "—",
                });
                if (d.FechaViaje.HasValue)
                    ws.Cell(fila, 4).Style.DateFormat.Format = "dd/MM/yyyy";

                var (bf, bt) = ColoresEstado(d.Estado);
                ws.Cell(fila, 6).Style.Fill.BackgroundColor = bf;
                ws.Cell(fila, 6).Style.Font.FontColor = bt;
                ws.Cell(fila, 6).Style.Font.Bold = true;
                ws.Cell(fila, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                fila++;
            }

            AplicarBandas(ws, inicio, fila - 1, cols.Length, estadoCol: 6);
            BordeFinalVerde(ws, fila, cols.Length);
            Finalizar(ws, fila + 1, cols.Length, "Reporte de solicitudes — uso interno");
            return Bytes(libro);
        }
        public static byte[] GenerarConductores(ReporteConductoresDTO r)
        {
            using var libro = new XLWorkbook();
            var ws = libro.Worksheets.Add("Conductores");
            var cols = new[] { "Conductor", "Núm. licencia", "Viajes", "Pasajeros", "Último viaje" };

            int fila = EscribirEncabezado(ws, "Reporte de Conductores", "Histórico general", cols.Length);

            fila = EscribirKpis(ws, fila, cols.Length, new[]
            {
                ("Conductores activos",     r.TotalConductores.ToString()),
                ("Total de viajes",         r.TotalViajes.ToString()),
                ("Pasajeros transportados", r.TotalPasajeros.ToString()),
                ("Prom. pasajeros / viaje", r.PromedioPasajerosPorViaje.ToString("N1")),
            });

            fila = EscribirEncabezadosCols(ws, fila, cols);
            int inicio = fila;

            foreach (var d in r.Detalles)
            {
                EscribirFila(ws, fila, new object[]
                {
                    d.NombreConductor ?? "—",
                    d.Licencia        ?? "—",
                    d.TotalViajes,
                    d.TotalPasajeros,
                    d.UltimoViaje,
                });
                if (d.UltimoViaje.HasValue)
                    ws.Cell(fila, 5).Style.DateFormat.Format = "dd/MM/yyyy";
                ws.Cell(fila, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(fila, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                fila++;
            }

            AplicarBandas(ws, inicio, fila - 1, cols.Length);
            BordeFinalVerde(ws, fila, cols.Length);
            Finalizar(ws, fila + 1, cols.Length, "Reporte de conductores — uso interno");
            return Bytes(libro);
        }
        private static int EscribirEncabezado(IXLWorksheet ws, string titulo, string sub, int numCols)
        {
            ws.Range(1, 1, 1, numCols).Merge().Style
              .Fill.SetBackgroundColor(ColVerde);
            ws.Cell(1, 1).Value = ReporteEstilo.Empresa;
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Font.FontColor = ColBlancoXL;
            ws.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(1, 1).Style.Alignment.Indent = 1;
            ws.Row(1).Height = 30;

            ws.Range(2, 1, 2, numCols).Merge().Style
              .Fill.SetBackgroundColor(ColVerde);
            ws.Cell(2, 1).Value = ReporteEstilo.Lema;
            ws.Cell(2, 1).Style.Font.FontSize = 9;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#A8C4B4");
            ws.Cell(2, 1).Style.Alignment.Indent = 1;
            ws.Row(2).Height = 16;

            ws.Range(3, 1, 3, numCols).Merge().Style
              .Fill.SetBackgroundColor(ColBronce);
            ws.Row(3).Height = 3;

            ws.Row(4).Height = 8;

            ws.Range(5, 1, 5, numCols).Merge();
            ws.Cell(5, 1).Value = titulo;
            ws.Cell(5, 1).Style.Font.Bold = true;
            ws.Cell(5, 1).Style.Font.FontSize = 13;
            ws.Cell(5, 1).Style.Font.FontColor = ColVerde;
            ws.Row(5).Height = 22;

            ws.Range(6, 1, 6, numCols).Merge();
            ws.Cell(6, 1).Value = sub;
            ws.Cell(6, 1).Style.Font.FontSize = 9;
            ws.Cell(6, 1).Style.Font.FontColor = ColGris;
            ws.Cell(6, 1).Style.Font.Italic = true;
            ws.Row(6).Height = 16;

            ws.Row(7).Height = 6;

            return 8;
        }
        private static int EscribirKpis(
            IXLWorksheet ws, int fila, int numCols,
            (string Etiqueta, string Valor)[] kpis)
        {
            for (int i = 0; i < kpis.Length; i++)
            {
                int col = i + 1;
                if (col > numCols) break;

                var cEtiq = ws.Cell(fila, col);
                cEtiq.Value = kpis[i].Etiqueta;
                cEtiq.Style.Font.FontSize = 7.5;
                cEtiq.Style.Font.FontColor = ColGris;
                cEtiq.Style.Fill.BackgroundColor = ColVMClaro;
                cEtiq.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cEtiq.Style.Border.OutsideBorderColor = XLColor.FromHtml(ReporteEstilo.GrisBorde);
                cEtiq.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                cEtiq.Style.Alignment.Indent = 1;

                var cVal = ws.Cell(fila + 1, col);
                cVal.Value = kpis[i].Valor;
                cVal.Style.Font.FontSize = 12;
                cVal.Style.Font.Bold = true;
                cVal.Style.Font.FontColor = ColVerde;
                cVal.Style.Fill.BackgroundColor = ColVMClaro;
                cVal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cVal.Style.Border.OutsideBorderColor = XLColor.FromHtml(ReporteEstilo.GrisBorde);
                cVal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                cVal.Style.Alignment.Indent = 1;
            }

            ws.Row(fila).Height = 14;
            ws.Row(fila + 1).Height = 22;
            ws.Row(fila + 2).Height = 8;

            return fila + 3;
        }

        private static int EscribirEncabezadosCols(IXLWorksheet ws, int fila, string[] cols)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                var c = ws.Cell(fila, i + 1);
                c.Value = cols[i];
                c.Style.Font.Bold = true;
                c.Style.Font.FontSize = 8.5;
                c.Style.Font.FontColor = ColBlancoXL;
                c.Style.Fill.BackgroundColor = ColVerde;
                c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                c.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                c.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                c.Style.Border.BottomBorderColor = ColBronce;
            }
            ws.Row(fila).Height = 22;
            ws.SheetView.FreezeRows(1);
            return fila + 1;
        }

        private static void EscribirFila(IXLWorksheet ws, int fila, object[] valores)
        {
            for (int i = 0; i < valores.Length; i++)
            {
                var c = ws.Cell(fila, i + 1);
                var v = valores[i];

                if (v == null)
                {
                    c.Value = "—";
                }
                else if (v is string s)
                {
                    c.Value = s;
                }
                else if (v is int n)
                {
                    c.Value = n;
                }
                else if (v is decimal dec)
                {
                    c.Value = dec;
                }
                else if (v is double db)
                {
                    c.Value = db;
                }
                else if (v is DateTime dt)
                {
                    c.Value = dt;
                }
                else
                {
                    var tipo = v.GetType();
                    if (tipo == typeof(DateTime?))
                    {
                        var dtn = (DateTime?)v;
                        c.Value = dtn.HasValue ? (XLCellValue)dtn.Value : "—";
                    }
                    else
                    {
                        c.Value = v.ToString() ?? "—";
                    }
                }

                c.Style.Font.FontSize = 8.5;
                c.Style.Font.FontColor = ColTexto;
                c.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            ws.Row(fila).Height = 18;
        }

        private static void AplicarBandas(
            IXLWorksheet ws, int inicio, int fin, int numCols, int estadoCol = 0)
        {
            for (int f = inicio; f <= fin; f++)
            {
                bool esBanda = (f - inicio) % 2 == 1;
                for (int c = 1; c <= numCols; c++)
                {
                    if (c == estadoCol) continue;
                    if (esBanda)
                        ws.Cell(f, c).Style.Fill.BackgroundColor = ColBanda;
                }
                ws.Range(f, 1, f, numCols).Style.Border.BottomBorder = XLBorderStyleValues.Hair;
                ws.Range(f, 1, f, numCols).Style.Border.BottomBorderColor = ColBorde;
            }
        }

        private static void BordeFinalVerde(IXLWorksheet ws, int fila, int numCols)
        {
            ws.Range(fila, 1, fila, numCols).Style.Border.TopBorder = XLBorderStyleValues.Medium;
            ws.Range(fila, 1, fila, numCols).Style.Border.TopBorderColor = ColVerde;
        }
        private static void Finalizar(IXLWorksheet ws, int filaNotas, int numCols, string nota)
        {
            ws.Row(filaNotas).Height = 8;

            ws.Range(filaNotas + 1, 1, filaNotas + 1, numCols).Merge();
            var pie = ws.Cell(filaNotas + 1, 1);
            pie.Value = $"{nota}   ·   Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
            pie.Style.Font.FontSize = 7.5;
            pie.Style.Font.FontColor = ColGris;
            pie.Style.Font.Italic = true;

            ws.Columns(1, numCols).AdjustToContents();
            for (int c = 1; c <= numCols; c++)
            {
                if (ws.Column(c).Width < 12) ws.Column(c).Width = 12;
                if (ws.Column(c).Width > 42) ws.Column(c).Width = 42;
            }

            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.PaperSize = XLPaperSize.LetterPaper;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.SetRowsToRepeatAtTop(1, 1);
        }
        private static byte[] Bytes(XLWorkbook libro)
        {
            using var ms = new MemoryStream();
            libro.SaveAs(ms);
            return ms.ToArray();
        }
    }
}