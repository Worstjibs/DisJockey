using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ChangedTimePulledtodouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TimePulled",
                table: "PullUps",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TimePulled",
                table: "PullUps",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
