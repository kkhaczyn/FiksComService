using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiksComService.Migrations
{
    /// <inheritdoc />
    public partial class AddingPropertyForImagesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Components",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5c11fbd0-0734-4d62-bc36-b25538cc8df7", "AQAAAAIAAYagAAAAEMv84T6Ek3+zUu6cZDW00+iEOhcGeJJXn0dG53LwAbJ9FiekyBxbxqsyjQNuECx7HA==", "08b00fc3-9b05-4361-a8da-b7842f69a17a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Components");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0d5e2e4a-5804-41e5-b7d8-df2cbb58fd35", "AQAAAAIAAYagAAAAEMv8gF4AllX/r/y08E+zVOTc9JQpEjkmtAEVHVhnBKROFnvbFjw3+qxtp6k0/LMAgA==", "1ed2380c-e231-4c40-84d0-e5db02957d89" });
        }
    }
}
