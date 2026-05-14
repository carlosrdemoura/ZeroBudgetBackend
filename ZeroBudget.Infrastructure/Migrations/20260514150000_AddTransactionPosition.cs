using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroBudget.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "position",
                table: "transactions",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql(@"
                UPDATE transactions
                SET position = sub.rn * 1024.0
                FROM (
                    SELECT id, ROW_NUMBER() OVER (ORDER BY date DESC, created_at DESC) AS rn
                    FROM transactions
                ) sub
                WHERE transactions.id = sub.id;
            ");

            migrationBuilder.DropIndex(
                name: "IX_transactions_date",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_date_position",
                table: "transactions",
                columns: new[] { "date", "position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_date_position",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "position",
                table: "transactions");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_date",
                table: "transactions",
                column: "date");
        }
    }
}
