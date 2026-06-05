using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SolicitudTransporteMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudesTransporte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaSolicitante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CantidadColaboradores = table.Column<int>(type: "int", nullable: false),
                    FechaViaje = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraSalida = table.Column<TimeSpan>(type: "time", nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoViaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    ConductorId = table.Column<int>(type: "int", nullable: false),
                    UsuarioSolicitaId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesTransporte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesTransporte_Conductores_ConductorId",
                        column: x => x.ConductorId,
                        principalTable: "Conductores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesTransporte_Usuarios_UsuarioSolicitaId",
                        column: x => x.UsuarioSolicitaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesTransporte_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesTransporte_ConductorId",
                table: "SolicitudesTransporte",
                column: "ConductorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesTransporte_UsuarioSolicitaId",
                table: "SolicitudesTransporte",
                column: "UsuarioSolicitaId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesTransporte_VehiculoId",
                table: "SolicitudesTransporte",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudesTransporte");
        }
    }
}
