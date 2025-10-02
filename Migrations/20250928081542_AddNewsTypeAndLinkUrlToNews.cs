using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimeHub.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsTypeAndLinkUrlToNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkUrl",
                table: "News",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NewsType",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ForumPosts",
                columns: table => new
                {
                    ForumPostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ParentPostId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPosts", x => x.ForumPostId);
                    table.ForeignKey(
                        name: "FK_ForumPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumPosts_ForumPosts_ParentPostId",
                        column: x => x.ParentPostId,
                        principalTable: "ForumPosts",
                        principalColumn: "ForumPostId");
                    table.ForeignKey(
                        name: "FK_ForumPosts_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "NewsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_NewsId",
                table: "ForumPosts",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_ParentPostId",
                table: "ForumPosts",
                column: "ParentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPosts_UserId",
                table: "ForumPosts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumPosts");

            migrationBuilder.DropColumn(
                name: "LinkUrl",
                table: "News");

            migrationBuilder.DropColumn(
                name: "NewsType",
                table: "News");
        }
    }
}
