using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotAPI.Migrations
{
    /// <inheritdoc />
    public partial class task_model_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskID",
                table: "Tasks",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tasks",
                newName: "TaskID");
        }
    }
}
