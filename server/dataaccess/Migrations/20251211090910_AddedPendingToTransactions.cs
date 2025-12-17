using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedPendingToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pending",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending",
                table: "Transactions");
        }
    }
}
