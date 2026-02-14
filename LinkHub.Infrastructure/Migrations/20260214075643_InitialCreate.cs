using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientContact1",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "int", nullable: false),
                    ContactsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContact1", x => new { x.ClientsId, x.ContactsId });
                    table.ForeignKey(
                        name: "FK_ClientContact1_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientContact1_Contacts_ContactsId",
                        column: x => x.ContactsId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => new { x.ClientId, x.ContactId });
                    table.ForeignKey(
                        name: "FK_ClientContacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "ClientCode", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "UDE001", new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5761), "Udemy", new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5765) },
                    { 2, "WTC002", new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5767), "weThinkCode", new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5767) }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "DEV" },
                    { 2, "QA" }
                });

            migrationBuilder.InsertData(
                table: "ClientContacts",
                columns: new[] { "ClientId", "ContactId", "LinkedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5772) },
                    { 1, 2, new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5773) },
                    { 2, 1, new DateTime(2026, 2, 14, 7, 56, 43, 388, DateTimeKind.Utc).AddTicks(5774) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientContact1_ContactsId",
                table: "ClientContact1",
                column: "ContactsId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ContactId",
                table: "ClientContacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientCode",
                table: "Clients",
                column: "ClientCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientContact1");

            migrationBuilder.DropTable(
                name: "ClientContacts");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
