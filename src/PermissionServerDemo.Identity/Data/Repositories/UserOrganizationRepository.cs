using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Core.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Perustaja.Polyglot.Option;

namespace PermissionServerDemo.Identity.Data.Repositories
{
    public class UserOrganizationRepository : IUserOrganizationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserOrganizationRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }
        public UserOrganizationRepository(IConfiguration config,
            ILogger<UserOrganizationRepository> logger,
            ApplicationDbContext applicationContext)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        public async Task<List<UserOrganization>> GetAllByOrgId(Guid orgId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId)
                .Include(uo => uo.User)
                .ThenInclude(u => u.UserOrganizationRoles.Where(uor => uor.OrgId == orgId))
                .ThenInclude(uor => uor.Role)
                .ToListAsync();
        }

        public async Task<List<UserOrganization>> GetAwaitingAccessByOrgId(Guid orgId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId && uo.AwaitingApproval == true)
                .Include(uo => uo.User)
                .ToListAsync();
        }

        public async Task<Option<UserOrganization>> GetByIdsAsync(Guid orgId, Guid userId)
        {
            var res = await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId && uo.UserId == userId)
                .Include(uo => uo.Organization)
                .FirstOrDefaultAsync();
            return res != null
                ? Option<UserOrganization>.Some(res)
                : Option<UserOrganization>.None;
        }

        public async Task<List<UserOrganization>> GetByUserIdAsync(Guid userId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.UserId == userId)
                .Include(uo => uo.Organization)
                .OrderBy(uo => uo.Organization.OwnerUserId != userId)
                .ToListAsync();
        }

        public UserOrganization Add(UserOrganization uo)
            => _applicationContext.Add(uo).Entity;

        public UserOrganization Update(UserOrganization uo)
            => _applicationContext.Set<UserOrganization>().Update(uo).Entity;

        public void Delete(UserOrganization uo)
            => _applicationContext.Set<UserOrganization>().Remove(uo);

        public async Task<bool> ExistsAsync(Guid userId, Guid orgId)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizations
                    WHERE UserId = @UserId
                    AND OrgId = @OrgId",
                    new { UserId = userId, OrgId = orgId }
                );
                return res > 0;
            }
        }
        public async Task<bool> ExistsWithAccessAsync(Guid userId, Guid orgId)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizations
                    WHERE UserId = @UserId
                    AND OrgId = @OrgId
                    AND AwaitingApproval = false
                    AND Blacklisted = false",
                    new { UserId = userId, OrgId = orgId }
                );
                return res > 0;
            }
        }
    }
}