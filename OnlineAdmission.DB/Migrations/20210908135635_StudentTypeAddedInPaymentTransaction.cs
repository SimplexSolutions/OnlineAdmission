using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class StudentTypeAddedInPaymentTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentType",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentType",
                table: "PaymentTransactions");
        }
    }
}
