using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.Service.Methodologies.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Revision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalculationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FormulaExpression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculationRules_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InputParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AliasName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SpecMinValue = table.Column<double>(type: "double precision", nullable: true),
                    SpecMaxValue = table.Column<double>(type: "double precision", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InputParameters_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultInstance = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    UnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpecMinValue = table.Column<double>(type: "double precision", nullable: true),
                    SpecMaxValue = table.Column<double>(type: "double precision", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultDefinitions_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultDefinitions_UnitSnapshots_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRules_StudyTemplateId",
                table: "CalculationRules",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InputParameters_StudyTemplateId_AliasName",
                table: "InputParameters",
                columns: new[] { "StudyTemplateId", "AliasName" },
                filter: "\"IsDeleted\" = false");

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

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplates_Name_Revision",
                table: "StudyTemplates",
                columns: new[] { "Name", "Revision" },
                unique: true,
                filter: "\"IsDeleted\" = false");

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
            migrationBuilder.DropTable(
                name: "CalculationRules");

            migrationBuilder.DropTable(
                name: "InputParameters");

            migrationBuilder.DropTable(
                name: "ResultDefinitions");

            migrationBuilder.DropTable(
                name: "StudyTemplates");

            migrationBuilder.DropTable(
                name: "UnitSnapshots");
        }
    }
}
