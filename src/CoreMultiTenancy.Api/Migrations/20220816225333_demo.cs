using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreMultiTenancy.Api.Migrations
{
    public partial class demo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    RegNumber = table.Column<string>(nullable: false),
                    ThumbnailUri = table.Column<string>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: false),
                    IsGrounded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.RegNumber);
                });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "IsGrounded", "TenantId", "ThumbnailUri" },
                values: new object[] { "N772GK", false, new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), "N772GK.jpg" });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "IsGrounded", "TenantId", "ThumbnailUri" },
                values: new object[] { "N5342K", false, new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), "N5342K.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aircraft");
        }
    }
}
