using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroBudget.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "transactions",
                newName: "source");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "category_groups",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_accounts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_transactions_account_id",
                table: "transactions",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_user_id_account_id",
                table: "transactions",
                columns: new[] { "user_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_user_id_is_deleted",
                table: "accounts",
                columns: new[] { "user_id", "is_deleted" });

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_transactions_account_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "IX_transactions_user_id_account_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "category_groups");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "source",
                table: "transactions",
                newName: "type");
        }
    }
}
