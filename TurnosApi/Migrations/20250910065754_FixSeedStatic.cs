using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurnosApi.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Turnos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaHora",
                value: new DateTime(2025, 10, 15, 15, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Turnos",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaHora",
                value: new DateTime(2025, 9, 11, 6, 54, 5, 638, DateTimeKind.Utc).AddTicks(9791));
        }
    }
}
