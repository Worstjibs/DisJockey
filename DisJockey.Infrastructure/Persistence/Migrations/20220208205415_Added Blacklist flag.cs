using Microsoft.EntityFrameworkCore.Migrations;

namespace DisJockey.Infrastructure.Migrations;

public partial class AddedBlacklistflag : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "Blacklisted",
            table: "Tracks",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Blacklisted",
            table: "Tracks");
    }
}
