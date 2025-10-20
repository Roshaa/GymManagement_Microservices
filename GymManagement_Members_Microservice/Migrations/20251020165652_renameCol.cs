using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement_Members_Microservice.Migrations
{
    /// <inheritdoc />
    public partial class renameCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankAccount",
                table: "Member",
                newName: "IBAN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IBAN",
                table: "Member",
                newName: "BankAccount");
        }
    }
}
