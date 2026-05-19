using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auctions.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentPrice",
                table: "Listings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Listings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "StartingPrice",
                table: "Listings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WinnerUserId",
                table: "Listings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_WinnerUserId",
                table: "Listings",
                column: "WinnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Listings_AspNetUsers_WinnerUserId",
                table: "Listings",
                column: "WinnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Listings_AspNetUsers_WinnerUserId",
                table: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_Listings_WinnerUserId",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "StartingPrice",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "WinnerUserId",
                table: "Listings");
        }
    }
}
