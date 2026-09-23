using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amazings_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderCustomerId",
                table: "Transactions",
                newName: "SenderAccountId");

            migrationBuilder.RenameColumn(
                name: "ReceiverCustomerId",
                table: "Transactions",
                newName: "ReceiverAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderAccountId",
                table: "Transactions",
                newName: "SenderCustomerId");

            migrationBuilder.RenameColumn(
                name: "ReceiverAccountId",
                table: "Transactions",
                newName: "ReceiverCustomerId");
        }
    }
}
