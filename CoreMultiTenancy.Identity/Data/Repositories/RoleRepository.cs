using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Authorization;
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

        public async Task<Option<Role>> UpdateRoleAsync(Role role)
        {
            try
            {
                _applicationContext.Update(role);
                await _applicationContext.SaveChangesAsync();
                return Option<Role>.Some(role);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Option<Role>.None();
            }
        }

        public async Task DeleteRoleAsync(Role role)
        {
            _applicationContext.Remove(role);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<Option<Error>> AddUserOrganizationRoleAsync(UserOrganizationRole uor)
        {
            try
            {
                await _applicationContext.Set<UserOrganizationRole>().AddAsync(uor);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None();
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e.ToString());
                return Option<Error>.Some(new Error(String.Empty, ErrorType.Unspecified));
            }
        }

        public async Task DeleteUserOrganizationRoleAsync(UserOrganizationRole uor)
        {
            _applicationContext.Remove(uor);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<List<Role>> GetUserOrganizationRolesByIdsAsync(Guid userId, Guid orgId)
        {
            // research better ways to do this with LINQ
            return await _applicationContext.Set<UserOrganizationRole>()
                .Where(uor => uor.UserId == userId && uor.OrgId == orgId)
                .Include(uor => uor.Role)
                .Select(uor => uor.Role)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Permission>> GetUsersPermissionsAsync(Guid userId, Guid orgId)
        {
            // this is outrageously inefficient, need to redo with dapper query
            var perms = await _applicationContext.Set<UserOrganizationRole>()
                .Where(uor => uor.UserId == userId && uor.OrgId == orgId)
                .Include(uor => uor.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .SelectMany(uor => uor.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .AsNoTracking().Distinct().ToListAsync();
            return perms.AsReadOnly();
        }

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params PermissionEnum[] perms)
        {
            // also needs to be redone with plain sql, checking if id matches
            var rps =  await _applicationContext.Set<UserOrganizationRole>()
                .Where(uor => uor.UserId == userId && uor.OrgId == orgId)
                .Include(uor => uor.Role)
                .ThenInclude(r => r.RolePermissions)
                .SelectMany(uor => uor.Role.RolePermissions)
                .Select(rp => rp.PermissionId)
                .ToArrayAsync();
            var set = rps.ToHashSet();
            return perms.Any(p => set.Contains(p));
        }
    }
}