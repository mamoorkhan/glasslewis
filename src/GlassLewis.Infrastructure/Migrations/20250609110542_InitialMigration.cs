using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlassLewis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StockTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Exchange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Isin = table.Column<string>(type: "nchar(12)", fixedLength: true, maxLength: 12, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: ["Id", "CreatedAt", "Exchange", "Isin", "Name", "StockTicker", "UpdatedAt", "Website"],
                values: new object[,]
                {
                    { new Guid("16511881-10fd-4772-9b62-f1b78513d8af"), new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pink Sheets", "US1104193065", "British Airways Plc", "BAIRY", new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("8973d1e2-3020-49db-862b-2d454746b42d"), new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "NASDAQ", "US0378331005", "Apple Inc.", "AAPL", new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "http://www.apple.com" },
                    { new Guid("e8146538-3991-4877-9b76-f276b9849d45"), new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Euronext Amsterdam", "NL0000009165", "Heineken NV", "HEIA", new DateTime(2025, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Isin",
                table: "Companies",
                column: "Isin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_StockTicker",
                table: "Companies",
                column: "StockTicker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
