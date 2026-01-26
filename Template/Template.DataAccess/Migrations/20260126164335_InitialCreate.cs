using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.DataAccess.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "template");

            migrationBuilder.CreateTable(
                name: "topics",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "templates",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TopicId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_templates_topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "template",
                        principalTable: "topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "template",
                        principalTable: "templates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TemplateId",
                schema: "template",
                table: "Questions",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_Id",
                schema: "template",
                table: "tags",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_tags_Name",
                schema: "template",
                table: "tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_tags_TemplateId",
                schema: "template",
                table: "tags",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_Id",
                schema: "template",
                table: "templates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_templates_OwnerId",
                schema: "template",
                table: "templates",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_templates_Title",
                schema: "template",
                table: "templates",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_templates_TopicId",
                schema: "template",
                table: "templates",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_topics_Id",
                schema: "template",
                table: "topics",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_topics_Name",
                schema: "template",
                table: "topics",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_users_Id",
                schema: "template",
                table: "users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_users_Name",
                schema: "template",
                table: "users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_users_TemplateId",
                schema: "template",
                table: "users",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_templates_TemplateId",
                schema: "template",
                table: "Questions",
                column: "TemplateId",
                principalSchema: "template",
                principalTable: "templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tags_templates_TemplateId",
                schema: "template",
                table: "tags",
                column: "TemplateId",
                principalSchema: "template",
                principalTable: "templates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_templates_users_OwnerId",
                schema: "template",
                table: "templates",
                column: "OwnerId",
                principalSchema: "template",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_templates_TemplateId",
                schema: "template",
                table: "users");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "template");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "template");

            migrationBuilder.DropTable(
                name: "templates",
                schema: "template");

            migrationBuilder.DropTable(
                name: "topics",
                schema: "template");

            migrationBuilder.DropTable(
                name: "users",
                schema: "template");
        }
    }
}
