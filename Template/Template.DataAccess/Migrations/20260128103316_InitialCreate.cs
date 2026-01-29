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
                name: "tags",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "templates",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_templates_users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "template",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_questions_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "template",
                        principalTable: "templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagTemplate",
                schema: "template",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTemplate", x => new { x.TagsId, x.TemplateId });
                    table.ForeignKey(
                        name: "FK_TagTemplate_tags_TagsId",
                        column: x => x.TagsId,
                        principalSchema: "template",
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagTemplate_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "template",
                        principalTable: "templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateUser",
                schema: "template",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateUser", x => new { x.TemplateId, x.UsersAccessId });
                    table.ForeignKey(
                        name: "FK_TemplateUser_templates_TemplateId",
                        column: x => x.TemplateId,
                        principalSchema: "template",
                        principalTable: "templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateUser_users_UsersAccessId",
                        column: x => x.UsersAccessId,
                        principalSchema: "template",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_questions_TemplateId",
                schema: "template",
                table: "questions",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_questions_title",
                schema: "template",
                table: "questions",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_tags_name",
                schema: "template",
                table: "tags",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TagTemplate_TemplateId",
                schema: "template",
                table: "TagTemplate",
                column: "TemplateId");

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
                name: "IX_TemplateUser_UsersAccessId",
                schema: "template",
                table: "TemplateUser",
                column: "UsersAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_topics_name",
                schema: "template",
                table: "topics",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_users_name",
                schema: "template",
                table: "users",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questions",
                schema: "template");

            migrationBuilder.DropTable(
                name: "TagTemplate",
                schema: "template");

            migrationBuilder.DropTable(
                name: "TemplateUser",
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
