using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroBudget.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovaIsDeletedFromTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_is_deleted",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_transactions_is_deleted",
                table: "transactions",
                column: "is_deleted");
        }
    }
}
