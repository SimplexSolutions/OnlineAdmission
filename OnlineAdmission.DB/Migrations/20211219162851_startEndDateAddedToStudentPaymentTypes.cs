using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class startEndDateAddedToStudentPaymentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StudentPaymentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StudentPaymentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentPaymentTypes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentPaymentTypes");
        }
    }
}
