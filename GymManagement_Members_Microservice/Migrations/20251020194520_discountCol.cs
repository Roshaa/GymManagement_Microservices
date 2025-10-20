using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement_Members_Microservice.Migrations
{
    /// <inheritdoc />
    public partial class discountCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberDiscounts",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    DiscountCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RemainingMonths = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberDiscounts", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_MemberDiscounts_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberDiscounts");
        }
    }
}
