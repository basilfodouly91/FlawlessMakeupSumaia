using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlawlessMakeupSumaia.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProductShadesAndRefactorCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ProductShades table if it exists (from previous attempt)
            migrationBuilder.Sql(@"
                IF OBJECT_ID('dbo.ProductShades', 'U') IS NOT NULL
                    DROP TABLE dbo.ProductShades;
            ");

            // Drop AvailableShades column if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'AvailableShades')
                    ALTER TABLE Products DROP COLUMN AvailableShades;
            ");

            // Drop ProductShadeId column if exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.CartItems') AND name = 'ProductShadeId')
                    ALTER TABLE CartItems DROP COLUMN ProductShadeId;
            ");

            // Add ProductShadeId column to CartItems
            migrationBuilder.AddColumn<int>(
                name: "ProductShadeId",
                table: "CartItems",
                type: "int",
                nullable: true);

            // Create ProductShades table
            migrationBuilder.CreateTable(
                name: "ProductShades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductShades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductShades_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductShadeId",
                table: "CartItems",
                column: "ProductShadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductShades_ProductId",
                table: "ProductShades",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_ProductShades_ProductShadeId",
                table: "CartItems",
                column: "ProductShadeId",
                principalTable: "ProductShades",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_ProductShades_ProductShadeId",
                table: "CartItems");

            migrationBuilder.DropTable(
                name: "ProductShades");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ProductShadeId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductShadeId",
                table: "CartItems");

            migrationBuilder.AddColumn<string>(
                name: "AvailableShades",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
