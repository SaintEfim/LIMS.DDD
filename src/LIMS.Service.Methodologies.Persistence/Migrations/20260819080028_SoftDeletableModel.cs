using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.Service.Methodologies.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeletableModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UnitSnapshots_Name",
                table: "UnitSnapshots",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitSnapshots_Name",
                table: "UnitSnapshots");
        }
    }
}
