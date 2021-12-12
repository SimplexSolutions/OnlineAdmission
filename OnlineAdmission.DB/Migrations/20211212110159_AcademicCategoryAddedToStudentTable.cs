using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class AcademicCategoryAddedToStudentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentCategory",
                table: "Students",
                newName: "StudentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentCategoryId",
                table: "Students",
                column: "StudentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentCategories_StudentCategoryId",
                table: "Students",
                column: "StudentCategoryId",
                principalTable: "StudentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentCategories_StudentCategoryId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_StudentCategoryId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "StudentCategoryId",
                table: "Students",
                newName: "StudentCategory");
        }
    }
}
