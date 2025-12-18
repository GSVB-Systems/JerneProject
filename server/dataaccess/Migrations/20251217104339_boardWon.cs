using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class boardWon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Win",
                table: "Boards",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Win",
                table: "Boards");
        }
    }
}
