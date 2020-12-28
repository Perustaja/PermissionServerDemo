using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreMultiTenancy.Identity.Data.Migrations
{
    public partial class seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("2301d884-221a-4e7d-b509-0113dcc043e1"), "fba4f3f6-43a8-4756-b8cc-4d6c0323015f", "", true, "Admin", "ADMIN", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"), "345c1ae0-470a-43b4-ac43-0db77143753e", "", true, "Mechanic", "MECHANIC", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), "801a3b1e-6df7-4905-aa3e-e11bfe86789f", "", true, "Pilot", "PILOT", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { new Guid("2301d884-221a-4e7d-b509-0113dcc043e1"), (byte)1 },
                    { new Guid("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"), (byte)2 },
                    { new Guid("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"), (byte)3 },
                    { new Guid("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"), (byte)4 },
                    { new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), (byte)3 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2301d884-221a-4e7d-b509-0113dcc043e1"),
                column: "ConcurrencyStamp",
                value: "fba4f3f6-43a8-4756-b8cc-4d6c0323015f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"),
                column: "ConcurrencyStamp",
                value: "801a3b1e-6df7-4905-aa3e-e11bfe86789f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("7d9b7113-a8f8-4035-99a7-a20dd400f6a3"),
                column: "ConcurrencyStamp",
                value: "345c1ae0-470a-43b4-ac43-0db77143753e");
        }
    }
}
