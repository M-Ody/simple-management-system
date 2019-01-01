using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SysGuiApi.Migrations
{
    public partial class AddCityAndUpdateClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Order",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Client",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Client",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Client",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_CityId",
                table: "Client",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_City_CityId",
                table: "Client",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_City_CityId",
                table: "Client");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropIndex(
                name: "IX_Client_CityId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Client");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Order",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
