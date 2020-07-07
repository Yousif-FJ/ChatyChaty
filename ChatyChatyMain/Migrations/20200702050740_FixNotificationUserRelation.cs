using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ChatyChaty.Migrations
{
    public partial class FixNotificationUserRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.CreateTable(
               name: "Notifications",
               columns: table => new
               {
                   Id = table.Column<long>(nullable: false)
                   .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                   ChatUpdate = table.Column<bool>(nullable: false),
                   MessageUpdate = table.Column<bool>(nullable: false),
                   DeliveredUpdate = table.Column<bool>(nullable: false),
                   UserId = table.Column<long>(nullable: false)
               },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
                table.ForeignKey(
                    name: "FK_Notifications_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId",
                unique: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");


            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ChatUpdate = table.Column<bool>(nullable: false),
                    MessageUpdate = table.Column<bool>(nullable: false),
                    DeliveredUpdate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
