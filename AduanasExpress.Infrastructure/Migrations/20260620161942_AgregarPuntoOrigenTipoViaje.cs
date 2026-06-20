using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPuntoOrigenTipoViaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraLlegada",
                table: "SolicitudesTransporte");

            migrationBuilder.AddColumn<string>(
                name: "PuntoOrigen",
                table: "SolicitudesTransporte",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TipoViaje",
                table: "SolicitudesTransporte",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencLicencia",
                table: "Conductores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAsignacion",
                table: "Asignaciones",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinalizacion",
                table: "Asignaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "KilometrajeFin",
                table: "Asignaciones",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PuntoOrigen",
                table: "SolicitudesTransporte");

            migrationBuilder.DropColumn(
                name: "TipoViaje",
                table: "SolicitudesTransporte");

            migrationBuilder.DropColumn(
                name: "FechaFinalizacion",
                table: "Asignaciones");

            migrationBuilder.DropColumn(
                name: "KilometrajeFin",
                table: "Asignaciones");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraLlegada",
                table: "SolicitudesTransporte",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencLicencia",
                table: "Conductores",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAsignacion",
                table: "Asignaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
