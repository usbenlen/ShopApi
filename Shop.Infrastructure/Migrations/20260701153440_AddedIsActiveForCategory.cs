using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActiveForCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "categories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Visible",
                table: "categories",
                newName: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "categories",
                newName: "Visible");
        }
    }
}
