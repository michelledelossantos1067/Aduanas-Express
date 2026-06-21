namespace AduanasExpress.Infrastructure.Reportes
{
    /// <summary>
    /// Paleta y constantes institucionales. Inspirada en documentación
    /// aduanera oficial: verde oscuro, dorado/bronce discreto, grises fríos.
    /// Sin colores brillantes ni decorativos; todo al servicio de la lectura.
    /// </summary>
    public static class ReporteEstilo
    {
        // Identidad
        public const string Empresa = "AduanasExpress";
        public const string Lema    = "Sistema de Gestión de Transporte";

        // Verde institucional — encabezados, bordes fuertes
        public const string VerdeInstitucional = "#1C3829";
        public const string VerdeClaro         = "#2D5040";
        public const string VerdeMuyClaro      = "#EBF2EE";

        // Bronce/dorado — acento sello; úsalo solo en separadores finos
        public const string Bronce    = "#8A6A2E";
        public const string BronceClaro = "#F5EFE0";

        // Grises — texto, fondos, bordes
        public const string GrisTexto  = "#1F2937";   // casi negro, muy legible
        public const string GrisSecund = "#4B5563";   // texto secundario
        public const string GrisClaro  = "#6B7280";   // pies, notas
        public const string GrisFondo  = "#F8F9FA";   // filas alternadas
        public const string GrisBorde  = "#E2E8F0";   // bordes suaves
        public const string Blanco     = "#FFFFFF";

        // Acentos semáforo — solo para badges de estado, muy discretos
        public const string VerdeEstado   = "#166534";  // fondo #DCFCE7
        public const string AmbarEstado   = "#92400E";  // fondo #FEF3C7
        public const string RojoEstado    = "#991B1B";  // fondo #FEE2E2
        public const string AzulEstado    = "#1E40AF";  // fondo #DBEAFE

        // Colores de fondo para badges
        public const string VerdeBadgeFondo = "#DCFCE7";
        public const string AmbarBadgeFondo = "#FEF3C7";
        public const string RojoBadgeFondo  = "#FEE2E2";
        public const string AzulBadgeFondo  = "#DBEAFE";

        // Acento por reporte (solo KPI cards — borde izquierdo discreto)
        public const string AcentoViajes      = "#1E3A5F";
        public const string AcentoConsumo     = "#7C4A00";
        public const string AcentoSolicitudes = "#134E4A";
        public const string AcentoConductores = "#312E81";
    }
}