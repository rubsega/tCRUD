using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tCRUD.Migrations
{
    /// <inheritdoc />
    public partial class VentaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VentaItem_Productos_ProductoId",
                table: "VentaItem");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaItem_Productos_ProductoId",
                table: "VentaItem",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VentaItem_Productos_ProductoId",
                table: "VentaItem");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaItem_Productos_ProductoId",
                table: "VentaItem",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
