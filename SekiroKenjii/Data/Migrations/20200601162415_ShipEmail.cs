using Microsoft.EntityFrameworkCore.Migrations;

namespace SekiroKenjii.Migrations
{
    public partial class ShipEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShipEmail",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShipEmail",
                table: "Orders");
        }
    }
}
