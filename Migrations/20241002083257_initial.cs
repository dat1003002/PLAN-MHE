using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspnetCoreMvcFull.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tenxuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Paginations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    ItemsPerPage = table.Column<int>(type: "int", nullable: false),
                    CurrentPage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paginations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mahang = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quycachloithep = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    khuonlodie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    khuonsoiholder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sosoi = table.Column<int>(type: "int", nullable: true),
                    pitch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tieuchuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thucte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    doday = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soi1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    soi2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sodaycatduoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chieudaicatlon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    chieudaicatnho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tocdomaydun = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tocdokeo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paginations");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
