﻿// <auto-generated />
using System;
using CoreMultiTenancy.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreMultiTenancy.Identity.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5");

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LogoUri")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerUserId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("RequiresConfirmationForNewUsers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Organizations");

                    b.HasData(
                        new
                        {
                            Id = new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"),
                            CreationDate = new DateTime(2022, 8, 16, 22, 57, 2, 482, DateTimeKind.Utc).AddTicks(8558),
                            IsActive = true,
                            LogoUri = "tenantlogo1.jpg",
                            OwnerUserId = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            RequiresConfirmationForNewUsers = false,
                            Title = "MyCompany"
                        },
                        new
                        {
                            Id = new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"),
                            CreationDate = new DateTime(2022, 8, 16, 22, 57, 2, 483, DateTimeKind.Utc).AddTicks(303),
                            IsActive = true,
                            LogoUri = "tenantlogo2.jpg",
                            OwnerUserId = new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"),
                            RequiresConfirmationForNewUsers = false,
                            Title = "OtherCompany"
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.Permission", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsObsolete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte>("PermCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("VisibleToUser")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PermCategoryId");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            Id = (byte)0,
                            Description = "",
                            IsObsolete = false,
                            Name = "Default",
                            PermCategoryId = (byte)0,
                            VisibleToUser = false
                        },
                        new
                        {
                            Id = (byte)1,
                            Description = "",
                            IsObsolete = false,
                            Name = "All",
                            PermCategoryId = (byte)0,
                            VisibleToUser = false
                        },
                        new
                        {
                            Id = (byte)2,
                            Description = "",
                            IsObsolete = false,
                            Name = "Create Aircraft",
                            PermCategoryId = (byte)1,
                            VisibleToUser = false
                        },
                        new
                        {
                            Id = (byte)3,
                            Description = "Users with this role can edit and ground aircraft.",
                            IsObsolete = false,
                            Name = "Edit Aircraft",
                            PermCategoryId = (byte)1,
                            VisibleToUser = false
                        },
                        new
                        {
                            Id = (byte)4,
                            Description = "",
                            IsObsolete = false,
                            Name = "Delete Aircraft",
                            PermCategoryId = (byte)1,
                            VisibleToUser = false
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.PermissionCategory", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsObsolete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PermissionCategories");

                    b.HasData(
                        new
                        {
                            Id = (byte)0,
                            IsObsolete = false,
                            Name = "Default"
                        },
                        new
                        {
                            Id = (byte)1,
                            IsObsolete = false,
                            Name = "Aircraft"
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<bool>("IsGlobal")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<Guid?>("OrgId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.HasIndex("OrgId");

                    b.ToTable("AspNetRoles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            ConcurrencyStamp = "a0191735-599a-4efc-a272-8ac1b4a50591",
                            Description = "",
                            IsGlobal = true,
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = new Guid("77a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            ConcurrencyStamp = "acec8408-32d2-4323-bc4f-b851cacbd9a4",
                            Description = "",
                            IsGlobal = true,
                            Name = "User",
                            NormalizedName = "USER"
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.RolePermission", b =>
                {
                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.Property<byte>("PermissionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions");

                    b.HasData(
                        new
                        {
                            RoleId = new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            PermissionId = (byte)1
                        },
                        new
                        {
                            RoleId = new Guid("77a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            PermissionId = (byte)0
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "ec5b53c2-c31c-4f37-9c85-d0aad5d810ea",
                            Email = "admin@mydomain.com",
                            EmailConfirmed = true,
                            FirstName = "Admin",
                            LastName = "Admin",
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@MYDOMAIN.COM",
                            NormalizedUserName = "ADMIN@MYDOMAIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEG79lZ7tfECmrTVBxiAmO4emg7aG1ql586GjDCakerAukhN1N9Maf2t8SC2e54RDCA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "00000000-0000-0000-0000-000000000000",
                            TwoFactorEnabled = false,
                            UserName = "admin@mydomain.com"
                        },
                        new
                        {
                            Id = new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"),
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "706e363f-87d5-4092-a7d0-1627e5d7219a",
                            Email = "shadow@mydomain.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "SHADOW@MYDOMAIN.COM",
                            NormalizedUserName = "SHADOW@MYDOMAIN.COM",
                            PasswordHash = "AQAAAAEAACcQAAAAEGu/FxqOUapBWRNzrTi07JbA7P9T+EyZ23hQB1hGgGT1yuFnVA3tsNCoVIO/WmencA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "00000000-0000-0000-0000-000000000000",
                            TwoFactorEnabled = false,
                            UserName = "shadow@mydomain.com"
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.UserOrganization", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrgId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("AwaitingApproval")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Blacklisted")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DateApproved")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateBlacklisted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateSubmitted")
                        .HasColumnType("TEXT");

                    b.Property<string>("InternalNotes")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "OrgId");

                    b.HasIndex("OrgId");

                    b.ToTable("UserOrganizations");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            OrgId = new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"),
                            AwaitingApproval = true,
                            Blacklisted = false,
                            DateSubmitted = new DateTime(2022, 8, 16, 0, 0, 0, 0, DateTimeKind.Local)
                        },
                        new
                        {
                            UserId = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            OrgId = new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"),
                            AwaitingApproval = true,
                            Blacklisted = false,
                            DateSubmitted = new DateTime(2022, 8, 16, 0, 0, 0, 0, DateTimeKind.Local)
                        });
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.UserOrganizationRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrgId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "OrgId", "RoleId");

                    b.HasIndex("OrgId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserOrganizationRoles");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            OrgId = new Guid("77a5570f-3ce5-48ba-9461-80283ed1d94d"),
                            RoleId = new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d")
                        },
                        new
                        {
                            UserId = new Guid("79a7570f-3ce5-48ba-9461-80283ed1d94d"),
                            OrgId = new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"),
                            RoleId = new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d")
                        },
                        new
                        {
                            UserId = new Guid("77a6570f-3ce5-48ba-9461-80283ed1d94d"),
                            OrgId = new Guid("77a6550f-3ce5-48ba-9461-80283ed1d94d"),
                            RoleId = new Guid("78a7570f-3ce5-48ba-9461-80283ed1d94d")
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.Permission", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.PermissionCategory", "PermCategory")
                        .WithMany()
                        .HasForeignKey("PermCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.Role", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Organization", "Organization")
                        .WithMany("Roles")
                        .HasForeignKey("OrgId");
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.RolePermission", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreMultiTenancy.Identity.Entities.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.Entities.UserOrganization", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Organization", "Organization")
                        .WithMany("UserOrganizations")
                        .HasForeignKey("OrgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", "User")
                        .WithMany("UserOrganizations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CoreMultiTenancy.Identity.UserOrganizationRole", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Organization", "Organization")
                        .WithMany("UserOrganizationRoles")
                        .HasForeignKey("OrgId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreMultiTenancy.Identity.Entities.Role", "Role")
                        .WithMany("UserOrganizationRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", "User")
                        .WithMany("UserOrganizationRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("CoreMultiTenancy.Identity.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
