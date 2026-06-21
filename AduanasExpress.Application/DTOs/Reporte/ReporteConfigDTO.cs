namespace AduanasExpress.Application.DTOs.Reporte
{
    public class ReporteConfigDTO
    {
        public string Estilo { get; set; } = "light";
        public string ColorPrimary { get; set; } = "#1C3829";
        public string ColorAccent { get; set; } = "#8A6A2E";
        public string ColorLight => LightenHex(ColorPrimary, 0.92f);
        private static string LightenHex(string hex, float factor)
        {
            try
            {
                hex = hex.TrimStart('#');
                int r = Convert.ToInt32(hex[..2], 16);
                int g = Convert.ToInt32(hex[2..4], 16);
                int b = Convert.ToInt32(hex[4..6], 16);
                r = (int)(r + (255 - r) * factor);
                g = (int)(g + (255 - g) * factor);
                b = (int)(b + (255 - b) * factor);
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            catch { return "#EBF2EE"; }
        }
    }
}