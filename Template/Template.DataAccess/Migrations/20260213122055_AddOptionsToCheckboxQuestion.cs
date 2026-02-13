using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Template.DataAccess.MsSql.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionsToCheckboxQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Options",
                schema: "template",
                table: "questions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                schema: "template",
                table: "questions",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                schema: "template",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "QuestionType",
                schema: "template",
                table: "questions");
        }
    }
}
