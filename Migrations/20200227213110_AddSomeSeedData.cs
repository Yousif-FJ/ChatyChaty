using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Migrations
{
    public partial class AddSomeSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "MessagesSet",
              columns: new[] { "Sender", "Body" },
              values: new object[] { "SomeOne's Name", "Some Text lol ?!" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("MessagesSet", "ID", 1);
            migrationBuilder.DeleteData("MessagesSet", "Sender", "SomeOne");
            migrationBuilder.DeleteData("MessagesSet", "Body", "Some Text lol ?!");
        }

    }
}
