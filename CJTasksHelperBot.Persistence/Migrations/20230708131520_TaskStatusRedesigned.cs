using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TaskStatusRedesigned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Tasks",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Homework",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "TaskStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Homework_StatusId",
                table: "Homework",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Homework_TaskStatus_StatusId",
                table: "Homework",
                column: "StatusId",
                principalTable: "TaskStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskStatus_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "TaskStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homework_TaskStatus_StatusId",
                table: "Homework");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskStatus_StatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "TaskStatus");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Homework_StatusId",
                table: "Homework");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Tasks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Homework",
                newName: "Status");
        }
    }
}
