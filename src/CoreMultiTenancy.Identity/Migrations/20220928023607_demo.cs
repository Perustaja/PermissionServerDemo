﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreMultiTenancy.Identity.Migrations
{
    public partial class demo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LogoUri = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresConfirmationForNewUsers = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsObsolete = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrgId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsGlobal = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlobalAdminDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlobalDefaultForNewUsers = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTenantDefaultForNewUsers = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrgId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AwaitingApproval = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blacklisted = table.Column<bool>(type: "INTEGER", nullable: false),
                    InternalNotes = table.Column<string>(type: "TEXT", nullable: true),
                    DateSubmitted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateApproved = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateBlacklisted = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => new { x.UserId, x.OrgId });
                    table.ForeignKey(
                        name: "FK_UserOrganizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    IsObsolete = table.Column<bool>(type: "INTEGER", nullable: false),
                    PermCategoryId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionCategories_PermCategoryId",
                        column: x => x.PermCategoryId,
                        principalTable: "PermissionCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizationRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrgId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizationRoles", x => new { x.UserId, x.OrgId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserOrganizationRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizationRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizationRoles_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PermissionId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "IsGlobalAdminDefault", "IsGlobalDefaultForNewUsers", "IsTenantDefaultForNewUsers", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("75a7570f-3ce5-48ba-9461-80283ed1d94d"), "2559330e-5148-424b-b4da-2c8ff3e99c68", "Role for creating new aircraft", true, false, false, false, "Create Aircraft", "CREATE AIRCRAFT", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "IsGlobalAdminDefault", "IsGlobalDefaultForNewUsers", "IsTenantDefaultForNewUsers", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("77a7570f-3ce5-48ba-9461-80283ed1d94d"), "5f91c305-f8dd-4937-bfa2-5240b2eb4c2d", "Default user role with minimal permissions", true, false, true, false, "User", "USER", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "IsGlobalAdminDefault", "IsGlobalDefaultForNewUsers", "IsTenantDefaultForNewUsers", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), "70e7f310-7627-4794-a763-cdebba54f23a", "Default admin role for new tenant owners", true, true, false, false, "Owner", "OWNER", null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"), 0, "b6edbcbe-16b2-4aed-abe8-c93839e7e048", "shadow@mydomain.com", true, null, null, false, null, "SHADOW@MYDOMAIN.COM", "SHADOW@MYDOMAIN.COM", "AQAAAAEAACcQAAAAEFiAUanrEdLgV7xbqLpEMct3NZ6jBZjOpL2IRKRX3pOS3+DY/zomqGulu/HdhPxPFQ==", null, false, "00000000-0000-0000-0000-000000000000", false, "shadow@mydomain.com" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), 0, "c4690d12-ca2b-4f6c-bfc7-ee420b61139a", "admin@mydomain.com", true, "Admin", "Admin", false, null, "ADMIN@MYDOMAIN.COM", "ADMIN@MYDOMAIN.COM", "AQAAAAEAACcQAAAAEE0m920wLJdOZ1mPnJgnjj8Ak6W0w2Gt+pkYlYksdcQ625S82/mr1gtmTcJ9W491Qg==", null, false, "00000000-0000-0000-0000-000000000000", false, "admin@mydomain.com" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "CreationDate", "IsActive", "LogoUri", "OwnerUserId", "RequiresConfirmationForNewUsers", "Title" },
                values: new object[] { new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new DateTime(2022, 9, 28, 2, 36, 6, 849, DateTimeKind.Utc).AddTicks(9806), true, "tenantlogo1.jpg", new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), false, "MyCompany" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "CreationDate", "IsActive", "LogoUri", "OwnerUserId", "RequiresConfirmationForNewUsers", "Title" },
                values: new object[] { new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new DateTime(2022, 9, 28, 2, 36, 6, 849, DateTimeKind.Utc).AddTicks(9810), true, "tenantlogo2.jpg", new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"), false, "OtherCompany" });

            migrationBuilder.InsertData(
                table: "PermissionCategories",
                columns: new[] { "Id", "IsObsolete", "Name" },
                values: new object[] { "Aircraft", false, "Aircraft" });

            migrationBuilder.InsertData(
                table: "PermissionCategories",
                columns: new[] { "Id", "IsObsolete", "Name" },
                values: new object[] { "Roles", false, "Roles" });

            migrationBuilder.InsertData(
                table: "PermissionCategories",
                columns: new[] { "Id", "IsObsolete", "Name" },
                values: new object[] { "Users", false, "Users" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "AircraftCreate", "Users with this permission can create new aircraft within the tenant.", false, "Create Aircraft", "Aircraft" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "AircraftDelete", "Users with this permission can delete aircraft within the tenant.", false, "Delete Aircraft", "Aircraft" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "AircraftEdit", "Users with this permission can edit aircraft within the tenant.", false, "Edit Aircraft", "Aircraft" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "RolesCreate", "Users with this permission can create roles within the tenant.", false, "Create Roles", "Roles" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "RolesDelete", "Users with this permission can delete roles within the tenant.", false, "Delete Roles", "Roles" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "RolesEdit", "Users with this permission can edit roles within the tenant.", false, "Edit Roles", "Roles" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "UsersManageAccess", "Users with this permission can revoke access for users within the tenant.", false, "Manage Users' Access", "Users" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId" },
                values: new object[] { "UsersManageRoles", "Users with this permission can add or remove users' roles within the tenant.", false, "Manage Users' Roles", "Users" });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "OrgId", "RoleId", "UserId" },
                values: new object[] { new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "OrgId", "RoleId", "UserId" },
                values: new object[] { new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("75a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "OrgId", "RoleId", "UserId" },
                values: new object[] { new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "OrgId", "RoleId", "UserId" },
                values: new object[] { new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizations",
                columns: new[] { "OrgId", "UserId", "AwaitingApproval", "Blacklisted", "DateApproved", "DateBlacklisted", "DateSubmitted", "InternalNotes" },
                values: new object[] { new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), false, false, new DateTime(2022, 9, 27, 0, 0, 0, 0, DateTimeKind.Local), null, new DateTime(2022, 9, 27, 0, 0, 0, 0, DateTimeKind.Local), null });

            migrationBuilder.InsertData(
                table: "UserOrganizations",
                columns: new[] { "OrgId", "UserId", "AwaitingApproval", "Blacklisted", "DateApproved", "DateBlacklisted", "DateSubmitted", "InternalNotes" },
                values: new object[] { new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), false, false, new DateTime(2022, 9, 27, 0, 0, 0, 0, DateTimeKind.Local), null, new DateTime(2022, 9, 27, 0, 0, 0, 0, DateTimeKind.Local), null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "AircraftCreate", new Guid("75a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "AircraftCreate", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "AircraftDelete", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "AircraftEdit", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "RolesCreate", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "RolesDelete", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "RolesEdit", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "UsersManageAccess", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { "UsersManageRoles", new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_OrgId",
                table: "AspNetRoles",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermCategoryId",
                table: "Permissions",
                column: "PermCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizationRoles_OrgId",
                table: "UserOrganizationRoles",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizationRoles_RoleId",
                table: "UserOrganizationRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrgId",
                table: "UserOrganizations",
                column: "OrgId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserOrganizationRoles");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PermissionCategories");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}