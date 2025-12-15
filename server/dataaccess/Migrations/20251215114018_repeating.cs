using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class repeating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRepeating",
                table: "Boards");

            migrationBuilder.AddColumn<int>(
                name: "Week",
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
                table: "Boards");

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeating",
                table: "Boards",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
