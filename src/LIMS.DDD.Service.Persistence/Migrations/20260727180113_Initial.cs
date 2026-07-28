using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Revision = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalculationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FormulaExpression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
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
                    Specification_MinValue = table.Column<double>(type: "double precision", nullable: true),
                    Specification_MaxValue = table.Column<double>(type: "double precision", nullable: true)
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
                    ResultInstance = table.Column<string>(type: "text", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Specification_MinValue = table.Column<double>(type: "double precision", nullable: true),
                    Specification_MaxValue = table.Column<double>(type: "double precision", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "CalculationInputs",
                columns: table => new
                {
                    CalculationRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariableAlias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParameterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationInputs", x => new { x.CalculationRuleId, x.Id });
                    table.ForeignKey(
                        name: "FK_CalculationInputs_CalculationRules_CalculationRuleId",
                        column: x => x.CalculationRuleId,
                        principalTable: "CalculationRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRules_Name",
                table: "CalculationRules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRules_StudyTemplateId",
                table: "CalculationRules",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InputParameters_StudyTemplateId",
                table: "InputParameters",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_StudyTemplateId",
                table: "ResultDefinitions",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_Unit_ResultInstance",
                table: "ResultDefinitions",
                columns: new[] { "Unit", "ResultInstance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplates_Name_Revision",
                table: "StudyTemplates",
                columns: new[] { "Name", "Revision" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationInputs");

            migrationBuilder.DropTable(
                name: "InputParameters");

            migrationBuilder.DropTable(
                name: "ResultDefinitions");

            migrationBuilder.DropTable(
                name: "CalculationRules");

            migrationBuilder.DropTable(
                name: "StudyTemplates");
        }
    }
}
