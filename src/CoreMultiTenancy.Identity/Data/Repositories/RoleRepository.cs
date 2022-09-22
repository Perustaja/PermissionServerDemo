using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Core.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<RoleRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }

        public RoleRepository(IConfiguration config,
            ILogger<RoleRepository> logger, ApplicationDbContext context)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Role>> GetRolesOfOrgAsync(Guid orgId)
        {
            return await _applicationContext.Set<Role>()
                .Where(r => r.OrgId == orgId || r.IsGlobal)
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ThenInclude(p => p.PermCategory)
                .ToListAsync();
        }

        public async Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId)
        {
            var res = await _applicationContext.Set<Role>()
                .Where(r => r.OrgId == orgId && r.Id == roleId)
                .FirstOrDefaultAsync();
            return res != null
                ? Option<Role>.Some(res)
                : Option<Role>.None;
        }

        public Role AddRoleToOrg(Guid orgId, Role role)
            => _applicationContext.Set<Role>().Add(role).Entity;

        public Role UpdateRoleOfOrg(Role role)
            => _applicationContext.Set<Role>().Update(role).Entity;

        public void DeleteRoleOfOrg(Role role)
            => _applicationContext.Remove(role);

        public async Task<bool> RoleIsOnlyRoleForAnyUserAsync(Role role)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                int c = await conn.QueryFirstOrDefaultAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizationRoles
                    WHERE OrgId = @OrgId
                    GROUP BY UserId
                    HAVING COUNT(*) = 1 AND RoleId = @RoleId",
                    new { RoleId = role.Id, OrgId = role.OrgId }
                );
                return c > 0;
            }
        }
    }
}