using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarModeloMantenimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "KilometrajeFin",
                table: "Asignaciones");

            migrationBuilder.RenameColumn(
                name: "TipoMantenimiento",
                table: "Mantenimientos",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "ProximoMantenimiento",
                table: "Mantenimientos",
                newName: "FechaRealizada");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Mantenimientos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaProgramada",
                table: "Mantenimientos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Kilometraje",
                table: "Mantenimientos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Mantenimientos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "Mantenimientos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "FechaProgramada",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Kilometraje",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Mantenimientos");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "Mantenimientos");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Mantenimientos",
                newName: "TipoMantenimiento");

            migrationBuilder.RenameColumn(
                name: "FechaRealizada",
                table: "Mantenimientos",
                newName: "ProximoMantenimiento");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Mantenimientos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "KilometrajeFin",
                table: "Asignaciones",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
