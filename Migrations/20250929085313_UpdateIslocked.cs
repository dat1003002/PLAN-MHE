using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLANMHE.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIslocked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "PlanCells",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "PlanCells");
        }
    }
}
