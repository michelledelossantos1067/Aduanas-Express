using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace AduanasExpress.Infrastructure.Migrations
{
    public partial class AgregarPuntoOrigenYTipoViaje : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Columna ya eliminada previamente — omitido para evitar error
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "KilometrajeFin",
                table: "Asignaciones",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
