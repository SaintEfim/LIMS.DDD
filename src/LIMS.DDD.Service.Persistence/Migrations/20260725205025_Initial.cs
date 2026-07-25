using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "StudyTemplateDetermination",
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
                    table.PrimaryKey("PK_StudyTemplateDetermination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTemplateDetermination_StudyTemplates_StudyTemplateId",
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
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AliasName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Specification_MinValue = table.Column<double>(type: "double precision", nullable: true),
                    Specification_MaxValue = table.Column<double>(type: "double precision", nullable: true)
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
                name: "IX_StudyTemplateDetermination_StudyTemplateId",
                table: "StudyTemplateDetermination",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateDetermination_Unit_ResultInstance",
                table: "StudyTemplateDetermination",
                columns: new[] { "Unit", "ResultInstance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateObservations_StudyTemplateId",
                table: "StudyTemplateObservations",
                column: "StudyTemplateId");

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
                name: "StudyTemplateDetermination");

            migrationBuilder.DropTable(
                name: "StudyTemplateObservations");

            migrationBuilder.DropTable(
                name: "StudyTemplates");
        }
    }
}
