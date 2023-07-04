using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRelationBetweenUsersAndChats2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChats");

            migrationBuilder.CreateTable(
                name: "UserChats",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChatId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChats", x => new { x.UserId, x.ChatId });
                    table.ForeignKey(
                        name: "FK_UserChats_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_UserChats_Chats_ChatId1",
                        column: x => x.ChatId1,
                        principalTable: "Chats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserChats_Chats_UserId",
                        column: x => x.UserId,
                        principalTable: "Chats",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_UserChats_Users_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Users",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_UserChats_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_UserChats_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_ChatId",
                table: "UserChats",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_ChatId1",
                table: "UserChats",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserChats_UserId1",
                table: "UserChats",
                column: "UserId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChats");

            migrationBuilder.CreateTable(
                name: "ChatUser",
                columns: table => new
                {
                    ChatsTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    UsersTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    ChatTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUser", x => new { x.ChatsTelegramId, x.UsersTelegramId });
                    table.ForeignKey(
                        name: "FK_ChatUser_Chats_ChatTelegramId",
                        column: x => x.ChatTelegramId,
                        principalTable: "Chats",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_ChatUser_Chats_ChatsTelegramId",
                        column: x => x.ChatsTelegramId,
                        principalTable: "Chats",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_ChatUser_Users_UserTelegramId",
                        column: x => x.UserTelegramId,
                        principalTable: "Users",
                        principalColumn: "TelegramId");
                    table.ForeignKey(
                        name: "FK_ChatUser_Users_UsersTelegramId",
                        column: x => x.UsersTelegramId,
                        principalTable: "Users",
                        principalColumn: "TelegramId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_ChatTelegramId",
                table: "ChatUser",
                column: "ChatTelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UsersTelegramId",
                table: "ChatUser",
                column: "UsersTelegramId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UserTelegramId",
                table: "ChatUser",
                column: "UserTelegramId");
        }
    }
}
