using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class addRemarksToStudentPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionURL",
                table: "StudentCategories");

            migrationBuilder.DropColumn(
                name: "PaymentURL",
                table: "StudentCategories");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "StudentPaymentTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StudentPaymentTypes");

            migrationBuilder.AddColumn<string>(
                name: "AdmissionURL",
                table: "StudentCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentURL",
                table: "StudentCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
