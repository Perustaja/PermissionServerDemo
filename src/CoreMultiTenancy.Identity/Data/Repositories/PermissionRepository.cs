using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Core.Interfaces;
using CoreMultiTenancy.Identity.Results.Errors;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }
        public PermissionRepository(IConfiguration config,
            ApplicationDbContext applicationContext)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }
        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.QuerySingleOrDefaultAsync(
                    @"SELECT COUNT(*) 
                    FROM UserOrganizationRoles uor
                    WHERE UserId = @UserId AND OrgId = @OrgId
                    JOIN RolePermissions rp ON uor.RoleId = rp.RoleId
                    JOIN Permissions p ON p.Id = rp.PermissionId AND p.Id = @PermId",
                    new { UserId = userId, OrgId = orgId, PermId = perm }
                );
                return res > 0;
            }
        }

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.QuerySingleOrDefaultAsync(
                    @"SELECT COUNT(*) 
                    FROM UserOrganizationRoles uor
                    WHERE UserId = @UserId AND OrgId = @OrgId
                    JOIN RolePermissions rp ON uor.RoleId = rp.RoleId
                    JOIN Permissions p ON p.Id = rp.PermissionId AND p.Id IN @PermIds",
                    new { UserId = userId, OrgId = orgId, PermIds = perms }
                );
                return res > 0;
            }
        }

        public async Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId)
        {
            var res = from uor in _applicationContext.Set<UserOrganizationRole>()
                      join rp in _applicationContext.Set<RolePermission>()
                        on uor.RoleId equals rp.RoleId
                      select rp.PermissionId;
            return await res.ToListAsync<PermissionEnum>();
        }

        public Task<Option<Error>> SetUsersPermissionsAsync(Guid userId, Guid orgId, Guid roleId)
        {
            throw new NotImplementedException();
        }
    }
}