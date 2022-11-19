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
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailUri = table.Column<string>(type: "TEXT", nullable: true),
                    IsGrounded = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsShadowOwned = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlobal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => new { x.RegNumber, x.TenantId });
                });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "TenantId", "IsGlobal", "IsGrounded", "IsShadowOwned", "Model", "ThumbnailUri" },
                values: new object[] { "N5342K", new Guid("00000000-0000-0000-0000-000000000000"), true, false, true, "Piper Archer", "N5342K.jpg" });

            migrationBuilder.InsertData(
                table: "Aircraft",
                columns: new[] { "RegNumber", "TenantId", "IsGlobal", "IsGrounded", "IsShadowOwned", "Model", "ThumbnailUri" },
                values: new object[] { "N772GK", new Guid("00000000-0000-0000-0000-000000000000"), true, false, false, "Cessna 172S", "N772GK.jpg" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aircraft");
        }
    }
}
