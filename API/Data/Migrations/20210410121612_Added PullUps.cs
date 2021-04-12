using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddedPullUps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullUp_Tracks_TrackId",
                table: "PullUp");

            migrationBuilder.DropForeignKey(
                name: "FK_PullUp_Users_UserId",
                table: "PullUp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PullUp",
                table: "PullUp");

            migrationBuilder.RenameTable(
                name: "PullUp",
                newName: "PullUps");

            migrationBuilder.RenameIndex(
                name: "IX_PullUp_UserId",
                table: "PullUps",
                newName: "IX_PullUps_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PullUp_TrackId",
                table: "PullUps",
                newName: "IX_PullUps_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PullUps",
                table: "PullUps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PullUps_Tracks_TrackId",
                table: "PullUps",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PullUps_Users_UserId",
                table: "PullUps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PullUps_Tracks_TrackId",
                table: "PullUps");

            migrationBuilder.DropForeignKey(
                name: "FK_PullUps_Users_UserId",
                table: "PullUps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PullUps",
                table: "PullUps");

            migrationBuilder.RenameTable(
                name: "PullUps",
                newName: "PullUp");

            migrationBuilder.RenameIndex(
                name: "IX_PullUps_UserId",
                table: "PullUp",
                newName: "IX_PullUp_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PullUps_TrackId",
                table: "PullUp",
                newName: "IX_PullUp_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PullUp",
                table: "PullUp",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PullUp_Tracks_TrackId",
                table: "PullUp",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PullUp_Users_UserId",
                table: "PullUp",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
