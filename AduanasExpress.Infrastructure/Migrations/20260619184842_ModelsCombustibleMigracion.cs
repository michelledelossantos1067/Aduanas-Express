using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelsCombustibleMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaViaje",
                table: "SolicitudesTransporte",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraLlegada",
                table: "SolicitudesTransporte",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Mantenimientos",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "ConsumoCombustibles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "SolicitudId",
                table: "ConsumoCombustibles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencLicencia",
                table: "Conductores",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Asignaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumoCombustibles_SolicitudId",
                table: "ConsumoCombustibles",
                column: "SolicitudId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumoCombustibles_SolicitudesTransporte_SolicitudId",
                table: "ConsumoCombustibles",
                column: "SolicitudId",
                principalTable: "SolicitudesTransporte",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsumoCombustibles_SolicitudesTransporte_SolicitudId",
                table: "ConsumoCombustibles");

            migrationBuilder.DropIndex(
                name: "IX_ConsumoCombustibles_SolicitudId",
                table: "ConsumoCombustibles");

            migrationBuilder.DropColumn(
                name: "HoraLlegada",
                table: "SolicitudesTransporte");

            migrationBuilder.DropColumn(
                name: "SolicitudId",
                table: "ConsumoCombustibles");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Asignaciones");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaViaje",
                table: "SolicitudesTransporte",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Mantenimientos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "ConsumoCombustibles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencLicencia",
                table: "Conductores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
