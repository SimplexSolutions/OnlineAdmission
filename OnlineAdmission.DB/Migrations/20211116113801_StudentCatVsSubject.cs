using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class StudentCatVsSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectTypes_SubjectTypeId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectTypes");

            migrationBuilder.RenameColumn(
                name: "SubjectTypeId",
                table: "Subjects",
                newName: "StudentCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_SubjectTypeId",
                table: "Subjects",
                newName: "IX_Subjects_StudentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_StudentCategories_StudentCategoryId",
                table: "Subjects",
                column: "StudentCategoryId",
                principalTable: "StudentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_StudentCategories_StudentCategoryId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "StudentCategoryId",
                table: "Subjects",
                newName: "SubjectTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Subjects_StudentCategoryId",
                table: "Subjects",
                newName: "IX_Subjects_SubjectTypeId");

            migrationBuilder.CreateTable(
                name: "SubjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectTypes_SubjectTypeId",
                table: "Subjects",
                column: "SubjectTypeId",
                principalTable: "SubjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
