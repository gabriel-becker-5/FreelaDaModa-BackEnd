using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _03_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplificaFreelancer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRevenueId",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AverageRevenueId",
                table: "FreelancerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
