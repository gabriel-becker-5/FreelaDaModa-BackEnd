using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompletaCamposVaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataPublicacao",
                table: "Vagas");

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Vagas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vagas",
                type: "datetime(6)",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Especialidade",
                table: "Vagas",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Vagas",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Modalidade",
                table: "Vagas",
                type: "int",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrazoConclusao",
                table: "Vagas",
                type: "datetime(6)",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "Especialidade",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "Modalidade",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "PrazoConclusao",
                table: "Vagas");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataPublicacao",
                table: "Vagas",
                type: "datetime(6)",
                nullable: false);
        }
    }
}
