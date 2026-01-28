using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Comment.DataAccess.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Comment");

            migrationBuilder.CreateTable(
                name: "templates",
                schema: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                schema: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comments_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "Comment",
                        principalTable: "templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                schema: "Comment",
                table: "templates",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Customer Feedback Template" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Product Review Template" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Support Ticket Template" }
                });

            migrationBuilder.InsertData(
                schema: "Comment",
                table: "comments",
                columns: new[] { "Id", "TemplateId", "Text" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), "Great product! Highly recommended." },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("10000000-0000-0000-0000-000000000001"), "The service was excellent and very helpful." },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("10000000-0000-0000-0000-000000000002"), "Good quality, but a bit expensive." },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), "Fast delivery and good packaging." },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new Guid("10000000-0000-0000-0000-000000000003"), "Issue resolved quickly by support team." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_TemplateId",
                schema: "Comment",
                table: "comments",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_Title",
                schema: "Comment",
                table: "templates",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments",
                schema: "Comment");

            migrationBuilder.DropTable(
                name: "templates",
                schema: "Comment");
        }
    }
}
