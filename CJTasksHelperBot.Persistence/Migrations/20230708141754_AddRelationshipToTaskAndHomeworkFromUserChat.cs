using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationshipToTaskAndHomeworkFromUserChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserChatChatId",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserChatUserId",
                table: "Tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserChatChatId",
                table: "Homework",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserChatUserId",
                table: "Homework",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserChatUserId_UserChatChatId",
                table: "Tasks",
                columns: new[] { "UserChatUserId", "UserChatChatId" });

            migrationBuilder.CreateIndex(
                name: "IX_Homework_UserChatUserId_UserChatChatId",
                table: "Homework",
                columns: new[] { "UserChatUserId", "UserChatChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Homework_UserChats_UserChatUserId_UserChatChatId",
                table: "Homework",
                columns: new[] { "UserChatUserId", "UserChatChatId" },
                principalTable: "UserChats",
                principalColumns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserChats_UserChatUserId_UserChatChatId",
                table: "Tasks",
                columns: new[] { "UserChatUserId", "UserChatChatId" },
                principalTable: "UserChats",
                principalColumns: new[] { "UserId", "ChatId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homework_UserChats_UserChatUserId_UserChatChatId",
                table: "Homework");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserChats_UserChatUserId_UserChatChatId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserChatUserId_UserChatChatId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Homework_UserChatUserId_UserChatChatId",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "UserChatChatId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserChatUserId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserChatChatId",
                table: "Homework");

            migrationBuilder.DropColumn(
                name: "UserChatUserId",
                table: "Homework");
        }
    }
}
