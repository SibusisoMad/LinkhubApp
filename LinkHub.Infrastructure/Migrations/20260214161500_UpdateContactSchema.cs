using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Contacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Contacts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NoOfLinkedClients",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Contacts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Contacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7107));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 1, 2 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7109));

            migrationBuilder.UpdateData(
                table: "ClientContacts",
                keyColumns: new[] { "ClientId", "ContactId" },
                keyValues: new object[] { 2, 1 },
                column: "LinkedAt",
                value: new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7110));

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7088), new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7093) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7096), new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7096) });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "Surname", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7101), "dev@example.com", "Team", new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7102) });

            migrationBuilder.UpdateData(
                table: "Contacts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Email", "Surname", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7105), "qa@example.com", "Team", new DateTime(2026, 2, 14, 16, 14, 59, 917, DateTimeKind.Utc).AddTicks(7105) });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Email",
                table: "Contacts",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_Email",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "NoOfLinkedClients",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Contacts");

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
    }
}
