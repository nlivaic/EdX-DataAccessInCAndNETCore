using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "actor",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    LastName = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actor", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "film",
                columns: table => new
                {
                    FilmId = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Rating = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    ReleaseYear = table.Column<int>(type: "int(11)", nullable: true),
                    Title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_film", x => x.FilmId);
                });

            migrationBuilder.CreateTable(
                name: "film_actor",
                columns: table => new
                {
                    FilmId = table.Column<int>(type: "int(11)", nullable: false),
                    ActorId = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_film_actor", x => new { x.FilmId, x.ActorId });
                    table.ForeignKey(
                        name: "film_actor_actor",
                        column: x => x.ActorId,
                        principalTable: "actor",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "film_actor_film",
                        column: x => x.FilmId,
                        principalTable: "film",
                        principalColumn: "FilmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "film_category",
                columns: table => new
                {
                    FilmId = table.Column<int>(type: "int(11)", nullable: false),
                    CategoryId = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_film_category", x => new { x.FilmId, x.CategoryId });
                    table.ForeignKey(
                        name: "film_category_category_fk",
                        column: x => x.CategoryId,
                        principalTable: "category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "film_category_film_fk",
                        column: x => x.FilmId,
                        principalTable: "film",
                        principalColumn: "FilmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "film_actor_actor_idx",
                table: "film_actor",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "film_category_category_fk_idx",
                table: "film_category",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "film_actor");

            migrationBuilder.DropTable(
                name: "film_category");

            migrationBuilder.DropTable(
                name: "actor");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "film");
        }
    }
}
