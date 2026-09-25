using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Metadata;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustesVaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vagas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Vagas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Vagas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AverageRevenueId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "FreelancerProfiles",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BusinessTypeId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FreelancerPreferencesId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasFixedProducer",
                table: "FreelancerProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasOwnCar",
                table: "FreelancerProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HowUsuallyArrangeServicesId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkshopSizeId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Candidaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VagaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    FreelancerId = table.Column<int>(type: "int", nullable: false),
                    DataCandidatura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidaturas_Vagas_VagaId",
                        column: x => x.VagaId,
                        principalTable: "Vagas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notificacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "longtext", nullable: false),
                    Mensagem = table.Column<string>(type: "longtext", nullable: false),
                    Lida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacoes", x => x.Id);
                })
                .Annotation("MySql:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_VagaId",
                table: "Candidaturas",
                column: "VagaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidaturas");

            migrationBuilder.DropTable(
                name: "Notificacoes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Vagas");

            migrationBuilder.DropColumn(
                name: "AverageRevenueId",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "BusinessTypeId",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "FreelancerPreferencesId",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "HasFixedProducer",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "HasOwnCar",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "HowUsuallyArrangeServicesId",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "WorkshopSizeId",
                table: "FreelancerProfiles");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}