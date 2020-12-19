using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ColumnNameChangeAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserTrack_Tracks_TrackId",
                table: "AppUserTrack");

            migrationBuilder.RenameColumn(
                name: "TrackId",
                table: "AppUserTrack",
                newName: "TracksId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserTrack_TrackId",
                table: "AppUserTrack",
                newName: "IX_AppUserTrack_TracksId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserTrack_Tracks_TracksId",
                table: "AppUserTrack",
                column: "TracksId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserTrack_Tracks_TracksId",
                table: "AppUserTrack");

            migrationBuilder.RenameColumn(
                name: "TracksId",
                table: "AppUserTrack",
                newName: "TrackId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserTrack_TracksId",
                table: "AppUserTrack",
                newName: "IX_AppUserTrack_TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserTrack_Tracks_TrackId",
                table: "AppUserTrack",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
