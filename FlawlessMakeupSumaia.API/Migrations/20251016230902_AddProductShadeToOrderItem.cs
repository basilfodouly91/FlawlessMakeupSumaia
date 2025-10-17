using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlawlessMakeupSumaia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductShadeToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only add new fields to OrderItems - Orders fields already exist
            migrationBuilder.AddColumn<int>(
                name: "ProductShadeId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductShadeName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductShadeId",
                table: "OrderItems",
                column: "ProductShadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductShades_ProductShadeId",
                table: "OrderItems",
                column: "ProductShadeId",
                principalTable: "ProductShades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductShades_ProductShadeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductShadeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductShadeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductShadeName",
                table: "OrderItems");
        }
    }
}
