// using System;
// using CoreMultiTenancy.Identity.Interfaces;
// using CoreMultiTenancy.Identity.Tenancy;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;

// namespace CoreMultiTenancy.Identity.Data
// {
//     public class ApplicationDbContext : IdentityDbContext, IMultiTenantDbContext
//     {
//         private readonly Tenant _tenant;
//         public Guid TenantId => _tenant.Id;
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantHook tenantHook)
//             : base(options)
//         {
//             _tenant = tenantHook.GetRequestTenant();
//         }
//         protected override void OnConfiguring(DbContextOptionsBuilder builder)
//         {
//             // Use tenant's specific connection string
//             builder.UseSqlServer(_tenant.ConnectionString);
//         }
//     }
// }