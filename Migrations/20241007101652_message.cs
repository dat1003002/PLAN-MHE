using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspnetCoreMvcFull.Migrations
{
    /// <inheritdoc />
    public partial class message : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "caosubemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "caosuloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chieudaithanchinhbemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chieudaithanchinhloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chieudaithannoibemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chieudaithannoiloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "docobemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "docoloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "khotieuchuanbemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "khotieuchuanloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "khuondunbemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "khuondunloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "may",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "solinkthanchinh",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "solinkthannoi",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trongluongthanchinhbemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trongluongthanchinhloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trongluongthannoibemat",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trongluongthannoiloplot",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "caosubemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "caosuloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "chieudaithanchinhbemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "chieudaithanchinhloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "chieudaithannoibemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "chieudaithannoiloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "docobemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "docoloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "khotieuchuanbemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "khotieuchuanloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "khuondunbemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "khuondunloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "may",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "solinkthanchinh",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "solinkthannoi",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "trongluongthanchinhbemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "trongluongthanchinhloplot",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "trongluongthannoibemat",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "trongluongthannoiloplot",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "Paginations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentPage = table.Column<int>(type: "int", nullable: false),
                    ItemsPerPage = table.Column<int>(type: "int", nullable: false),
                    TotalItems = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paginations", x => x.Id);
                });
        }
    }
}
