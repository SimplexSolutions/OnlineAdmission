using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class StudentCategoryAndSessionAddedToAppliedStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicSessionId",
                table: "AppliedStudents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentCategoryId",
                table: "AppliedStudents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcademicSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSession", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppliedStudents_AcademicSessionId",
                table: "AppliedStudents",
                column: "AcademicSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedStudents_StudentCategoryId",
                table: "AppliedStudents",
                column: "StudentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppliedStudents_AcademicSession_AcademicSessionId",
                table: "AppliedStudents",
                column: "AcademicSessionId",
                principalTable: "AcademicSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppliedStudents_StudentCategories_StudentCategoryId",
                table: "AppliedStudents",
                column: "StudentCategoryId",
                principalTable: "StudentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppliedStudents_AcademicSession_AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_AppliedStudents_StudentCategories_StudentCategoryId",
                table: "AppliedStudents");

            migrationBuilder.DropTable(
                name: "AcademicSession");

            migrationBuilder.DropIndex(
                name: "IX_AppliedStudents_AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropIndex(
                name: "IX_AppliedStudents_StudentCategoryId",
                table: "AppliedStudents");

            migrationBuilder.DropColumn(
                name: "AcademicSessionId",
                table: "AppliedStudents");

            migrationBuilder.DropColumn(
                name: "StudentCategoryId",
                table: "AppliedStudents");
        }
    }
}
