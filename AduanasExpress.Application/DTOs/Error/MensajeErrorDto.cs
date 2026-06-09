namespace AduanasExpress.API.Middleware
{
    public class MensajeErrorDto
    {
        public int Estado { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Detalle { get; set; }
    }
}