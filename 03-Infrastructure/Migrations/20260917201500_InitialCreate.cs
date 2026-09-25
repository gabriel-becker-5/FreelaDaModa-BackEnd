using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    LegalResponsibleFullName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    LegalResponsibleDocument = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    ContactNumber = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Roles = table.Column<string>(type: "json", nullable: false),
                    PublicProfileDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(9)", maxLength: 9, nullable: false),
                    Address = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    AddressNumber = table.Column<int>(type: "int", nullable: false),
                    Neighborhood = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    AdditionalAddressInfo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true),
                    City = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    State = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LegalName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    CompanyName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    CompanyRegistrationDocument = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: false),
                    CoreBusiness = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FreelancerProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BusinessTypeId = table.Column<int>(type: "int", nullable: false),
                    ExperienceYearsId = table.Column<int>(type: "int", nullable: false),
                    WorkshopSizeId = table.Column<int>(type: "int", nullable: false),
                    SpecialtiesIds = table.Column<string>(type: "json", nullable: false),
                    OwnMachinesIds = table.Column<string>(type: "json", nullable: false),
                    HowUsuallyArrangeServicesId = table.Column<int>(type: "int", nullable: false),
                    AvailableTimeId = table.Column<int>(type: "int", nullable: false),
                    FreelancerPreferencesId = table.Column<int>(type: "int", nullable: false),
                    AverageRevenueId = table.Column<int>(type: "int", nullable: false),
                    HasFixedProducer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasOwnCar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreelancerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreelancerProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_CompanyRegistrationDocument",
                table: "CompanyProfiles",
                column: "CompanyRegistrationDocument",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_UserId",
                table: "CompanyProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FreelancerProfiles_UserId",
                table: "FreelancerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_LegalResponsibleDocument",
                table: "Users",
                column: "LegalResponsibleDocument",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "FreelancerProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
