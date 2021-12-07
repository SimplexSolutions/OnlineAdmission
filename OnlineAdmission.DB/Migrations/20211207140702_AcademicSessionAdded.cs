using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class AcademicSessionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppliedStudents_AcademicSession_AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicSession",
                table: "AcademicSession");

            migrationBuilder.RenameTable(
                name: "AcademicSession",
                newName: "AcademicSessions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicSessions",
                table: "AcademicSessions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppliedStudents_AcademicSessions_AcademicSessionId",
                table: "AppliedStudents",
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicSessions",
                table: "AcademicSessions");

            migrationBuilder.RenameTable(
                name: "AcademicSessions",
                newName: "AcademicSession");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicSession",
                table: "AcademicSession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppliedStudents_AcademicSession_AcademicSessionId",
                table: "AppliedStudents",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
