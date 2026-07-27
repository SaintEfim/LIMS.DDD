using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Added_calculation_input : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationInput",
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
                    table.PrimaryKey("PK_CalculationInput", x => new { x.CalculationRuleId, x.Id });
                    table.ForeignKey(
                        name: "FK_CalculationInput_CalculationRules_CalculationRuleId",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationInput");

            migrationBuilder.DropIndex(
                name: "IX_CalculationRules_Name",
                table: "CalculationRules");
        }
    }
}
