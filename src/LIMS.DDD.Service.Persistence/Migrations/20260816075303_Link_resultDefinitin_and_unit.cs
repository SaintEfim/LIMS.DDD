using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Link_resultDefinitin_and_unit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_Unit",
                table: "ResultDefinitions");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ResultDefinitions");

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "ResultDefinitions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_UnitId",
                table: "ResultDefinitions",
                columns: new[] { "StudyTemplateId", "ResultInstance", "UnitId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_UnitId",
                table: "ResultDefinitions",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResultDefinitions_UnitSnapshots_UnitId",
                table: "ResultDefinitions",
                column: "UnitId",
                principalTable: "UnitSnapshots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultDefinitions_UnitSnapshots_UnitId",
                table: "ResultDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_UnitId",
                table: "ResultDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_ResultDefinitions_UnitId",
                table: "ResultDefinitions");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "ResultDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ResultDefinitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_Unit",
                table: "ResultDefinitions",
                columns: new[] { "StudyTemplateId", "ResultInstance", "Unit" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
