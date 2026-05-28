using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRequireLoginToPurchaseSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireLoginToPurchase",
                table: "SystemSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireLoginToPurchase",
                table: "SystemSettings");
        }
    }
}
