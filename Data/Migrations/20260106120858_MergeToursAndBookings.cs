using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourismMVC.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergeToursAndBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    DurationDays = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MaxGroupSize = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    AgencyId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerEmail = table.Column<string>(type: "TEXT", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_TourPackages_TourPackageId",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourPackageId",
                table: "Bookings",
                column: "TourPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "TourPackages");
        }
    }
}
