using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_database : Migration
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
                    OrderStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GatherDateBegin = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    GatherDateEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    VolumeValue = table.Column<double>(type: "numeric(18,4)", nullable: true),
                    VolumeUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SampleStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Samples_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Studies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "MeasuredValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudyId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParameterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "numeric(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredValues", x => x.Id);
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
                    IsOutOfSpec = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_Studies_StudyId",
                        column: x => x.StudyId,
                        principalTable: "Studies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_StudyId",
                table: "MeasuredValues",
                column: "StudyId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Code",
                table: "Orders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_Code",
                table: "Samples",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrderId",
                table: "Samples",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Studies_SampleId",
                table: "Studies",
                column: "SampleId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_StudyId",
                table: "TestResults",
                column: "StudyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuredValues");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropTable(
                name: "Studies");

            migrationBuilder.DropTable(
                name: "Samples");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
