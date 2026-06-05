using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConsumoCombustibleMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumoCombustibles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Galones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoPorGalon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumoCombustibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumoCombustibles_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumoCombustibles_VehiculoId",
                table: "ConsumoCombustibles",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumoCombustibles");
        }
    }
}
