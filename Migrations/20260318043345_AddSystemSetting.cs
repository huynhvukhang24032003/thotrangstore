using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WebsiteName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EnableRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableOrdering = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeoDescription = table.Column<string>(type: "TEXT", nullable: true),
                    SeoKeywords = table.Column<string>(type: "TEXT", nullable: true),
                    Hotline = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    Slogan = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
