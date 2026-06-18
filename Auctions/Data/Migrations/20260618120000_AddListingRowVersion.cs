using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctions.Data.Migrations
{
    [Migration("20260618120000_AddListingRowVersion")]
    public partial class AddListingRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Listings",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Listings");
        }
    }
}
