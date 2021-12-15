using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class AcademicSessionRemovedFromAppliedStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppliedStudents_AcademicSessions_AcademicSessionId",
                table: "AppliedStudents");

            //migrationBuilder.DropIndex(
            //    name: "IX_AppliedStudents_AcademicSessionId",
            //    table: "AppliedStudents");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
                table: "AppliedStudents",
                type: "int",
                nullable: true);

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
        }
    }
}
