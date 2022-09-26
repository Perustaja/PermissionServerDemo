using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Results.Errors;
using Perustaja.Polyglot.Option;

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
        {
            var perms = await _permRepo.GetUsersPermissionsAsync(userId, orgId);
            return perms;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync() => 
            await _permRepo.GetAllPermissionsAsync();

        public Task<Option<Error>> SetUsersPermissionsAsync(Guid userId, Guid orgId, Guid roleId)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params PermissionEnum[] perms)
            => await _permRepo.UserHasPermissionsAsync(userId, orgId, perms.ToList());

        public async Task<List<PermissionCategory>> GetAllPermissionCategoriesAsync()
            => await _permRepo.GetAllPermissionCategoriesAsync();

    }
}