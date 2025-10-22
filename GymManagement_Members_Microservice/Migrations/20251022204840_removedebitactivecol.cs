using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement_Members_Microservice.Migrations
{
    /// <inheritdoc />
    public partial class removedebitactivecol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebitActive",
                table: "Member");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DebitActive",
                table: "Member",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
