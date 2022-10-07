using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Interfaces;

namespace PermissionServerDemo.Identity.Data.Repositories
{
    public interface IRolePermissionRepository : IRepository
    {
        void Add(params RolePermission[] rps);
    }
}