using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineAdmission.DB.Migrations
{
    public partial class BasePropsToPaymentTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PaymentTransactions");
        }
    }
}
