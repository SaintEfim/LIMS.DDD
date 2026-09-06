using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.Service.LaboratoryOperations.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Make_not_deletable_unit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UnitSnapshots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UnitSnapshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "UnitSnapshots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UnitSnapshots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
