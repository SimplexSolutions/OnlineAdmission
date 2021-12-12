using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class MeritStudentModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentCategory",
                table: "MeritStudents",
                newName: "StudentCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "MeritTypeId",
                table: "MeritStudents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeritStudents_MeritTypeId",
                table: "MeritStudents",
                column: "MeritTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MeritStudents_StudentCategoryId",
                table: "MeritStudents",
                column: "StudentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MeritStudents_MeritTypes_MeritTypeId",
                table: "MeritStudents",
                column: "MeritTypeId",
                principalTable: "MeritTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeritStudents_StudentCategories_StudentCategoryId",
                table: "MeritStudents",
                column: "StudentCategoryId",
                principalTable: "StudentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeritStudents_MeritTypes_MeritTypeId",
                table: "MeritStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_MeritStudents_StudentCategories_StudentCategoryId",
                table: "MeritStudents");

            migrationBuilder.DropIndex(
                name: "IX_MeritStudents_MeritTypeId",
                table: "MeritStudents");

            migrationBuilder.DropIndex(
                name: "IX_MeritStudents_StudentCategoryId",
                table: "MeritStudents");

            migrationBuilder.DropColumn(
                name: "MeritTypeId",
                table: "MeritStudents");

            migrationBuilder.RenameColumn(
                name: "StudentCategoryId",
                table: "MeritStudents",
                newName: "StudentCategory");
        }
    }
}
