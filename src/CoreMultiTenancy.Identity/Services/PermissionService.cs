using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Data.Repositories;
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

        public Task<Option<Error>> SetUsersPermissionsAsync(Guid userId, Guid orgId, Guid roleId)
        {
            // TODO
            throw new NotImplementedException();
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm)
            => await _permRepo.UserHasPermissionAsync(userId, orgId, perm);

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms)
            => await _permRepo.UserHasPermissionsAsync(userId, orgId, perms);
    }
}