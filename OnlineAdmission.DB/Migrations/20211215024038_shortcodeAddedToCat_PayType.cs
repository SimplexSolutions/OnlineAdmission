using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class shortcodeAddedToCat_PayType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryShortCode",
                table: "StudentCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTypeShortCode",
                table: "PaymentTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryShortCode",
                table: "StudentCategories");

            migrationBuilder.DropColumn(
                name: "PaymentTypeShortCode",
                table: "PaymentTypes");
        }
    }
}
