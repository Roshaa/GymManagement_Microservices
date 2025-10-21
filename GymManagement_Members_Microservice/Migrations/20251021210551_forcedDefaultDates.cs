using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement_Members_Microservice.Migrations
{
    /// <inheritdoc />
    public partial class forcedDefaultDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "RegisterDay",
                table: "Member",
                type: "date",
                nullable: false,
                defaultValueSql: "CONVERT(date, GETUTCDATE())",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ActiveUntilDay",
                table: "Member",
                type: "date",
                nullable: false,
                defaultValueSql: "CONVERT(date, DATEADD(month, 1, GETUTCDATE()))",
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "RegisterDay",
                table: "Member",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CONVERT(date, GETUTCDATE())");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ActiveUntilDay",
                table: "Member",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValueSql: "CONVERT(date, DATEADD(month, 1, GETUTCDATE()))");
        }
    }
}
