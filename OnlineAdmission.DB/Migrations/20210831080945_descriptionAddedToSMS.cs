using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class descriptionAddedToSMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SMSModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SMSModels");
        }
    }
}
