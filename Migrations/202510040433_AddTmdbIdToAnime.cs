using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeHub.Migrations
{
    /// <inheritdoc />
    public partial class AddTmdbIdToAnime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TmdbId",
                table: "Animes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TmdbId",
                table: "Animes");
        }
    }
}