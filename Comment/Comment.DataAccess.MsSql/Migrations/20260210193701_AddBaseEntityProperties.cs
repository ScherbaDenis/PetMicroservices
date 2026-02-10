using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comment.DataAccess.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "Comment",
                table: "templates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "Comment",
                table: "templates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Comment",
                table: "templates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "Comment",
                table: "comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                schema: "Comment",
                table: "comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Comment",
                table: "comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "Comment",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "Comment",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Comment",
                table: "templates");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "Comment",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                schema: "Comment",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Comment",
                table: "comments");
        }
    }
}
