using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Migrations
{
    public partial class AddDeliveredUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeliveredUpdate",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredUpdate",
                table: "Notifications");
        }
    }
}
