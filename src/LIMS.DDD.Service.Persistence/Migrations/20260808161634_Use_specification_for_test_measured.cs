using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIMS.DDD.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Use_specification_for_test_measured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResSpecMin",
                table: "TestResults",
                newName: "SpecMinValue");

            migrationBuilder.RenameColumn(
                name: "ResSpecMax",
                table: "TestResults",
                newName: "SpecMaxValue");

            migrationBuilder.RenameColumn(
                name: "ParamSpecMin",
                table: "MeasuredValues",
                newName: "SpecMinValue");

            migrationBuilder.RenameColumn(
                name: "ParamSpecMax",
                table: "MeasuredValues",
                newName: "SpecMaxValue");

            migrationBuilder.AlterColumn<double>(
                name: "SpecMinValue",
                table: "TestResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric(18,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "SpecMaxValue",
                table: "TestResults",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric(18,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "SpecMinValue",
                table: "MeasuredValues",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric(18,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "SpecMaxValue",
                table: "MeasuredValues",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric(18,6)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecMinValue",
                table: "TestResults",
                newName: "ResSpecMin");

            migrationBuilder.RenameColumn(
                name: "SpecMaxValue",
                table: "TestResults",
                newName: "ResSpecMax");

            migrationBuilder.RenameColumn(
                name: "SpecMinValue",
                table: "MeasuredValues",
                newName: "ParamSpecMin");

            migrationBuilder.RenameColumn(
                name: "SpecMaxValue",
                table: "MeasuredValues",
                newName: "ParamSpecMax");

            migrationBuilder.AlterColumn<double>(
                name: "ResSpecMin",
                table: "TestResults",
                type: "numeric(18,6)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ResSpecMax",
                table: "TestResults",
                type: "numeric(18,6)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ParamSpecMin",
                table: "MeasuredValues",
                type: "numeric(18,6)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ParamSpecMax",
                table: "MeasuredValues",
                type: "numeric(18,6)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
