using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permRepo;
        public PermissionService(IPermissionRepository permRepo)
        {
            _permRepo = permRepo ?? throw new ArgumentNullException(nameof(permRepo));
        }

        public async Task<List<PermissionEnum>> GetUsersPermissionsAsync(Guid userId, Guid orgId)
            => await _permRepo.GetUsersPermissionsAsync(userId, orgId);

        public async Task<List<Permission>> GetAllPermissionsAsync() =>
            await _permRepo.GetAllPermissionsAsync();

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params string[] perms)
            => await _permRepo.UserHasPermissionsAsync(userId, orgId, perms);

        public async Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync()
            => await _permRepo.GetAllPermissionCategoriesAsync();

    }
}