using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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

            migrationBuilder.InsertData(
                schema: "template",
                table: "Questions",
                columns: new[] { "Id", "Description", "TemplateId", "Title" },
                values: new object[,]
                {
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Please provide your full name", null, "What is your name?" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Please provide a valid email address", null, "What is your email?" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Rate your satisfaction from 1 to 10", null, "How satisfied are you?" }
                });

            migrationBuilder.InsertData(
                schema: "template",
                table: "tags",
                columns: new[] { "Id", "Name", "TemplateId" },
                values: new object[,]
                {
                    { 1, "Programming", null },
                    { 2, "Database", null },
                    { 3, "Web Development", null },
                    { 4, "Machine Learning", null }
                });

            migrationBuilder.InsertData(
                schema: "template",
                table: "templates",
                columns: new[] { "Id", "Description", "OwnerId", "Title", "TopicId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "A template for collecting customer feedback", null, "Customer Feedback Survey", null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "A comprehensive onboarding checklist for new employees", null, "Employee Onboarding Checklist", null }
                });

            migrationBuilder.InsertData(
                schema: "template",
                table: "topics",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Technology" },
                    { 2, "Science" },
                    { 3, "Education" }
                });

            migrationBuilder.InsertData(
                schema: "template",
                table: "users",
                columns: new[] { "Id", "Name", "TemplateId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "John Doe", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Jane Smith", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TemplateId",
                schema: "template",
                table: "Questions",
                column: "TemplateId");

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
                name: "IX_topics_Name",
                schema: "template",
                table: "topics",
                column: "Name");

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
