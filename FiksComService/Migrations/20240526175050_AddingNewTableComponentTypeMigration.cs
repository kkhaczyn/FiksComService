using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FiksComService.Migrations
{
    /// <inheritdoc />
    public partial class AddingNewTableComponentTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComponentType",
                table: "Components",
                newName: "ComponentTypeCode");

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Code);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7f0d7223-1519-490c-8a4d-76f92eb02474", "AQAAAAIAAYagAAAAEPp/fbESSfpQFaSkDiomNuUCiYUOUjWfc3R9dA+h+PVV/zAYrGtLPCBkvzQuXwZjtQ==", "5b4ea04b-b2ae-4b0b-ab7c-5bafb4bd921f" });

            migrationBuilder.InsertData(
                table: "ComponentTypes",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "GPU", "Karta graficzna" },
                    { "PROCESSOR", "Procesor" },
                    { "RAM", "Pamięć RAM" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentTypeCode",
                table: "Components",
                column: "ComponentTypeCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_ComponentTypes_ComponentTypeCode",
                table: "Components",
                column: "ComponentTypeCode",
                principalTable: "ComponentTypes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_ComponentTypes_ComponentTypeCode",
                table: "Components");

            migrationBuilder.DropTable(
                name: "ComponentTypes");

            migrationBuilder.DropIndex(
                name: "IX_Components_ComponentTypeCode",
                table: "Components");

            migrationBuilder.RenameColumn(
                name: "ComponentTypeCode",
                table: "Components",
                newName: "ComponentType");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f55878b6-8a31-4e4c-b321-1e33b6c91aa3", "AQAAAAIAAYagAAAAEHVP15pfvZ+4o9hzM/EUxGgBlaLunUNpPgpNWHW/kXhwVlr4DZa9JcUoqQeWTOwd0w==", "1cf193cb-02e3-4547-9203-667176bac17f" });
        }
    }
}
