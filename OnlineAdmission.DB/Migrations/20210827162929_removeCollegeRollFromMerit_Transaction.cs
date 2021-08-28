using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class removeCollegeRollFromMerit_Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollegeRoll",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CollegeRoll",
                table: "MeritStudents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollegeRoll",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollegeRoll",
                table: "MeritStudents",
                type: "int",
                nullable: true);
        }
    }
}
