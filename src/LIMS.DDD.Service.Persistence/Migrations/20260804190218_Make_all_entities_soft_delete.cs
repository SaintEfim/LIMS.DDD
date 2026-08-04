using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Make_all_entities_soft_delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_SampleId",
                table: "Studies");

            migrationBuilder.DropIndex(
                name: "IX_Samples_Code",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_Unit_ResultInstance",
                table: "ResultDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Code",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_InputParameters_StudyTemplateId_AliasName",
                table: "InputParameters");

            migrationBuilder.DropIndex(
                name: "IX_CalculationRules_StudyTemplateId_Name",
                table: "CalculationRules");

            migrationBuilder.RenameColumn(
                name: "Specification_MinValue",
                table: "ResultDefinitions",
                newName: "SpecMinValue");

            migrationBuilder.RenameColumn(
                name: "Specification_MaxValue",
                table: "ResultDefinitions",
                newName: "SpecMaxValue");

            migrationBuilder.RenameColumn(
                name: "Specification_MinValue",
                table: "InputParameters",
                newName: "SpecMinValue");

            migrationBuilder.RenameColumn(
                name: "Specification_MaxValue",
                table: "InputParameters",
                newName: "SpecMaxValue");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "TestResults",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TestResults",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Revision",
                table: "StudyTemplates",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "StudyTemplates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudyTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Studies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Studies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Samples",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Samples",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ResultInstance",
                table: "ResultDefinitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ResultDefinitions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ResultDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "MeasuredValues",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MeasuredValues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "InputParameters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InputParameters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "CalculationRules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CalculationRules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Studies_SampleId_TemplateId",
                table: "Studies",
                columns: new[] { "SampleId", "TemplateId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_Code",
                table: "Samples",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_Unit",
                table: "ResultDefinitions",
                columns: new[] { "StudyTemplateId", "ResultInstance", "Unit" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_InputParameters_StudyTemplateId_AliasName",
                table: "InputParameters",
                columns: new[] { "StudyTemplateId", "AliasName" });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRules_StudyTemplateId",
                table: "CalculationRules",
                column: "StudyTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studies_SampleId_TemplateId",
                table: "Studies");

            migrationBuilder.DropIndex(
                name: "IX_Samples_Code",
                table: "Samples");

            migrationBuilder.DropIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_ResultInstance_Unit",
                table: "ResultDefinitions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Code",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_InputParameters_StudyTemplateId_AliasName",
                table: "InputParameters");

            migrationBuilder.DropIndex(
                name: "IX_CalculationRules_StudyTemplateId",
                table: "CalculationRules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StudyTemplates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudyTemplates");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ResultDefinitions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ResultDefinitions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MeasuredValues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MeasuredValues");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InputParameters");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InputParameters");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CalculationRules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CalculationRules");

            migrationBuilder.RenameColumn(
                name: "SpecMinValue",
                table: "ResultDefinitions",
                newName: "Specification_MinValue");

            migrationBuilder.RenameColumn(
                name: "SpecMaxValue",
                table: "ResultDefinitions",
                newName: "Specification_MaxValue");

            migrationBuilder.RenameColumn(
                name: "SpecMinValue",
                table: "InputParameters",
                newName: "Specification_MinValue");

            migrationBuilder.RenameColumn(
                name: "SpecMaxValue",
                table: "InputParameters",
                newName: "Specification_MaxValue");

            migrationBuilder.AlterColumn<string>(
                name: "Revision",
                table: "StudyTemplates",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ResultInstance",
                table: "ResultDefinitions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Studies_SampleId",
                table: "Studies",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_Code",
                table: "Samples",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitions_StudyTemplateId_Unit_ResultInstance",
                table: "ResultDefinitions",
                columns: new[] { "StudyTemplateId", "Unit", "ResultInstance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InputParameters_StudyTemplateId_AliasName",
                table: "InputParameters",
                columns: new[] { "StudyTemplateId", "AliasName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRules_StudyTemplateId_Name",
                table: "CalculationRules",
                columns: new[] { "StudyTemplateId", "Name" },
                unique: true);
        }
    }
}
