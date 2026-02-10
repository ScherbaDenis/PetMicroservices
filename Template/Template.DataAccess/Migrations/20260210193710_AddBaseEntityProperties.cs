using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.DataAccess.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "template",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "template",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "template",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "template",
                table: "topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "template",
                table: "topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "template",
                table: "topics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "template",
                table: "templates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "template",
                table: "templates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "template",
                table: "templates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "template",
                table: "tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "template",
                table: "tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "template",
                table: "tags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "template",
                table: "questions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "template",
                table: "questions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "template",
                table: "questions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "template",
                table: "users");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "template",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "template",
                table: "users");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "template",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "template",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "template",
                table: "topics");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "template",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "template",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "template",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "template",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "template",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "template",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "template",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "template",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "template",
                table: "questions");
        }
    }
}
