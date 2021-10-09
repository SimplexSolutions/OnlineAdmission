using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class StudentCategoryAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentType",
                table: "PaymentTransactions",
                newName: "StudentCategory");

            migrationBuilder.AddColumn<int>(
                name: "StudentCategory",
                table: "MeritStudents",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentCategory",
                table: "MeritStudents");

            migrationBuilder.RenameColumn(
                name: "StudentCategory",
                table: "PaymentTransactions",
                newName: "StudentType");
        }
    }
}
