using Microsoft.EntityFrameworkCore.Migrations;

namespace SysGuiApi.Migrations
{
    public partial class PermissionsOnUserRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermissionGroupId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermissionGroupId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
