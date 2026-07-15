using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentTrackingSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddImasgeForTeachers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Teatchers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Teatchers");
        }
    }
}
