using Microsoft.EntityFrameworkCore.Migrations;

namespace SysGuiApi.Migrations
{
    public partial class UpdateOrderAddFiscals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CloseningPaid",
                table: "Order",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "CloseningPayment",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "CloseningPaymentFiscal",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "FirstPaymentFiscal",
                table: "Order",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "InstallmentValueFiscal",
                table: "Order",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseningPaid",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CloseningPayment",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CloseningPaymentFiscal",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "FirstPaymentFiscal",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "InstallmentValueFiscal",
                table: "Order");
        }
    }
}
