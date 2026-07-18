using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tCRUD.Migrations
{
    /// <inheritdoc />
    public partial class IdentificadoresPublicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Folio",
                table: "Ventas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "Productos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Folio",
                table: "Compras",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Folio",
                table: "Ventas",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Uuid",
                table: "Productos",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_Folio",
                table: "Compras",
                column: "Folio",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ventas_Folio",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Productos_Uuid",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Compras_Folio",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "Compras");
        }
    }
}
