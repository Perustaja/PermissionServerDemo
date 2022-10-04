using CoreMultiTenancy.Core.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Perustaja.Polyglot.Result;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class UserOrganizationRoleRepository : IUserOrganizationRoleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserOrganizationRoleRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }

        public UserOrganizationRoleRepository(IConfiguration config,
            ILogger<UserOrganizationRoleRepository> logger,
            ApplicationDbContext context)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UserOrganizationRole Add(UserOrganizationRole uor)
            => _applicationContext.Set<UserOrganizationRole>().Add(uor).Entity;

        public void Update(List<UserOrganizationRole> uors)
            => _applicationContext.Set<UserOrganizationRole>().UpdateRange(uors);

        public void Delete(UserOrganizationRole uors)
            => _applicationContext.Set<UserOrganizationRole>().Remove(uors);

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

        public async Task<List<UserOrganizationRole>> GetUsersRolesAsync(Guid userId,
            Guid orgId)
        {
            return await _applicationContext.Set<UserOrganizationRole>()
                .Where(uor => uor.UserId == userId && uor.OrgId == orgId)
                .ToListAsync();
        }
    }
}