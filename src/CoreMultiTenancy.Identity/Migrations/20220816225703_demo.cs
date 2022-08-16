using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerUserId = table.Column<Guid>(nullable: false),
                    LogoUri = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RequiresConfirmationForNewUsers = table.Column<bool>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionCategories",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsObsolete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
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
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    OrgId = table.Column<Guid>(nullable: true),
                    Description = table.Column<string>(maxLength: 50, nullable: true),
                    IsGlobal = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoles_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    OrgId = table.Column<Guid>(nullable: false),
                    AwaitingApproval = table.Column<bool>(nullable: false),
                    Blacklisted = table.Column<bool>(nullable: false),
                    InternalNotes = table.Column<string>(nullable: true),
                    DateSubmitted = table.Column<DateTime>(nullable: false),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    DateBlacklisted = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => new { x.UserId, x.OrgId });
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsObsolete = table.Column<bool>(nullable: false),
                    VisibleToUser = table.Column<bool>(nullable: false),
                    PermCategoryId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_PermissionCategories_PermCategoryId",
                        column: x => x.PermCategoryId,
                        principalTable: "PermissionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
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
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
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
                    UserId = table.Column<Guid>(nullable: false),
                    OrgId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizationRoles", x => new { x.UserId, x.OrgId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserOrganizationRoles_Organizations_OrgId",
                        column: x => x.OrgId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false),
                    PermissionId = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), "a0191735-599a-4efc-a272-8ac1b4a50591", "", true, "Admin", "ADMIN", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "IsGlobal", "Name", "NormalizedName", "OrgId" },
                values: new object[] { new Guid("77a7570f-3ce5-48ba-9461-80283ed1d94d"), "acec8408-32d2-4323-bc4f-b851cacbd9a4", "", true, "User", "USER", null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), 0, "ec5b53c2-c31c-4f37-9c85-d0aad5d810ea", "admin@mydomain.com", true, "Admin", "Admin", false, null, "ADMIN@MYDOMAIN.COM", "ADMIN@MYDOMAIN.COM", "AQAAAAEAACcQAAAAEG79lZ7tfECmrTVBxiAmO4emg7aG1ql586GjDCakerAukhN1N9Maf2t8SC2e54RDCA==", null, false, "00000000-0000-0000-0000-000000000000", false, "admin@mydomain.com" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"), 0, "706e363f-87d5-4092-a7d0-1627e5d7219a", "shadow@mydomain.com", true, null, null, false, null, "SHADOW@MYDOMAIN.COM", "SHADOW@MYDOMAIN.COM", "AQAAAAEAACcQAAAAEGu/FxqOUapBWRNzrTi07JbA7P9T+EyZ23hQB1hGgGT1yuFnVA3tsNCoVIO/WmencA==", null, false, "00000000-0000-0000-0000-000000000000", false, "shadow@mydomain.com" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "CreationDate", "IsActive", "LogoUri", "OwnerUserId", "RequiresConfirmationForNewUsers", "Title" },
                values: new object[] { new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new DateTime(2022, 8, 16, 22, 57, 2, 482, DateTimeKind.Utc).AddTicks(8558), true, "tenantlogo1.jpg", new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), false, "MyCompany" });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "CreationDate", "IsActive", "LogoUri", "OwnerUserId", "RequiresConfirmationForNewUsers", "Title" },
                values: new object[] { new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new DateTime(2022, 8, 16, 22, 57, 2, 483, DateTimeKind.Utc).AddTicks(303), true, "tenantlogo2.jpg", new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"), false, "OtherCompany" });

            migrationBuilder.InsertData(
                table: "PermissionCategories",
                columns: new[] { "Id", "IsObsolete", "Name" },
                values: new object[] { (byte)0, false, "Default" });

            migrationBuilder.InsertData(
                table: "PermissionCategories",
                columns: new[] { "Id", "IsObsolete", "Name" },
                values: new object[] { (byte)1, false, "Aircraft" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId", "VisibleToUser" },
                values: new object[] { (byte)0, "", false, "Default", (byte)0, false });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId", "VisibleToUser" },
                values: new object[] { (byte)1, "", false, "All", (byte)0, false });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId", "VisibleToUser" },
                values: new object[] { (byte)2, "", false, "Create Aircraft", (byte)1, false });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId", "VisibleToUser" },
                values: new object[] { (byte)3, "Users with this role can edit and ground aircraft.", false, "Edit Aircraft", (byte)1, false });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "IsObsolete", "Name", "PermCategoryId", "VisibleToUser" },
                values: new object[] { (byte)4, "", false, "Delete Aircraft", (byte)1, false });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "UserId", "OrgId", "RoleId" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "UserId", "OrgId", "RoleId" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizationRoles",
                columns: new[] { "UserId", "OrgId", "RoleId" },
                values: new object[] { new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d") });

            migrationBuilder.InsertData(
                table: "UserOrganizations",
                columns: new[] { "UserId", "OrgId", "AwaitingApproval", "Blacklisted", "DateApproved", "DateBlacklisted", "DateSubmitted", "InternalNotes" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"), true, false, null, null, new DateTime(2022, 8, 16, 0, 0, 0, 0, DateTimeKind.Local), null });

            migrationBuilder.InsertData(
                table: "UserOrganizations",
                columns: new[] { "UserId", "OrgId", "AwaitingApproval", "Blacklisted", "DateApproved", "DateBlacklisted", "DateSubmitted", "InternalNotes" },
                values: new object[] { new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"), new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"), true, false, null, null, new DateTime(2022, 8, 16, 0, 0, 0, 0, DateTimeKind.Local), null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[] { new Guid("77a7570f-3ce5-48ba-9461-80283ed1d94d"), (byte)0 });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[] { new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"), (byte)1 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_OrgId",
                table: "AspNetRoles",
                column: "OrgId");

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
