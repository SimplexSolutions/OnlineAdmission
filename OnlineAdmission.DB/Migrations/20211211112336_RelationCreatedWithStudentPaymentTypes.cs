using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class RelationCreatedWithStudentPaymentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeritTypeId",
                table: "StudentPaymentTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentPaymentTypes_MeritTypeId",
                table: "StudentPaymentTypes",
                column: "MeritTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentPaymentTypes_MeritTypes_MeritTypeId",
                table: "StudentPaymentTypes",
                column: "MeritTypeId",
                principalTable: "MeritTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentPaymentTypes_MeritTypes_MeritTypeId",
                table: "StudentPaymentTypes");

            migrationBuilder.DropIndex(
                name: "IX_StudentPaymentTypes_MeritTypeId",
                table: "StudentPaymentTypes");

            migrationBuilder.DropColumn(
                name: "MeritTypeId",
                table: "StudentPaymentTypes");
        }
    }
}
