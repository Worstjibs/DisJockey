using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddedSmallandLargeThumbnails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "Tracks",
                newName: "SmallThumbnail");

            migrationBuilder.AddColumn<string>(
                name: "LargeThumbnail",
                table: "Tracks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediumThumbnail",
                table: "Tracks",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LargeThumbnail",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "MediumThumbnail",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "SmallThumbnail",
                table: "Tracks",
                newName: "Thumbnail");
        }
    }
}
