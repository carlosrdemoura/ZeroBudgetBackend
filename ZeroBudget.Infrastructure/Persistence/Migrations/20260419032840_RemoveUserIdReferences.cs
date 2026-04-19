using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroBudget.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_category_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_account_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_category_id_date",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_date",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_is_deleted",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_budget_entries_category_id",
                table: "budget_entries");

            migrationBuilder.DropIndex(
                name: "IX_budget_entries_user_id_category_id_month",
                table: "budget_entries");

            migrationBuilder.DropIndex(
                name: "IX_budget_entries_user_id_month",
                table: "budget_entries");

            migrationBuilder.DropIndex(
                name: "IX_accounts_user_id",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "category_groups");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "budget_entries");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "accounts");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id_date",
                table: "transactions",
                columns: new[] { "category_id", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_date",
                table: "transactions",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_is_deleted",
                table: "transactions",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_budget_entries_category_id_month",
                table: "budget_entries",
                columns: new[] { "category_id", "month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_transactions_category_id_date",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_date",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_is_deleted",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_budget_entries_category_id_month",
                table: "budget_entries");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "category_groups",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "budget_entries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                column: "user_id",
                value: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.UpdateData(
                table: "category_groups",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                column: "user_id",
                value: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.CreateIndex(
                name: "IX_transactions_category_id",
                table: "transactions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_account_id",
                table: "transactions",
                columns: new[] { "user_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_category_id_date",
                table: "transactions",
                columns: new[] { "user_id", "category_id", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_date",
                table: "transactions",
                columns: new[] { "user_id", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_is_deleted",
                table: "transactions",
                columns: new[] { "user_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_budget_entries_category_id",
                table: "budget_entries",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_entries_user_id_category_id_month",
                table: "budget_entries",
                columns: new[] { "user_id", "category_id", "month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budget_entries_user_id_month",
                table: "budget_entries",
                columns: new[] { "user_id", "month" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_user_id",
                table: "accounts",
                column: "user_id");
        }
    }
}
