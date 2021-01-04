using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Perustaja.Polyglot.Option;
using CoreMultiTenancy.Identity.Results.Errors;
using CoreMultiTenancy.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using CoreMultiTenancy.Identity.Extensions;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly string _connectionString;
        private readonly Guid _defaultRoleId;
        private readonly ILogger<OrganizationManager> _logger;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserOrganizationRoleRepository _userOrgRoleRepo;
        private readonly IOrganizationInviteService _inviteSvc;

        public OrganizationManager(IConfiguration config,
            ILogger<OrganizationManager> logger,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationRepository orgRepo,
            IRoleRepository roleRepo,
            IUserOrganizationRoleRepository userOrgRoleRepo,
            IOrganizationInviteService inviteSvc)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _defaultRoleId = config.GetDefaultRoleId();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _userOrgRoleRepo = userOrgRoleRepo ?? throw new ArgumentNullException(nameof(userOrgRoleRepo));
            _inviteSvc = inviteSvc ?? throw new ArgumentNullException(nameof(inviteSvc));
        }

        #region OrganizationManagement
        public async Task<Option<Organization>> GetByIdAsync(Guid orgId)
            => await _orgRepo.GetByIdAsync(orgId);

        public async Task<Option<Organization>> AddAsync(Organization o)
            => await _orgRepo.AddAsync(o);


        public async Task<Option<Organization>> UpdateAsync(Organization o)
            => await _orgRepo.UpdateAsync(o);
        #endregion

        #region  UserManagement
        public async Task<List<UserOrganization>> GetUsersOfOrgAsync(Guid orgId)
            => await _userOrgRepo.GetAllByOrgId(orgId);

        public async Task<List<UserOrganization>> GetUsersOfOrgAwaitingApprovalAsync(Guid orgId)
            => await _userOrgRepo.GetAwaitingAccessByOrgId(orgId);

        public async Task<Option<UserOrganization>> GetUserOfOrgByIdsAsync(Guid orgId, Guid userId)
            => await _userOrgRepo.GetByIdsAsync(orgId, userId);

        public async Task UpdateUserOfOrgAsync(UserOrganization uo)
        {
            // Save changes to the UserOrganizationRepository and UserOrganizationRoles
            await _userOrgRepo.UpdateAsync(uo);
            await _userOrgRoleRepo.UpdateBulkAsync(uo.User.UserOrganizationRoles);
        }
        #endregion

        #region RoleManagement
        public async Task<List<Role>> GetRolesOfOrgAsync(Guid orgId)
            => await _roleRepo.GetRolesOfOrgAsync(orgId);

        public async Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId)
            => await _roleRepo.GetRoleOfOrgByIdsAsync(orgId, roleId);

        public async Task<Option<Role>> AddRoleToOrgAsync(Guid orgId, Role role)
            => await _roleRepo.AddRoleToOrgAsync(orgId, role);

        public async Task UpdateRoleOfOrgAsync(Guid orgId, Role role)
            => await _roleRepo.UpdateRoleOfOrgAsync(role);

        public async Task<Option<Error>> DeleteRoleOfOrgAsync(Role role)
            => await _roleRepo.DeleteRoleOfOrgAsync(role);
        #endregion

        #region InvitationManagement
        public async Task<string> CreatePermanentInvitationLinkAsync(Organization org)
            => await _inviteSvc.CreatePermanentInviteLinkAsync(org.Id);

        public async Task<InviteResult> UsePermanentInvitationAsync(User user, string link)
        {
            if (!await _inviteSvc.TryDecodePermanentInviteLinkAsync(link, out var guid))
            {
                // org exists
                var orgResult = await _orgRepo.GetByIdAsync(guid);
                if (orgResult.IsSome())
                    return await GrantAccessAsync(user, orgResult.Unwrap());
            }
            return InviteResult.LinkInvalid();
        }
        #endregion

        #region PermissionAndAccessChecks
        public async Task<bool> UserHasAccessAsync(Guid userId, Guid orgId)
            => await _userOrgRepo.ExistsWithAccessAsync(userId, orgId);

        public async Task<bool> UserHasPermissionAsync(Guid userId, Guid orgId, PermissionEnum perm)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleOrDefaultAsync(
                    @"SELECT COUNT(*) 
                    FROM UserOrganizationRoles uor
                    WHERE UserId = @UserId AND OrgId = @OrgId
                    JOIN RolePermissions rp ON uor.RoleId = rp.RoleId
                    JOIN Permissions p ON p.Id = rp.PermissionId AND p.Id = @PermId",
                    new { UserId = userId, OrgId = orgId, PermId = perm }
                );
                return res > 0;
            }
        }

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleOrDefaultAsync(
                    @"SELECT COUNT(*) 
                    FROM UserOrganizationRoles uor
                    WHERE UserId = @UserId AND OrgId = @OrgId
                    JOIN RolePermissions rp ON uor.RoleId = rp.RoleId
                    JOIN Permissions p ON p.Id = rp.PermissionId AND p.Id IN @PermIds",
                    new { UserId = userId, OrgId = orgId, PermIds = perms }
                );
                return res > 0;
            }
        }
        #endregion

        private async Task<InviteResult> GrantAccessAsync(User user, Organization org)
        {
            var record = await _userOrgRepo.GetByIdsAsync(user.Id, org.Id);
            // Check existing record to see its status
            if (record.IsSome())
                return InviteResult.FromExistingAccess(record.Unwrap(), org.Title);

            // Save new record
            var accessGrant = new UserOrganization(user.Id, org.Id);
            var accessResult = await _userOrgRepo.AddAsync(accessGrant);
            var defaultRoleResult = await AddDefaultRoleToUserAsync(user.Id, org.Id);
            if (accessResult.IsNone() && defaultRoleResult.IsNone())
            {
                return org.RequiresConfirmation
                    ? InviteResult.RequiresConfirmation(org.Title)
                    : InviteResult.ImmediateSuccess(org.Title);
            }
            return InviteResult.LinkInvalid();
        }

        private async Task<Option<Error>> AddDefaultRoleToUserAsync(Guid userId, Guid orgId)
        {
            var defaultRole = new UserOrganizationRole(userId, orgId, _defaultRoleId);
            return await _userOrgRoleRepo.AddAsync(defaultRole);
        }
    }
}