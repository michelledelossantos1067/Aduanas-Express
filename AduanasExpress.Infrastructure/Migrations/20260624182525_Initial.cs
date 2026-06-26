using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsSistema = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Permitido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermisos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolPermisos_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Descripcion", "EsSistema", "Icono", "IsActive", "Nombre" },
                values: new object[,]
                {
                    { 1, "Acceso total al sistema", true, null, true, "Administrador" },
                    { 2, "Acceso parcial", true, null, true, "Supervisor" },
                    { 3, "Registrar solicitudes y visualizar todo", true, null, true, "Operador" }
                });

            migrationBuilder.InsertData(
                table: "RolPermisos",
                columns: new[] { "Id", "Accion", "Modulo", "Permitido", "RolId" },
                values: new object[,]
                {
                    { 1, "ver", "vehiculos", true, 1 },
                    { 2, "crear", "vehiculos", true, 1 },
                    { 3, "editar", "vehiculos", true, 1 },
                    { 4, "cancelar", "vehiculos", true, 1 },
                    { 5, "ver", "conductores", true, 1 },
                    { 6, "crear", "conductores", true, 1 },
                    { 7, "editar", "conductores", true, 1 },
                    { 8, "cancelar", "conductores", true, 1 },
                    { 9, "ver", "solicitudes", true, 1 },
                    { 10, "crear", "solicitudes", true, 1 },
                    { 11, "editar", "solicitudes", true, 1 },
                    { 12, "cancelar", "solicitudes", true, 1 },
                    { 13, "ver", "asignaciones", true, 1 },
                    { 14, "asignar", "asignaciones", true, 1 },
                    { 15, "editar", "asignaciones", true, 1 },
                    { 16, "cancelar", "asignaciones", true, 1 },
                    { 17, "ver", "reportes", true, 1 },
                    { 18, "exportar", "reportes", true, 1 },
                    { 19, "estadisticas", "reportes", true, 1 },
                    { 20, "ver", "consumo-combustible", true, 1 },
                    { 21, "crear", "consumo-combustible", true, 1 },
                    { 22, "editar", "consumo-combustible", true, 1 },
                    { 23, "eliminar", "consumo-combustible", true, 1 },
                    { 24, "ver", "usuarios", true, 1 },
                    { 25, "crear", "usuarios", true, 1 },
                    { 26, "editar", "usuarios", true, 1 },
                    { 27, "cancelar", "usuarios", true, 1 },
                    { 28, "ver", "vehiculos", true, 2 },
                    { 29, "crear", "vehiculos", false, 2 },
                    { 30, "editar", "vehiculos", true, 2 },
                    { 31, "cancelar", "vehiculos", false, 2 },
                    { 32, "ver", "conductores", true, 2 },
                    { 33, "crear", "conductores", true, 2 },
                    { 34, "editar", "conductores", true, 2 },
                    { 35, "cancelar", "conductores", false, 2 },
                    { 36, "ver", "solicitudes", true, 2 },
                    { 37, "crear", "solicitudes", true, 2 },
                    { 38, "editar", "solicitudes", true, 2 },
                    { 39, "cancelar", "solicitudes", false, 2 },
                    { 40, "ver", "asignaciones", true, 2 },
                    { 41, "asignar", "asignaciones", true, 2 },
                    { 42, "editar", "asignaciones", true, 2 },
                    { 43, "cancelar", "asignaciones", false, 2 },
                    { 44, "ver", "reportes", true, 2 },
                    { 45, "exportar", "reportes", true, 2 },
                    { 46, "estadisticas", "reportes", false, 2 },
                    { 47, "ver", "consumo-combustible", true, 2 },
                    { 48, "crear", "consumo-combustible", true, 2 },
                    { 49, "editar", "consumo-combustible", true, 2 },
                    { 50, "eliminar", "consumo-combustible", false, 2 },
                    { 51, "ver", "usuarios", true, 2 },
                    { 52, "crear", "usuarios", false, 2 },
                    { 53, "editar", "usuarios", false, 2 },
                    { 54, "cancelar", "usuarios", false, 2 },
                    { 55, "ver", "vehiculos", true, 3 },
                    { 56, "crear", "vehiculos", false, 3 },
                    { 57, "editar", "vehiculos", false, 3 },
                    { 58, "cancelar", "vehiculos", false, 3 },
                    { 59, "ver", "conductores", true, 3 },
                    { 60, "crear", "conductores", false, 3 },
                    { 61, "editar", "conductores", false, 3 },
                    { 62, "cancelar", "conductores", false, 3 },
                    { 63, "ver", "solicitudes", true, 3 },
                    { 64, "crear", "solicitudes", true, 3 },
                    { 65, "editar", "solicitudes", false, 3 },
                    { 66, "cancelar", "solicitudes", false, 3 },
                    { 67, "ver", "consumo-combustible", true, 3 },
                    { 68, "crear", "consumo-combustible", false, 3 },
                    { 69, "editar", "consumo-combustible", false, 3 },
                    { 70, "eliminar", "consumo-combustible", false, 3 },
                    { 71, "ver", "asignaciones", true, 3 },
                    { 72, "asignar", "asignaciones", false, 3 },
                    { 73, "editar", "asignaciones", false, 3 },
                    { 74, "cancelar", "asignaciones", false, 3 },
                    { 75, "ver", "reportes", true, 3 },
                    { 76, "exportar", "reportes", false, 3 },
                    { 77, "estadisticas", "reportes", false, 3 },
                    { 78, "ver", "usuarios", true, 3 },
                    { 79, "crear", "usuarios", false, 3 },
                    { 80, "editar", "usuarios", false, 3 },
                    { 81, "cancelar", "usuarios", false, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Rol",
                table: "Usuarios",
                column: "Rol");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_RolId_Modulo_Accion",
                table: "RolPermisos",
                columns: new[] { "RolId", "Modulo", "Accion" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_Rol",
                table: "Usuarios",
                column: "Rol",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_Rol",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Rol",
                table: "Usuarios");
        }
    }
}
