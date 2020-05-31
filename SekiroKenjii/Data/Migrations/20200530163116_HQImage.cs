using Microsoft.EntityFrameworkCore.Migrations;

namespace SekiroKenjii.Migrations
{
    public partial class HQImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HQImage",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HQImage",
                table: "Products");
        }
    }
}
