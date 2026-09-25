using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Metadata;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaMensagens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mensagens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RemetenteId = table.Column<int>(type: "int", nullable: false),
                    DestinatarioId = table.Column<int>(type: "int", nullable: false),
                    Conteudo = table.Column<string>(type: "longtext", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensagens", x => x.Id);
                })
                .Annotation("MySql:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mensagens");
        }
    }
}