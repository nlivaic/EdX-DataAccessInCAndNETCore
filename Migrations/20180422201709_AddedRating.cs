using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MovieApp.Migrations
{
    public partial class AddedRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "film_rating_index",
                table: "film");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "film");

            migrationBuilder.AddColumn<string>(
                name: "RatingCode",
                table: "film",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingId",
                table: "film",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "rating",
                columns: table => new
                {
                    RatingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rating", x => x.RatingId);
                });

            migrationBuilder.CreateIndex(
                name: "film_rating_index",
                table: "film",
                column: "RatingCode");

            migrationBuilder.CreateIndex(
                name: "IX_film_RatingId",
                table: "film",
                column: "RatingId");

            migrationBuilder.AddForeignKey(
                name: "FK_film_rating_RatingId",
                table: "film",
                column: "RatingId",
                principalTable: "rating",
                principalColumn: "RatingId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_film_rating_RatingId",
                table: "film");

            migrationBuilder.DropTable(
                name: "rating");

            migrationBuilder.DropIndex(
                name: "film_rating_index",
                table: "film");

            migrationBuilder.DropIndex(
                name: "IX_film_RatingId",
                table: "film");

            migrationBuilder.DropColumn(
                name: "RatingCode",
                table: "film");

            migrationBuilder.DropColumn(
                name: "RatingId",
                table: "film");

            migrationBuilder.AddColumn<string>(
                name: "Rating",
                table: "film",
                maxLength: 45,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "film_rating_index",
                table: "film",
                column: "Rating");
        }
    }
}
