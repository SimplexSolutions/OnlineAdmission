using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class student_Department_District_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DepartmentNameBn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CollegeOrUniversity = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistrictNameBn = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FatherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MotherOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuardianName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GuardianOccupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PresentAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PresentDistrictId = table.Column<int>(type: "int", nullable: false),
                    PermanentAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermanentAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermanentDistrictId = table.Column<int>(type: "int", nullable: false),
                    MailingVillage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MailingPO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MailingPostCode = table.Column<int>(type: "int", nullable: true),
                    MailingPS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MailingDistrictId = table.Column<int>(type: "int", nullable: false),
                    StudentMobile = table.Column<int>(type: "int", nullable: false),
                    GuardianMobile = table.Column<int>(type: "int", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Religion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HSCRoll = table.Column<int>(type: "int", nullable: false),
                    SSCRoll = table.Column<int>(type: "int", nullable: false),
                    HSCGPA = table.Column<double>(type: "float", nullable: false),
                    SSCGPA = table.Column<double>(type: "float", nullable: false),
                    CollegeRoll = table.Column<int>(type: "int", nullable: false),
                    SSCPassingYear = table.Column<int>(type: "int", nullable: false),
                    HSCPassingYear = table.Column<int>(type: "int", nullable: false),
                    SSCBoard = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HSCBoard = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SSCRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HSCRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Districts_MailingDistrictId",
                        column: x => x.MailingDistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Districts_PermanentDistrictId",
                        column: x => x.PermanentDistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Students_Districts_PresentDistrictId",
                        column: x => x.PresentDistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentId",
                table: "Students",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_MailingDistrictId",
                table: "Students",
                column: "MailingDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PermanentDistrictId",
                table: "Students",
                column: "PermanentDistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PresentDistrictId",
                table: "Students",
                column: "PresentDistrictId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Districts");
        }
    }
}
