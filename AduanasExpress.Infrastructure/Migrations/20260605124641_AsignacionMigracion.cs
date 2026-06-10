using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AsignacionMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asignaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolicitudId = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    ConductorId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AsignadoPorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asignaciones_Conductores_ConductorId",
                        column: x => x.ConductorId,
                        principalTable: "Conductores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asignaciones_SolicitudesTransporte_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesTransporte",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asignaciones_Usuarios_AsignadoPorId",
                        column: x => x.AsignadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asignaciones_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_AsignadoPorId",
                table: "Asignaciones",
                column: "AsignadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_ConductorId",
                table: "Asignaciones",
                column: "ConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_SolicitudId",
                table: "Asignaciones",
                column: "SolicitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_VehiculoId",
                table: "Asignaciones",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asignaciones");
        }
    }
}
