using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class MOdelRedesign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCategories_AcademicSessions_AcademicSessionId",
                table: "StudentCategories");

            migrationBuilder.DropIndex(
                name: "IX_StudentCategories_AcademicSessionId",
                table: "StudentCategories");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "StudentCategories");

            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentCategoryId",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "MeritStudents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "AppliedStudents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_AcademicSessionId",
                table: "Students",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_StudentCategoryId",
                table: "PaymentTransactions",
                column: "StudentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritStudents_AcademicSessionId",
                table: "MeritStudents",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedStudents_AcademicSessionId",
                table: "AppliedStudents",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppliedStudents_AcademicSessions_AcademicSessionId",
                table: "AppliedStudents",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeritStudents_AcademicSessions_AcademicSessionId",
                table: "MeritStudents",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_StudentCategories_StudentCategoryId",
                table: "PaymentTransactions",
                column: "StudentCategoryId",
                principalTable: "StudentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AcademicSessions_AcademicSessionId",
                table: "Students",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppliedStudents_AcademicSessions_AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_MeritStudents_AcademicSessions_AcademicSessionId",
                table: "MeritStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_StudentCategories_StudentCategoryId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AcademicSessions_AcademicSessionId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AcademicSessionId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_StudentCategoryId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_MeritStudents_AcademicSessionId",
                table: "MeritStudents");

            migrationBuilder.DropIndex(
                name: "IX_AppliedStudents_AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentCategoryId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "MeritStudents");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "StudentCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCategories_AcademicSessionId",
                table: "StudentCategories",
                column: "AcademicSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCategories_AcademicSessions_AcademicSessionId",
                table: "StudentCategories",
                column: "AcademicSessionId",
                principalTable: "AcademicSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
