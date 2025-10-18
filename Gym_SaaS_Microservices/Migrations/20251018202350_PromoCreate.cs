using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gym_SaaS_Microservices.Migrations
{
    /// <inheritdoc />
    public partial class PromoCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Promo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    MonthDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Promo",
                columns: new[] { "Id", "Code", "Discount", "MonthDuration" },
                values: new object[,]
                {
                    { 1, "22ABT", 0.50m, 3 },
                    { 2, "23H5F", 0.20m, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Promo");
        }
    }
}
