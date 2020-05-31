using Microsoft.EntityFrameworkCore.Migrations;

namespace SekiroKenjii.Migrations
{
    public partial class HQBrandImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HQBrandImage",
                table: "Suppliers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HQBrandImage",
                table: "Suppliers");
        }
    }
}
