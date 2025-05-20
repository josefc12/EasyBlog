using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class articleauthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Articles",
                type: "integer",
                nullable: true);
            migrationBuilder.Sql("UPDATE \"Articles\" SET \"AuthorId\" = 1 WHERE \"AuthorId\" IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Articles");
        }
    }
}
