using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CambiarDeleteBehaviorConsumo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsumoCombustibles_Vehiculos_VehiculoId",
                table: "ConsumoCombustibles");

            migrationBuilder.CreateTable(
                name: "OtpVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpVerifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_Email",
                table: "OtpVerifications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_OtpVerifications_ExpiryTime",
                table: "OtpVerifications",
                column: "ExpiryTime");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumoCombustibles_Vehiculos_VehiculoId",
                table: "ConsumoCombustibles",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsumoCombustibles_Vehiculos_VehiculoId",
                table: "ConsumoCombustibles");

            migrationBuilder.DropTable(
                name: "OtpVerifications");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumoCombustibles_Vehiculos_VehiculoId",
                table: "ConsumoCombustibles",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
