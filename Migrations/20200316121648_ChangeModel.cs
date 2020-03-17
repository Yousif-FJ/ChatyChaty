using Microsoft.EntityFrameworkCore.Migrations;

namespace ChatyChaty.Migrations
{
    public partial class ChangeModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_ReceiverID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_SenderID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationID",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ReceiverID",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_SenderID",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ReceiverID",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderID",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ConversationID",
                table: "Messages",
                newName: "ConversationId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Messages",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationID",
                table: "Messages",
                newName: "IX_Messages_ConversationId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Conversations",
                newName: "Id");

            migrationBuilder.AddColumn<long>(
                name: "SenderId",
                table: "Messages",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FirstUserId",
                table: "Conversations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SecondUserId",
                table: "Conversations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_FirstUserId",
                table: "Conversations",
                column: "FirstUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SecondUserId",
                table: "Conversations",
                column: "SecondUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_FirstUserId",
                table: "Conversations",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_SecondUserId",
                table: "Conversations",
                column: "SecondUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_FirstUserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_SecondUserId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_FirstUserId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_SecondUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FirstUserId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "SecondUserId",
                table: "Conversations");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "Messages",
                newName: "ConversationID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Messages",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                newName: "IX_Messages_ConversationID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Conversations",
                newName: "ID");

            migrationBuilder.AddColumn<long>(
                name: "ReceiverID",
                table: "Conversations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "SenderID",
                table: "Conversations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ReceiverID",
                table: "Conversations",
                column: "ReceiverID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SenderID",
                table: "Conversations",
                column: "SenderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_ReceiverID",
                table: "Conversations",
                column: "ReceiverID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_SenderID",
                table: "Conversations",
                column: "SenderID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationID",
                table: "Messages",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
