using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Migrations
{
    public partial class RemovePhotoID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoID",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoID",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}
