using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AduanasExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConductorSupervisorMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conductores_Usuarios_SupervisorId1",
                table: "Conductores");

            migrationBuilder.DropIndex(
                name: "IX_Conductores_SupervisorId1",
                table: "Conductores");

            migrationBuilder.DropColumn(
                name: "SupervisorId1",
                table: "Conductores");

            migrationBuilder.AlterColumn<int>(
                name: "SupervisorId",
                table: "Conductores",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Conductores_SupervisorId",
                table: "Conductores",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conductores_Usuarios_SupervisorId",
                table: "Conductores",
                column: "SupervisorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conductores_Usuarios_SupervisorId",
                table: "Conductores");

            migrationBuilder.DropIndex(
                name: "IX_Conductores_SupervisorId",
                table: "Conductores");

            migrationBuilder.AlterColumn<string>(
                name: "SupervisorId",
                table: "Conductores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId1",
                table: "Conductores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Conductores_SupervisorId1",
                table: "Conductores",
                column: "SupervisorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Conductores_Usuarios_SupervisorId1",
                table: "Conductores",
                column: "SupervisorId1",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
