using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnumMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "direccion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "telefono",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "rol",
                table: "Usuarios",
                newName: "Rol");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Usuarios",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Usuarios",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "apellido",
                table: "Usuarios",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Usuarios",
                newName: "rol");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Usuarios",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Usuarios",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "Usuarios",
                newName: "apellido");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Usuarios",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "direccion",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "telefono",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
