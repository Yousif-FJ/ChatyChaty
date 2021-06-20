using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Infrastructure.Migrations
{
    public partial class RenameDeliveryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryTime",
                table: "Messages",
                newName: "StatusUpdateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusUpdateTime",
                table: "Messages",
                newName: "DeliveryTime");
        }
    }
}
