using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations.SqliteMigrations
{
    /// <inheritdoc />
    public partial class AddLanguageFieldToChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageCode",
                table: "Chats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Chats");
        }
    }
}
