using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Rename_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyTemplateDeterminations");

            migrationBuilder.DropTable(
                name: "StudyTemplateObservations");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputParameters");

            migrationBuilder.DropTable(
                name: "ResultDefinitions");

            migrationBuilder.CreateTable(
                name: "StudyTemplateDeterminations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultInstance = table.Column<string>(type: "text", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Specification_MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    Specification_MinValue = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplateDeterminations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTemplateDeterminations_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyTemplateObservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AliasName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Specification_MaxValue = table.Column<double>(type: "double precision", nullable: true),
                    Specification_MinValue = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplateObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTemplateObservations_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateDeterminations_StudyTemplateId",
                table: "StudyTemplateDeterminations",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateDeterminations_Unit_ResultInstance",
                table: "StudyTemplateDeterminations",
                columns: new[] { "Unit", "ResultInstance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateObservations_StudyTemplateId",
                table: "StudyTemplateObservations",
                column: "StudyTemplateId");
        }
    }
}
