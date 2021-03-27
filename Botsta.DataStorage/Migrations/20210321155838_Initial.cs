using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Botsta.DataStorage.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chatroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chatroom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageJson = table.Column<string>(type: "text", nullable: false),
                    ChatroomId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderType = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Registerd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatroomMessage",
                columns: table => new
                {
                    ChatroomsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessagesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomMessage", x => new { x.ChatroomsId, x.MessagesId });
                    table.ForeignKey(
                        name: "FK_ChatroomMessage_Chatroom_ChatroomsId",
                        column: x => x.ChatroomsId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatroomMessage_Message_MessagesId",
                        column: x => x.MessagesId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BotName = table.Column<string>(type: "text", nullable: false),
                    WebhookUrl = table.Column<string>(type: "text", nullable: true),
                    ApiKeyHash = table.Column<string>(type: "text", nullable: false),
                    ApiKeySalt = table.Column<string>(type: "text", nullable: false),
                    Registerd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatroomId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bots_Chatroom_ChatroomId",
                        column: x => x.ChatroomId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bots_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatroomUser",
                columns: table => new
                {
                    ChatroomsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomUser", x => new { x.ChatroomsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ChatroomUser_Chatroom_ChatroomsId",
                        column: x => x.ChatroomsId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatroomUser_User_UsersId",
                        column: x => x.UsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bots_ChatroomId",
                table: "Bots",
                column: "ChatroomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bots_OwnerId",
                table: "Bots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomMessage_MessagesId",
                table: "ChatroomMessage",
                column: "MessagesId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatroomUser_UsersId",
                table: "ChatroomUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "ChatroomMessage");

            migrationBuilder.DropTable(
                name: "ChatroomUser");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Chatroom");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
