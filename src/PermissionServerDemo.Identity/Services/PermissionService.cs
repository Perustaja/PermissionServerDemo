using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PermissionServer.Common;
using PermissionServerDemo.Core.Authorization;
using PermissionServerDemo.Identity.Data;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Interfaces;

namespace PermissionServerDemo.Identity.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository<ApplicationDbContext, PermissionEnum, PermissionCategoryEnum> _permRepo;
        private readonly string _connectionString;
        private readonly ApplicationDbContext _applicationContext;

        public PermissionService(IConfiguration config,
            ApplicationDbContext applicationContext,
            IPermissionRepository<ApplicationDbContext, PermissionEnum, PermissionCategoryEnum> permRepo)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _permRepo = permRepo ?? throw new ArgumentNullException(nameof(permRepo));
        }

        public async Task<List<Permission<PermissionEnum, PermissionCategoryEnum>>> GetAllPermissionsAsync() =>
            await _permRepo.GetAllPermissionsAsync();

        public async Task<List<PermissionCategory<PermissionEnum, PermissionCategoryEnum>>> GetAllPermissionCategoriesAsync()
            => await _permRepo.GetAllPermissionCategoriesAsync();

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, string[] perms)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(*) 
                    FROM UserOrganizationRoles uor
                    JOIN RolePermissions rp ON uor.RoleId = rp.RoleId
                    JOIN Permissions p ON p.Id = rp.PermissionId AND p.Id IN @PermIds
                    WHERE UserId = @UserId AND OrgId = @OrgId",
                    new { UserId = userId, OrgId = orgId, PermIds = perms }
                );

                return res >= perms.Count();
            }
        }

        public async Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId)
        {
            var res = (from uor in _applicationContext.Set<UserOrganizationRole>()
                       where uor.UserId == userId && uor.OrgId == orgId
                       join rp in _applicationContext.Set<RolePermission>()
                         on uor.RoleId equals rp.RoleId
                       select rp.PermissionId).Distinct();
            return await res.Select(r => Enum.Parse<PermissionEnum>(r)).ToListAsync();
        }

    }
}