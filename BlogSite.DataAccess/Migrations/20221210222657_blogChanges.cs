using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogSite.Migrations
{
    public partial class blogChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_ApplicationUserId",
                table: "BlogPosts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_AspNetUsers_ApplicationUserId",
                table: "BlogPosts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_AspNetUsers_ApplicationUserId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_ApplicationUserId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "BlogPosts");
        }
    }
}
