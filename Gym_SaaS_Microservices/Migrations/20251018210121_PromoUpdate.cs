using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement_Promo_Microservice.Migrations
{
    /// <inheritdoc />
    public partial class PromoUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Promo",
                type: "varchar(5)",
                unicode: false,
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.CreateIndex(
                name: "IX_Promo_Code",
                table: "Promo",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Promo_Code",
                table: "Promo");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Promo",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldUnicode: false,
                oldMaxLength: 5);
        }
    }
}
