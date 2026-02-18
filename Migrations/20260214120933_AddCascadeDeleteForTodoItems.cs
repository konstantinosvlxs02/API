using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace part2_exersice.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForTodoItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Posts_TodoId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Todos");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "TodoListItems");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Todos",
                newName: "IX_Todos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_TodoId",
                table: "TodoListItems",
                newName: "IX_TodoListItems_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Todos",
                table: "Todos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoListItems",
                table: "TodoListItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoListItems_Todos_TodoId",
                table: "TodoListItems",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Users_UserId",
                table: "Todos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoListItems_Todos_TodoId",
                table: "TodoListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Users_UserId",
                table: "Todos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Todos",
                table: "Todos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoListItems",
                table: "TodoListItems");

            migrationBuilder.RenameTable(
                name: "Todos",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "TodoListItems",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_Todos_UserId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoListItems_TodoId",
                table: "Categories",
                newName: "IX_Categories_TodoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Posts_TodoId",
                table: "Categories",
                column: "TodoId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
