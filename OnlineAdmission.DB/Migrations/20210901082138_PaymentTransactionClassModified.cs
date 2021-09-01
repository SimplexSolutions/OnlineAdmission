using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class PaymentTransactionClassModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Subjects_SubjectId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_SubjectId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "PaymentTransactions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "PaymentTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_SubjectId",
                table: "PaymentTransactions",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Subjects_SubjectId",
                table: "PaymentTransactions",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
