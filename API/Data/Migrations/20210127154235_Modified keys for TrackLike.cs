using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ModifiedkeysforTrackLike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackLikes_Tracks_TrackId",
                table: "TrackLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackLikes_Users_UserId",
                table: "TrackLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackLikes",
                table: "TrackLikes");

            migrationBuilder.DropIndex(
                name: "IX_TrackLikes_UserId",
                table: "TrackLikes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TrackId",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackLikes",
                table: "TrackLikes",
                columns: new[] { "UserId", "TrackId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TrackLikes_Tracks_TrackId",
                table: "TrackLikes",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackLikes_Users_UserId",
                table: "TrackLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackLikes_Tracks_TrackId",
                table: "TrackLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackLikes_Users_UserId",
                table: "TrackLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackLikes",
                table: "TrackLikes");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "TrackId",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TrackLikes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackLikes",
                table: "TrackLikes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TrackLikes_UserId",
                table: "TrackLikes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackLikes_Tracks_TrackId",
                table: "TrackLikes",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackLikes_Users_UserId",
                table: "TrackLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
