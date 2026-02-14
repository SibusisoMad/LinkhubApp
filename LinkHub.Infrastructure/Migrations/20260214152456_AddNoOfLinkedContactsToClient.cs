using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNoOfLinkedContactsToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoOfLinkedContacts",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9731));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 2 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9733));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 2, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9734));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9713), new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9719) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9722), new DateTime(2026, 2, 14, 15, 24, 55, 126, DateTimeKind.Utc).AddTicks(9723) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoOfLinkedContacts",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5772));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 2 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5773));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 2, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5774));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5761), new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5765) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5767), new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5767) });
        }
    }
}
