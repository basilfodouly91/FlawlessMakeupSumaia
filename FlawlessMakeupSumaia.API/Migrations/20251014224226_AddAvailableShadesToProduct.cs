using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlawlessMakeupSumaia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailableShadesToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvailableShades",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableShades",
                table: "Products");
        }
    }
}
