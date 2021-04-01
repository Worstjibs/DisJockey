using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ChangedDBTablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTracks_Tracks_TrackId",
                table: "UserTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTracks_Users_AppUserId",
                table: "UserTracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTracks",
                table: "UserTracks");

            migrationBuilder.DropColumn(
                name: "LastPlayed",
                table: "UserTracks");

            migrationBuilder.RenameTable(
                name: "UserTracks",
                newName: "TrackPlays");

            migrationBuilder.RenameColumn(
                name: "TimesPlayed",
                table: "TrackPlays",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserTracks_TrackId",
                table: "TrackPlays",
                newName: "IX_TrackPlays_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackPlays",
                table: "TrackPlays",
                columns: new[] { "AppUserId", "TrackId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TrackPlays_Tracks_TrackId",
                table: "TrackPlays",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackPlays_Users_AppUserId",
                table: "TrackPlays",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackPlays_Tracks_TrackId",
                table: "TrackPlays");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackPlays_Users_AppUserId",
                table: "TrackPlays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackPlays",
                table: "TrackPlays");

            migrationBuilder.RenameTable(
                name: "TrackPlays",
                newName: "UserTracks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserTracks",
                newName: "TimesPlayed");

            migrationBuilder.RenameIndex(
                name: "IX_TrackPlays_TrackId",
                table: "UserTracks",
                newName: "IX_UserTracks_TrackId");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPlayed",
                table: "UserTracks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTracks",
                table: "UserTracks",
                columns: new[] { "AppUserId", "TrackId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserTracks_Tracks_TrackId",
                table: "UserTracks",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTracks_Users_AppUserId",
                table: "UserTracks",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
