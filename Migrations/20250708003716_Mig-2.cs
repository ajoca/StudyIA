using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace study_ia_web_app.Migrations
{
    /// <inheritdoc />
    public partial class Mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TituloSeccion",
                table: "TareasEstudios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TituloSeccion",
                table: "TareasEstudios");
        }
    }
}
