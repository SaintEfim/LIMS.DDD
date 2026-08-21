using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.Service.LaboratoryOperations.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Contractor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyTemplateSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Revision = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyTemplateSnapshots", x => x.Id);
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
                name: "CalculationRuleCalculationRuleSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FormulaExpression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ResultDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationRuleCalculationRuleSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalculationRuleCalculationRuleSnapshots_StudyTemplateSnapsh~",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplateSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InputParameterSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AliasName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MinValue = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    MaxValue = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputParameterSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InputParameterSnapshots_StudyTemplateSnapshots_StudyTemplat~",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplateSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultDefinitionSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultInstance = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinValue = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    MaxValue = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultDefinitionSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultDefinitionSnapshots_StudyTemplateSnapshots_StudyTempl~",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplateSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResultDefinitionSnapshots_UnitSnapshots_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GatherDateBegin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    GatherDateEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VolumeValue = table.Column<double>(type: "numeric(18,4)", nullable: true),
                    VolumeUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    SampleStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Samples_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Samples_UnitSnapshots_VolumeUnitId",
                        column: x => x.VolumeUnitId,
                        principalTable: "UnitSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StudyTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Studies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Studies_Samples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "Samples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Studies_StudyTemplateSnapshots_StudyTemplateId",
                        column: x => x.StudyTemplateId,
                        principalTable: "StudyTemplateSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasuredValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyId = table.Column<Guid>(type: "uuid", nullable: false),
                    InputParameterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasuredValues_InputParameterSnapshots_InputParameterId",
                        column: x => x.InputParameterId,
                        principalTable: "InputParameterSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MeasuredValues_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResultDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "numeric(18,6)", nullable: true),
                    IsOutOfSpec = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_ResultDefinitionSnapshots_ResultDefinitionId",
                        column: x => x.ResultDefinitionId,
                        principalTable: "ResultDefinitionSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationRuleCalculationRuleSnapshots_StudyTemplateId",
                table: "CalculationRuleCalculationRuleSnapshots",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InputParameterSnapshots_StudyTemplateId",
                table: "InputParameterSnapshots",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_InputParameterId",
                table: "MeasuredValues",
                column: "InputParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_StudyId",
                table: "MeasuredValues",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitionSnapshots_StudyTemplateId",
                table: "ResultDefinitionSnapshots",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultDefinitionSnapshots_UnitId",
                table: "ResultDefinitionSnapshots",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_Code",
                table: "Samples",
                column: "Code",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrderId",
                table: "Samples",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_VolumeUnitId",
                table: "Samples",
                column: "VolumeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_SampleId",
                table: "Studies",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_StudyTemplateId",
                table: "Studies",
                column: "StudyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyTemplateSnapshots_Name_Revision",
                table: "StudyTemplateSnapshots",
                columns: new[] { "Name", "Revision" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_ResultDefinitionId",
                table: "TestResults",
                column: "ResultDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_StudyId",
                table: "TestResults",
                column: "StudyId");

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
                name: "CalculationRuleCalculationRuleSnapshots");

            migrationBuilder.DropTable(
                name: "MeasuredValues");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "InputParameterSnapshots");

            migrationBuilder.DropTable(
                name: "ResultDefinitionSnapshots");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "Samples");

            migrationBuilder.DropTable(
                name: "StudyTemplateSnapshots");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "UnitSnapshots");
        }
    }
}
