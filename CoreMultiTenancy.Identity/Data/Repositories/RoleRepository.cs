using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;
        private ApplicationDbContext _applicationContext;

        public RoleRepository(ILogger<RoleRepository> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Role>> GetAllByOrgIdAsync(Guid orgId)
        {
            return await _applicationContext.Set<Role>()
                .Where(r => r.IsGlobal || r.OrgId == orgId)
                .ToListAsync();
        }

        public async Task<Option<Role>> GetByIdAsync(Guid roleId)
        {
            var res = await _applicationContext.Set<Role>()
                .Where(r => r.Id == roleId)
                .SingleOrDefaultAsync();
            return res != null
                ? Option<Role>.Some(res)
                : Option<Role>.None();
        }

        public async Task<Option<Role>> AddRoleAsync(Role role)
        {
            try
            {
                await _applicationContext.Set<Role>().AddAsync(role);
                await _applicationContext.SaveChangesAsync();
                return Option<Role>.Some(role);
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e.ToString());
                return Option<Role>.None();
            }
        }

        public async Task<Option<Error>> DeleteRoleAsync(Guid roleId)
        {
            var role = await _applicationContext.Set<Role>().SingleOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
                return Option<Error>.Some(new Error($"Role with id {roleId} doesn't exist.", ErrorType.NotFound));
            if (role.IsGlobal)
                return Option<Error>.Some(new Error("Cannot delete global role.", ErrorType.DomainLogic));
            _applicationContext.Remove(role);
            await _applicationContext.SaveChangesAsync();
            return Option<Error>.None();
        }

        public Task<Option<Error>> AddRoleToUserAsync(Guid roleId, Guid userId, Guid orgId)
        {
            // Add userorganizationrole
            // role doesn't exist
            // user doesn't exist
            // org doesn't exist
        }

        /// <summary>
        /// Removes the role associated with the id from the user if both exist.
        /// </summary>
        public Task<Option<Error>> RemoveRoleFromUserAsync(Guid roleId, Guid userId, Guid orgId);

        /// <summary>
        /// Returns a list of permissions that the user has based on the user and organization ids.
        /// </summary>
        public Task<List<Permission>> GetUsersPermissionsAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns a list of roles that the user has based on the user and organization ids.
        /// </summary>
        public Task<List<Role>> GetUsersRolesAsync(Guid userId, Guid orgId);

        /// <summary>
        /// Returns whether the given user has the permission within the scope of the organization.
        /// </summary>
        public Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm);
    }
}