using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MKxStore247.Migrations
{
    /// <inheritdoc />
    public partial class MCategoryShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shop_AspNetUsers_MainCategoryId",
                table: "Shop");

            migrationBuilder.DropForeignKey(
                name: "FK_Shop_CategoryProduct_MainCategoryProductCategoryId",
                table: "Shop");

            migrationBuilder.DropIndex(
                name: "IX_Shop_MainCategoryProductCategoryId",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "MainCategoryProductCategoryId",
                table: "Shop");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Shop",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "MainCategoryId",
                table: "Shop",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_OwnerId",
                table: "Shop",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_AspNetUsers_OwnerId",
                table: "Shop",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_CategoryProduct_MainCategoryId",
                table: "Shop",
                column: "MainCategoryId",
                principalTable: "CategoryProduct",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shop_AspNetUsers_OwnerId",
                table: "Shop");

            migrationBuilder.DropForeignKey(
                name: "FK_Shop_CategoryProduct_MainCategoryId",
                table: "Shop");

            migrationBuilder.DropIndex(
                name: "IX_Shop_OwnerId",
                table: "Shop");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Shop",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MainCategoryId",
                table: "Shop",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MainCategoryProductCategoryId",
                table: "Shop",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Shop_MainCategoryProductCategoryId",
                table: "Shop",
                column: "MainCategoryProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_AspNetUsers_MainCategoryId",
                table: "Shop",
                column: "MainCategoryId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_CategoryProduct_MainCategoryProductCategoryId",
                table: "Shop",
                column: "MainCategoryProductCategoryId",
                principalTable: "CategoryProduct",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
