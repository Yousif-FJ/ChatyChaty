using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Infrastructure.Migrations
{
    public partial class AddPhotoUrlToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoURL",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoURL",
                table: "AspNetUsers");
        }
    }
}
