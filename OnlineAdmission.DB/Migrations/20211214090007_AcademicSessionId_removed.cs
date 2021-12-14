using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class AcademicSessionId_removed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_AcademicSessions_AcademicSessionId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_AcademicSessionId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "PaymentTransactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_AcademicSessionId",
                table: "PaymentTransactions",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_AcademicSessions_AcademicSessionId",
                table: "PaymentTransactions",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
