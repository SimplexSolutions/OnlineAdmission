using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class StudentModelModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HONSPassingYear",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HONSRemark",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HonsGPA",
                table: "Students",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "Students",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HONSPassingYear",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "HONSRemark",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "HonsGPA",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "University",
                table: "Students");
        }
    }
}
