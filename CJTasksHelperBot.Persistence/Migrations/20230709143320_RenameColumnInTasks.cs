using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnInTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAd",
                table: "Tasks",
                newName: "CompletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "Tasks",
                newName: "CompletedAd");
        }
    }
}
