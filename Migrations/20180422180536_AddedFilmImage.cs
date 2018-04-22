using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieApp.Migrations
{
    public partial class AddedFilmImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilmImageId",
                table: "film",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FilmImage",
                columns: table => new
                {
                    FilmImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FilmId = table.Column<int>(type: "int(11)", nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true),
                    Title = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmImage", x => x.FilmImageId);
                    table.ForeignKey(
                        name: "FK_FilmImage_film_FilmId",
                        column: x => x.FilmId,
                        principalTable: "film",
                        principalColumn: "FilmId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmImage_FilmId",
                table: "FilmImage",
                column: "FilmId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmImage");

            migrationBuilder.DropColumn(
                name: "FilmImageId",
                table: "film");
        }
    }
}
