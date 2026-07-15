using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTrackingSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Adds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "StudentQuizSubmissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "StudentQuizSubmissions");
        }
    }
}
