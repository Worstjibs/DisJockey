using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddedAppUserToTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Users_AppUserId",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Tracks",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_AppUserId",
                table: "Tracks",
                newName: "IX_Tracks_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Users_UserId",
                table: "Tracks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Users_UserId",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Tracks",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tracks_UserId",
                table: "Tracks",
                newName: "IX_Tracks_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Users_AppUserId",
                table: "Tracks",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
