using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewTimeStampVariablesToHandleWeekAndYearConversions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "WinningBoards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeekYear",
                table: "WinningBoards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeeksPurchased",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week",
                table: "WinningBoards");

            migrationBuilder.DropColumn(
                name: "WeekYear",
                table: "WinningBoards");

            migrationBuilder.DropColumn(
                name: "WeeksPurchased",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Boards");
        }
    }
}
