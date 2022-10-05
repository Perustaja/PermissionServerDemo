using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IRolePermissionRepository : IRepository
    {
        void Add(params RolePermission[] rps);
    }
}