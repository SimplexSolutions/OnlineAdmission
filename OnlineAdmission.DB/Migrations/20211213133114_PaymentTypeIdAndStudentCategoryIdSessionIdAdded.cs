using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class PaymentTypeIdAndStudentCategoryIdSessionIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentCategory",
                table: "PaymentTransactions",
                newName: "PaymentTypeId");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "PaymentTransactions",
                newName: "AcademicSessionId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_AcademicSessions_AcademicSessionId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_AcademicSessionId",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "PaymentTypeId",
                table: "PaymentTransactions",
                newName: "StudentCategory");

            migrationBuilder.RenameColumn(
                name: "AcademicSessionId",
                table: "PaymentTransactions",
                newName: "PaymentType");
        }
    }
}
