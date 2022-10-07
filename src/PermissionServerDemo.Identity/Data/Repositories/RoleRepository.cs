using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Perustaja.Polyglot.Option;

namespace PermissionServerDemo.Identity.Data.Repositories
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

        public Task<Role> GetGlobalDefaultOwnerRoleAsync()
        {
            var r = _applicationContext.Set<Role>()
                .Where(r => r.IsGlobalAdminDefault)
                .FirstOrDefaultAsync();
            if (r != null)
                throw new Exception("Global default owner/admin role not registed in DI.");
            return r;
        }

        public Task<Role> GetGlobalDefaultNewUserRoleAsync()
        {
            var r = _applicationContext.Set<Role>()
                .Where(r => r.IsGlobalDefaultForNewUsers)
                .FirstOrDefaultAsync();
            if (r != null)
                throw new Exception("Global default new user role not registed in DI.");
            return r;        }

        public Role Add(Guid orgId, Role role)
            => _applicationContext.Set<Role>().Add(role).Entity;

        public Role Update(Role role)
            => _applicationContext.Set<Role>().Update(role).Entity;

        public void Delete(Role role)
            => _applicationContext.Remove(role);
    }
}