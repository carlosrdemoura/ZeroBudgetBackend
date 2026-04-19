using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZeroBudget.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSourceFromTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "source",
                table: "transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "source",
                table: "transactions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
