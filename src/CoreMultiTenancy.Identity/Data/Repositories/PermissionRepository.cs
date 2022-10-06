using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Core.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }
        public PermissionRepository(
            IConfiguration config,
            ApplicationDbContext applicationContext)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

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

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _applicationContext.Set<Permission>()
                .Where(p => !p.IsObsolete)
                .Include(p => p.PermCategory)
                .ToListAsync();
        }

        public async Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync()
        {
            return await _applicationContext.Set<PermissionCategory>()
                .Include(pc => pc.Permissions)
                .ToListAsync();
        }
    }
}