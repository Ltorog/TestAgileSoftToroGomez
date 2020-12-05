using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAgileSoft.Migrations
{
    public partial class Primeramigracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TSTAS_Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSTAS_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TSTAG_Tareas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSTAG_Tareas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TSTAG_Tareas_TSTAS_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TSTAS_Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TSTAG_Tareas_UsuarioId",
                table: "TSTAG_Tareas",
                column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TSTAG_Tareas");

            migrationBuilder.DropTable(
                name: "TSTAS_Usuarios");
        }
    }
}
