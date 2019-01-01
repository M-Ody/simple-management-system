using Microsoft.EntityFrameworkCore.Migrations;

namespace SysGuiApi.Migrations
{
    public partial class PermissionsRelateToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionGroupId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PermissionGroup_PermissionGroupId",
                table: "Users",
                column: "PermissionGroupId",
                principalTable: "PermissionGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_PermissionGroup_PermissionGroupId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "PermissionGroupId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Users_PermissionGroup_PermissionGroupId",
                table: "Users",
                column: "PermissionGroupId",
                principalTable: "PermissionGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
