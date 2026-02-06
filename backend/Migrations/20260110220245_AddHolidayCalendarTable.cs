using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cdr.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHolidayCalendarTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Turkey 2026 public holidays
            migrationBuilder.InsertData(
                table: "HolidayCalendar",
                columns: new[] { "HolidayDate", "HolidayName", "HolidayType", "Year", "HolidayNameEn", "IsWeekend", "SendReportsOnHoliday", "IsActive", "Notes", "CreatedAtUtc", "ModifiedAtUtc" },
                values: new object[,]
                {
                    // New Year's Day
                    { new DateTime(2026, 1, 1), "Yılbaşı", "National", 2026, "New Year's Day", false, false, true, "1 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // National Sovereignty and Children's Day
                    { new DateTime(2026, 4, 23), "Ulusal Egemenlik ve Çocuk Bayramı", "National", 2026, "National Sovereignty and Children's Day", false, false, true, "1 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Labour and Solidarity Day
                    { new DateTime(2026, 5, 1), "Emek ve Dayanışma Günü", "National", 2026, "Labour and Solidarity Day", false, false, true, "1 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Commemoration of Atatürk, Youth and Sports Day
                    { new DateTime(2026, 5, 19), "Atatürk'ü Anma Gençlik ve Spor Bayramı", "National", 2026, "Commemoration of Atatürk, Youth and Sports Day", false, false, true, "1 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Ramadan Feast (Eid al-Fitr) - Day 1
                    { new DateTime(2026, 3, 20), "Ramazan Bayramı 1. Gün", "Religious", 2026, "Ramadan Feast Day 1", false, false, true, "3,5 gün resmi tatil (1. gün yarım gün)", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Ramadan Feast - Day 2
                    { new DateTime(2026, 3, 21), "Ramazan Bayramı 2. Gün", "Religious", 2026, "Ramadan Feast Day 2", false, false, true, "3,5 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Ramadan Feast - Day 3
                    { new DateTime(2026, 3, 22), "Ramazan Bayramı 3. Gün", "Religious", 2026, "Ramadan Feast Day 3", false, false, true, "3,5 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Feast of Sacrifice (Eid al-Adha) - Day 1
                    { new DateTime(2026, 5, 27), "Kurban Bayramı 1. Gün", "Religious", 2026, "Feast of Sacrifice Day 1", false, false, true, "4,5 gün resmi tatil (1. gün yarım gün)", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Feast of Sacrifice - Day 2
                    { new DateTime(2026, 5, 28), "Kurban Bayramı 2. Gün", "Religious", 2026, "Feast of Sacrifice Day 2", false, false, true, "4,5 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Feast of Sacrifice - Day 3
                    { new DateTime(2026, 5, 29), "Kurban Bayramı 3. Gün", "Religious", 2026, "Feast of Sacrifice Day 3", false, false, true, "4,5 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Feast of Sacrifice - Day 4
                    { new DateTime(2026, 5, 30), "Kurban Bayramı 4. Gün", "Religious", 2026, "Feast of Sacrifice Day 4", false, false, true, "4,5 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Victory Day
                    { new DateTime(2026, 8, 30), "Zafer Bayramı", "National", 2026, "Victory Day", false, false, true, "1 gün resmi tatil", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    
                    // Republic Day
                    { new DateTime(2026, 10, 29), "Cumhuriyet Bayramı", "National", 2026, "Republic Day", false, false, true, "1,5 gün resmi tatil (28 Ekim öğleden sonra tatil)", new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all Turkey 2026 holidays
            migrationBuilder.DeleteData(
                table: "HolidayCalendar",
                keyColumn: "Year",
                keyValue: 2026);
        }
    }
}
