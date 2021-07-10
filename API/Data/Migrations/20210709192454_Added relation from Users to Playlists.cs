using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddedrelationfromUserstoPlaylists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Playlists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_AppUserId",
                table: "Playlists",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_AppUserId",
                table: "Playlists",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_AppUserId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_AppUserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Playlists");
        }
    }
}
