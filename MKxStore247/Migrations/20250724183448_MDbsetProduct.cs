using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MKxStore247.Migrations
{
    /// <inheritdoc />
    public partial class MDbsetProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Product_ProductId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Product_ProductId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_CategoryProduct_CategoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Shop_ShopId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Product_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOption_Product_ProductId",
                table: "ProductOption");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValue_ProductOption_OptionId",
                table: "ProductOptionValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductRating_Product_ProductId",
                table: "ProductRating");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Product_ProductId",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Product_ProductId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_StockImport_Product_ProductId",
                table: "StockImport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOption",
                table: "ProductOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "ProductOptionValue",
                newName: "ProductOptionValues");

            migrationBuilder.RenameTable(
                name: "ProductOption",
                newName: "ProductOptions");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValue_OptionId",
                table: "ProductOptionValues",
                newName: "IX_ProductOptionValues_OptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOption_ProductId",
                table: "ProductOptions",
                newName: "IX_ProductOptions_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ShopId",
                table: "Products",
                newName: "IX_Products_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues",
                column: "ValueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptions",
                table: "ProductOptions",
                column: "OptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Products_ProductId",
                table: "Favorite",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Products_ProductId",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptions_Products_ProductId",
                table: "ProductOptions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_OptionId",
                table: "ProductOptionValues",
                column: "OptionId",
                principalTable: "ProductOptions",
                principalColumn: "OptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRating_Products_ProductId",
                table: "ProductRating",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategoryProduct_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "CategoryProduct",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shop_ShopId",
                table: "Products",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Products_ProductId",
                table: "ProductWishlist",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Products_ProductId",
                table: "Report",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockImport_Products_ProductId",
                table: "StockImport",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Products_ProductId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorite_Products_ProductId",
                table: "Favorite");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Products_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptions_Products_ProductId",
                table: "ProductOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOptionValues_ProductOptions_OptionId",
                table: "ProductOptionValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductRating_Products_ProductId",
                table: "ProductRating");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategoryProduct_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shop_ShopId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWishlist_Products_ProductId",
                table: "ProductWishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Products_ProductId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_StockImport_Products_ProductId",
                table: "StockImport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptionValues",
                table: "ProductOptionValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOptions",
                table: "ProductOptions");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "ProductOptionValues",
                newName: "ProductOptionValue");

            migrationBuilder.RenameTable(
                name: "ProductOptions",
                newName: "ProductOption");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ShopId",
                table: "Product",
                newName: "IX_Product_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Product",
                newName: "IX_Product_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptionValues_OptionId",
                table: "ProductOptionValue",
                newName: "IX_ProductOptionValue_OptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOptions_ProductId",
                table: "ProductOption",
                newName: "IX_ProductOption_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOptionValue",
                table: "ProductOptionValue",
                column: "ValueId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOption",
                table: "ProductOption",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Product_ProductId",
                table: "CartItem",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorite_Product_ProductId",
                table: "Favorite",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product_ProductId",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_CategoryProduct_CategoryId",
                table: "Product",
                column: "CategoryId",
                principalTable: "CategoryProduct",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Shop_ShopId",
                table: "Product",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Product_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOption_Product_ProductId",
                table: "ProductOption",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOptionValue_ProductOption_OptionId",
                table: "ProductOptionValue",
                column: "OptionId",
                principalTable: "ProductOption",
                principalColumn: "OptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRating_Product_ProductId",
                table: "ProductRating",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWishlist_Product_ProductId",
                table: "ProductWishlist",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Product_ProductId",
                table: "Report",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockImport_Product_ProductId",
                table: "StockImport",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
