using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddedTrackPlayHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Playlists_PlaylistId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Tracks_TrackId",
                table: "PlaylistTrack");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTrack_Users_CreatedById",
                table: "PlaylistTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistTrack",
                table: "PlaylistTrack");

            migrationBuilder.RenameTable(
                name: "PlaylistTrack",
                newName: "PlaylistTracks");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_PlaylistId",
                table: "PlaylistTracks",
                newName: "IX_PlaylistTracks_PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTrack_CreatedById",
                table: "PlaylistTracks",
                newName: "IX_PlaylistTracks_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistTracks",
                table: "PlaylistTracks",
                columns: new[] { "TrackId", "PlaylistId" });

            migrationBuilder.CreateTable(
                name: "TrackPlayHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackPlayId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackPlayHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackPlayHistory_TrackPlays_TrackPlayId",
                        column: x => x.TrackPlayId,
                        principalTable: "TrackPlays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackPlayHistory_TrackPlayId",
                table: "TrackPlayHistory",
                column: "TrackPlayId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Playlists_PlaylistId",
                table: "PlaylistTracks",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTracks_Users_CreatedById",
                table: "PlaylistTracks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Playlists_PlaylistId",
                table: "PlaylistTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Tracks_TrackId",
                table: "PlaylistTracks");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistTracks_Users_CreatedById",
                table: "PlaylistTracks");

            migrationBuilder.DropTable(
                name: "TrackPlayHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistTracks",
                table: "PlaylistTracks");

            migrationBuilder.RenameTable(
                name: "PlaylistTracks",
                newName: "PlaylistTrack");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTracks_PlaylistId",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistTracks_CreatedById",
                table: "PlaylistTrack",
                newName: "IX_PlaylistTrack_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistTrack",
                table: "PlaylistTrack",
                columns: new[] { "TrackId", "PlaylistId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Playlists_PlaylistId",
                table: "PlaylistTrack",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Tracks_TrackId",
                table: "PlaylistTrack",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistTrack_Users_CreatedById",
                table: "PlaylistTrack",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
