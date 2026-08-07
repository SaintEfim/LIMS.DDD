using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Removed_index_for_studyTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_Name_SampleId",
                table: "Studies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Studies_Name_SampleId",
                table: "Studies",
                columns: new[] { "Name", "SampleId" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
