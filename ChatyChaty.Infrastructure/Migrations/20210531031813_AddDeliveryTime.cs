using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Infrastructure.Migrations
{
    public partial class AddDeliveryTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeSent",
                table: "Messages",
                newName: "SentTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryTime",
                table: "Messages",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTime",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "Messages",
                newName: "TimeSent");
        }
    }
}
