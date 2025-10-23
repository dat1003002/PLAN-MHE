using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PLANMHE.Migrations
{
    /// <inheritdoc />
    public partial class addmodelplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Plans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Plans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_CreatorId",
                table: "Plans",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Users_CreatorId",
                table: "Plans",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Users_CreatorId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_CreatorId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Plans");
        }
    }
}
