using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EliminarVehiculoConductorDeSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesTransporte_Conductores_ConductorId",
                table: "SolicitudesTransporte");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesTransporte_Vehiculos_VehiculoId",
                table: "SolicitudesTransporte");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesTransporte_ConductorId",
                table: "SolicitudesTransporte");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesTransporte_VehiculoId",
                table: "SolicitudesTransporte");

            migrationBuilder.DropColumn(
                name: "ConductorId",
                table: "SolicitudesTransporte");

            migrationBuilder.DropColumn(
                name: "VehiculoId",
                table: "SolicitudesTransporte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConductorId",
                table: "SolicitudesTransporte",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VehiculoId",
                table: "SolicitudesTransporte",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesTransporte_ConductorId",
                table: "SolicitudesTransporte",
                column: "ConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesTransporte_VehiculoId",
                table: "SolicitudesTransporte",
                column: "VehiculoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesTransporte_Conductores_ConductorId",
                table: "SolicitudesTransporte",
                column: "ConductorId",
                principalTable: "Conductores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesTransporte_Vehiculos_VehiculoId",
                table: "SolicitudesTransporte",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id");
        }
    }
}
