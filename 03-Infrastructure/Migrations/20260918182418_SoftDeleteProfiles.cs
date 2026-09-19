using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LegalResponsibleDocument",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfiles_CompanyRegistrationDocument",
                table: "CompanyProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FreelancerProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CompanyProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LegalResponsibleDocument",
                table: "Users",
                column: "LegalResponsibleDocument");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_CompanyRegistrationDocument",
                table: "CompanyProfiles",
                column: "CompanyRegistrationDocument");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LegalResponsibleDocument",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_CompanyProfiles_CompanyRegistrationDocument",
                table: "CompanyProfiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FreelancerProfiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CompanyProfiles");

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

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_CompanyRegistrationDocument",
                table: "CompanyProfiles",
                column: "CompanyRegistrationDocument",
                unique: true);
        }
    }
}
