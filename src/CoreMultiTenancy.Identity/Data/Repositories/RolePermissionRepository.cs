using CoreMultiTenancy.Core.Interfaces;
using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }

        public RolePermissionRepository(IConfiguration config,
            ILogger<RoleRepository> logger, ApplicationDbContext context)
        {
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void Add(params RolePermission[] rps)
           => _applicationContext.Set<RolePermission>().AddRange(rps);
    }
}