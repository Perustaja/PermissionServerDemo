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
        /// <summary>
        /// Returns an organization associated with the given id.
        /// </summary>
        public async Task<Option<Organization>> GetByIdAsync(Guid orgId);

        /// <summary>
        /// Returns all organizations.
        /// </summary>
        public async Task<List<Organization>> GetAllAsync();

        /// <summary>
        /// Updates the given tenant.
        /// </summary>
        /// <returns>The updated organization.</returns>
        public async Task<Option<Organization>> UpdateAsync(Organization org);

        /// <summary>
        /// Returns the role with its permissions navigation property populated.
        /// </summary>
        public async Task<Option<Role>> GetRoleWithPermsByIdAsync(Guid orgId);

        /// <summary>
        /// Returns all roles that the accompanied organization has, including global roles.
        /// If list is empty, it can be assumed that the organization does not exist.
        /// </summary>
        public async Task<List<Role>> GetAllRolesAsync(Guid orgId);

        /// <summary>
        /// Adds a role to be used by the specified organization.
        /// </summary>
        /// <returns>The added role.</returns>
        public async Task<Option<Role>> AddRoleAsync(Role role);

        /// <summary>
        /// Adds a role to be used by the specified organization.
        /// </summary>
        /// <returns>The updated role.</returns>
        public async Task<Option<Role>> UpdateRoleAsync(Role role);

        /// <summary>
        /// Attempts to delete the role, removing all traces of it. This will fail if any user has
        /// this role as their only role or if the role is global.
        /// </summary>
        public async Task<Option<Error>> DeleteRoleAsync(Role role);

        /// <summary>
        /// Returns a list of all organizations that the specified user has access to, or null.
        /// </summary>
        public async Task<List<Organization>> GetUsersOrgsAsync(Guid userId);

        /// <summary>
        /// Returns a list of users with their roles.
        /// </summary>
        public async Task<List<User>> GetUsersWithRolesAsync(Guid userId);

        /// <summary>
        /// Returns all users awaiting approval.
        /// </summary>
        public async Task<List<User>> GetUsersAwaitingApprovalAsync(Guid orgId);

        /// <summary>
        /// Returns all users awaiting approval.
        /// </summary>
        public async Task<List<User>> GetBlacklistedUsersAsync(Guid orgId);

        public async Task<string> CreatePermanentInvitationLinkAsync(Guid orgId)
            => await _inviteSvc.CreatePermanentInviteLinkAsync(orgId);

        public async Task<InviteResult> UsePermanentInvitationAsync(User user, string link)
        {
            if (!await _inviteSvc.TryDecodePermanentInviteLinkAsync(link, out var guid))
                return InviteResult.LinkInvalid();

            var org = await _orgRepo.GetByIdAsync(guid);
            if (org != null)
            {
                // link corresponds to a valid Organization, change access and return result
                var res = await GrantAccessAsync(user, org);
                if (res.Success)
                    return res.RequiresConfirmation ? InviteResult.RequiresConfirmation(org.Title) : InviteResult.ImmediateSuccess(org.Title);
                else if (res.UserBlacklisted)
                    return InviteResult.Blacklisted();
                else if (res.ExistingAccess)
                    return InviteResult.ExistingAccess(org.Title);
            }
            // No Organization found or some miscellaneous failure occurred
            return InviteResult.LinkInvalid();
        }

        public async Task<AccessModifiedResult> GrantAccessAsync(Organization org, params User[] users)
        {
            var record = await _userOrgRepo.GetByIdsAsync(user.Id, org.Id);
            // Check existing record to see its status
            if (record != null)
            {
                if (record.Blacklisted)
                    return new AccessModifiedResult() { UserBlacklisted = true };
                else if (record.AwaitingApproval)
                    return new AccessModifiedResult() { AwaitingConfirmation = true };
                else
                    return new AccessModifiedResult() { ExistingAccess = true };
            }

            // Attempt to save new record
            var accessGrant = new UserOrganization(user.Id, org.Id);
            try
            {
                await _userOrgRepo.AddAsync(accessGrant);
            }
            catch
            {
                _logger.LogError($"Exception while trying to create new UserOrganization. User: {accessGrant.UserId} Organization: {accessGrant.OrganizationId}.");
                var errorResult = new AccessModifiedResult() { ErrorMessage = "An unexpected error has occurred. If the issue persists, contact site administration." };
                return errorResult; // Return an empty result indicating complete failure
            }
            return AccessModifiedResult.SuccessfulResult(org.RequiresConfirmation);
        }

        public async Task<Option<Error>> RevokeAccessAsync(Guid userId, Guid orgId)
        {
            var uo = await _userOrgRepo.GetByIdsAsync(userId, orgId);
            if (uo.IsSome())
            {
                await _userOrgRepo.DeleteAsync(uo.Unwrap());                
                return Option<Error>.None();
            }
            return Option<Error>.Some(new Error($"User {userId} or Org {orgId} doesn't exist.", ErrorType.NotFound));
        }

        public async Task<bool> UserHasAccessAsync(Guid userId, Guid orgId)
            => await _userOrgRepo.ExistsWithAccessAsync(userId, orgId);

        /// <summary>
        /// Returns whether the user has permission within the scope of the organization.
        /// Returns false if either doesn't exist or the user does not have access.
        /// </summary>
        public async Task<bool> UserHasPermissionsAsync(Guid userId, Guid orgId, params PermissionEnum[] perms);
    }
}