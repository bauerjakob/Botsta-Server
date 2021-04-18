using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Botsta.DataStorage.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatPracticant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Registerd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SecretHash = table.Column<string>(type: "text", nullable: false),
                    SecretSalt = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPracticant", x => x.Id);
                });

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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_ChatPracticant_Id",
                        column: x => x.Id,
                        principalTable: "ChatPracticant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatPracticantChatroom",
                columns: table => new
                {
                    ChatPracticantsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatroomsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPracticantChatroom", x => new { x.ChatPracticantsId, x.ChatroomsId });
                    table.ForeignKey(
                        name: "FK_ChatPracticantChatroom_ChatPracticant_ChatPracticantsId",
                        column: x => x.ChatPracticantsId,
                        principalTable: "ChatPracticant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatPracticantChatroom_Chatroom_ChatroomsId",
                        column: x => x.ChatroomsId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ChatroomId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_ChatPracticant_SenderId",
                        column: x => x.SenderId,
                        principalTable: "ChatPracticant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_Chatroom_ChatroomId",
                        column: x => x.ChatroomId,
                        principalTable: "Chatroom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bots_ChatPracticant_Id",
                        column: x => x.Id,
                        principalTable: "ChatPracticant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bots_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bots_OwnerId",
                table: "Bots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPracticant_Name",
                table: "ChatPracticant",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatPracticantChatroom_ChatroomsId",
                table: "ChatPracticantChatroom",
                column: "ChatroomsId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ChatroomId",
                table: "Message",
                column: "ChatroomId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.DropTable(
                name: "ChatPracticantChatroom");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Chatroom");

            migrationBuilder.DropTable(
                name: "ChatPracticant");
        }
    }
}
