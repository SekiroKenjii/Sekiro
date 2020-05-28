using Microsoft.EntityFrameworkCore.Migrations;

namespace SekiroKenjii.Data.Migrations
{
    public partial class AddSupplierToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    BrandImage = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    Country = table.Column<string>(nullable: false),
                    Region = table.Column<string>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    HomePage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
