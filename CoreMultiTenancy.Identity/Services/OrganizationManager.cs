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
using Microsoft.EntityFrameworkCore;
using CoreMultiTenancy.Identity.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly ILogger<OrganizationManager> _logger;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IOrganizationInviteService _inviteSvc;

        public OrganizationManager(ILogger<OrganizationManager> logger,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationRepository orgRepo,
            IRoleRepository roleRepo,
            IOrganizationInviteService inviteSvc)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
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
            => await _userOrgRepo.UpdateAsync(uo);
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
        /// <summary>
        /// Returns a code that can be reused to grant a user access to the organization.
        /// </summary>
        public async Task<string> CreatePermanentInvitationLinkAsync(Organization org)
            => await _inviteSvc.CreatePermanentInviteLinkAsync(org.Id);

        /// <summary>
        /// Uses the specified permanent invitation code to attempt to grant the user access to the organization.
        /// </summary>
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

        }

        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, List<PermissionEnum> perms)
        {

        }
        #endregion

        private async Task<InviteResult> GrantAccessAsync(User user, Organization org)
        {
            var record = await _userOrgRepo.GetByIdsAsync(user.Id, org.Id);
            // Check existing record to see its status
            if (record.IsSome())
                return InviteResult.FromExistingAccess(record.Unwrap(), org.Title);

            // Attempt to save new record
            var accessGrant = new UserOrganization(user.Id, org.Id);
            var result = await _userOrgRepo.AddAsync(accessGrant);
            if (result.IsSome())
            {
                return org.RequiresConfirmation
                    ? InviteResult.RequiresConfirmation(org.Title)
                    : InviteResult.ImmediateSuccess(org.Title);
            }
            return InviteResult.LinkInvalid();
        }
    }
}