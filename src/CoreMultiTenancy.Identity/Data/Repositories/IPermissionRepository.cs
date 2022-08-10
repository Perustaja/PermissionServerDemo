using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public interface IPermissionRepository
    {
        Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm);
        Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms);
    }
}