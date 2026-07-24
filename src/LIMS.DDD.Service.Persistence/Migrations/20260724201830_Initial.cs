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
                name: "StudyTemplateParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AliasName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ValueRange_MinValue = table.Column<double>(type: "double precision", nullable: true),
                    ValueRange_MaxValue = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplateParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTemplateParameters_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyTemplateResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultInstance = table.Column<string>(type: "text", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ValueRange_MinValue = table.Column<double>(type: "double precision", nullable: true),
                    ValueRange_MaxValue = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplateResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyTemplateResults_StudyTemplates_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateParameters_StudyTemplateId",
                table: "StudyTemplateParameters",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateResults_StudyTemplateId",
                table: "StudyTemplateResults",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateResults_Unit_ResultInstance",
                table: "StudyTemplateResults",
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
                name: "StudyTemplateParameters");

            migrationBuilder.DropTable(
                name: "StudyTemplateResults");

            migrationBuilder.DropTable(
                name: "StudyTemplates");
        }
    }
}
