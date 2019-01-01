using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SysGuiApi.Migrations
{
    public partial class TurnInstallmentsIntoArray : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallmentValue",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InstallmentValueFiscal",
                table: "Order");

            migrationBuilder.AddColumn<double[]>(
                name: "InstallmentValues",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<double[]>(
                name: "InstallmentValuesFiscal",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallmentValues",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InstallmentValuesFiscal",
                table: "Order");

            migrationBuilder.AddColumn<double>(
                name: "InstallmentValue",
                table: "Order",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "InstallmentValueFiscal",
                table: "Order",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
