using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiksComService.Migrations
{
    /// <inheritdoc />
    public partial class InvoicesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    DocumentGuid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f55878b6-8a31-4e4c-b321-1e33b6c91aa3", "AQAAAAIAAYagAAAAEHVP15pfvZ+4o9hzM/EUxGgBlaLunUNpPgpNWHW/kXhwVlr4DZa9JcUoqQeWTOwd0w==", "1cf193cb-02e3-4547-9203-667176bac17f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c11fbd0-0734-4d62-bc36-b25538cc8df7", "AQAAAAIAAYagAAAAEMv84T6Ek3+zUu6cZDW00+iEOhcGeJJXn0dG53LwAbJ9FiekyBxbxqsyjQNuECx7HA==", "08b00fc3-9b05-4361-a8da-b7842f69a17a" });
        }
    }
}
