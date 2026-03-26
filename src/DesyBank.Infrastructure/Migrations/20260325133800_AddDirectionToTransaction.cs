using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesyBank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDirectionToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferType",
                table: "transactions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferType",
                table: "transactions");
        }
    }
}
