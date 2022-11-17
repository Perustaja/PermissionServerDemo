using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PermissionServerDemo.Api.Migrations
{
    public partial class demo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    RegNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailUri = table.Column<string>(type: "TEXT", nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsGrounded = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.RegNumber);
                });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "IsGrounded", "Model", "TenantId", "ThumbnailUri" },
                values: new object[] { "N5342K", false, "Piper Archer", new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), "N5342K.jpg" });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "IsGrounded", "Model", "TenantId", "ThumbnailUri" },
                values: new object[] { "N772GK", false, "Cessna 172S", new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), "N772GK.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aircraft");
        }
    }
}
