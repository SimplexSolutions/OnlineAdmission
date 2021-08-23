using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class MeritStudentModelUpdated_TransactionIdAdded_DeductedAmountAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaidAmaount",
                table: "MeritStudents",
                newName: "DeductedAmaount");

            migrationBuilder.AddColumn<int>(
                name: "PaymentTransactionId",
                table: "MeritStudents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeritStudents_PaymentTransactionId",
                table: "MeritStudents",
                column: "PaymentTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeritStudents_PaymentTransactions_PaymentTransactionId",
                table: "MeritStudents",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeritStudents_PaymentTransactions_PaymentTransactionId",
                table: "MeritStudents");

            migrationBuilder.DropIndex(
                name: "IX_MeritStudents_PaymentTransactionId",
                table: "MeritStudents");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "MeritStudents");

            migrationBuilder.RenameColumn(
                name: "DeductedAmaount",
                table: "MeritStudents",
                newName: "PaidAmaount");
        }
    }
}
