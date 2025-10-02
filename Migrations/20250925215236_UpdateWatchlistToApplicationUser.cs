using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWatchlistToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_Users_UserId",
                table: "Watchlists");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Watchlists",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Watchlists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId1",
                table: "Watchlists",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_ApplicationUser_UserId",
                table: "Watchlists",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_Users_UserId1",
                table: "Watchlists",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_ApplicationUser_UserId",
                table: "Watchlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Watchlists_Users_UserId1",
                table: "Watchlists");

            migrationBuilder.DropIndex(
                name: "IX_Watchlists_UserId1",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Watchlists");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Watchlists",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Watchlists_Users_UserId",
                table: "Watchlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
