using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using System.Collections.Generic;
using Perustaja.Polyglot.Option;
using CoreMultiTenancy.Identity.Results.Errors;
using Microsoft.Extensions.Configuration;
using CoreMultiTenancy.Identity.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly string _connectionString;
        private readonly Guid _defaultLowestUserRoleId;
        private readonly Guid _defaultAdminRoleId;
        private readonly UserManager<User> _userManager;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserOrganizationRoleRepository _userOrgRoleRepo;
        private readonly IOrganizationInviteService _inviteSvc;

        public OrganizationManager(IConfiguration config,
            UserManager<User> userManager,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationRepository orgRepo,
            IRoleRepository roleRepo,
            IUserOrganizationRoleRepository userOrgRoleRepo,
            IOrganizationInviteService inviteSvc)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _defaultAdminRoleId = config.GetDefaultAdminRoleId();
            _defaultLowestUserRoleId = config.GetDefaultAdminRoleId();
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _userOrgRoleRepo = userOrgRoleRepo ?? throw new ArgumentNullException(nameof(userOrgRoleRepo));
            _inviteSvc = inviteSvc ?? throw new ArgumentNullException(nameof(inviteSvc));
        }

        #region OrganizationManagement
        public async Task<bool> ExistsAsync(Guid orgId)
            => await _orgRepo.ExistsByIdAsync(orgId);

        public async Task<Option<Organization>> GetByIdAsync(Guid orgId)
            => await _orgRepo.GetByIdAsync(orgId);

        public async Task<Option<Error>> AddAsync(Organization o, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Option<Error>.Some(new Error($"Unable to find user with id {userId}", ErrorType.NotFound));

            _orgRepo.Add(o);
            var userGuid = new Guid(userId);
            _userOrgRepo.Add(new UserOrganization(userGuid, o.Id));
            AddDefaultAdminRoleToUserAsync(userGuid, o.Id);
            await _orgRepo.UnitOfWork.Commit();

            return Option<Error>.None;
        }

        public async Task<Organization> UpdateAsync(Organization o)
        {
            o = _orgRepo.Update(o);
            await _orgRepo.UnitOfWork.Commit();
            return o;
        }
        #endregion

        #region  UserManagement
        public async Task<List<UserOrganization>> GetUsersOfOrgAsync(Guid orgId)
            => await _userOrgRepo.GetAllByOrgId(orgId);

        public async Task<List<UserOrganization>> GetUsersOfOrgAwaitingApprovalAsync(Guid orgId)
            => await _userOrgRepo.GetAwaitingAccessByOrgId(orgId);

        public async Task<Option<UserOrganization>> GetUserOfOrgByIdsAsync(Guid orgId, Guid userId)
            => await _userOrgRepo.GetByIdsAsync(orgId, userId);

        public async Task<Option<Error>> UpdateUserOfOrgAsync(UserOrganization uo, List<UserOrganizationRole> uors)
        {
            // Verify not empty, should be performed at an earlier point but this is a final check
            if (uors.Count == 0)
                return Option<Error>.Some(new Error("A User must have at least one Role.", ErrorType.DomainLogic));
            // Roles passed must have existing ids in database
            if (uors.Any(uor => (uor.Role == null) || (!uor.Role.IsGlobal && uor.Role.OrgId != uo.OrgId)))
                return Option<Error>.Some(new Error("One of the passed Roles was not valid for this Organization.", ErrorType.DomainLogic));

            _userOrgRepo.Update(uo);
            _userOrgRoleRepo.UpdateBulk(uors);
            await _userOrgRepo.UnitOfWork.Commit();
            return Option<Error>.None;
        }
        #endregion

        #region RoleManagement
        public async Task<List<Role>> GetRolesOfOrgAsync(Guid orgId)
            => await _roleRepo.GetRolesOfOrgAsync(orgId);

        public async Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId)
            => await _roleRepo.GetRoleOfOrgByIdsAsync(orgId, roleId);

        public async Task<Role> AddRoleToOrgAsync(Guid orgId, Role role)
        {
            role.SetOrganization(orgId); // Assign OrgId to Role
            role = _roleRepo.AddRoleToOrg(orgId, role);
            await _roleRepo.UnitOfWork.Commit();
            return role;
        }

        public async Task UpdateRoleOfOrgAsync(Guid orgId, Role role)
        {
            _roleRepo.UpdateRoleOfOrg(role);
            await _roleRepo.UnitOfWork.Commit();
        }

        public async Task<Option<Error>> DeleteRoleOfOrgAsync(Role role)
        {
            // verify role is not global or only role for any user before deletion
            if (role.IsGlobal)
                return Option<Error>.Some(new Error("Cannot delete a global role.", ErrorType.DomainLogic));
            if (await _roleRepo.RoleIsOnlyRoleForAnyUserAsync(role))
                return Option<Error>.Some(new Error("This Role cannot be deleted because it is the last Role for at least one User.", ErrorType.DomainLogic));
            _roleRepo.DeleteRoleOfOrg(role);
            await _roleRepo.UnitOfWork.Commit();
            return Option<Error>.None;
        }

        public async Task<List<UserOrganization>> GetUserOrganizationsByUserIdAsync(Guid userId) {
            return await _userOrgRepo.GetByUserIdAsync(userId);
        }
        #endregion

        #region InvitationManagement
        public async Task<string> CreatePermanentInvitationLinkAsync(Organization org)
            => await _inviteSvc.CreatePermanentInviteLinkAsync(org.Id);

        public async Task<InviteResult> UsePermanentInvitationAsync(User user, string link)
        {
            if (await _inviteSvc.TryDecodePermanentInviteLinkAsync(link, out var guid))
            {
                // org exists
                var orgResult = await _orgRepo.GetByIdAsync(guid);
                if (orgResult.IsSome())
                    return await GrantAccessAsync(user, orgResult.Unwrap());
            }
            return InviteResult.LinkInvalid();
        }

        public async Task<bool> UserHasAccessAsync(Guid userId, Guid orgId)
            => await _userOrgRepo.ExistsWithAccessAsync(userId, orgId);

        #endregion

        private async Task<InviteResult> GrantAccessAsync(User user, Organization org)
        {
            var record = await _userOrgRepo.GetByIdsAsync(user.Id, org.Id);
            // Check existing record to see its status if one exists
            if (record.IsSome())
                return InviteResult.FromExistingAccess(record.Unwrap(), org.Title);

            // Save new record representing User's access to Organization, assign default role
            var accessGrant = new UserOrganization(user.Id, org.Id);
            _userOrgRepo.Add(accessGrant);
            AddDefaultLowestUserRoleToUserAsync(user.Id, org.Id);
            await _userOrgRepo.UnitOfWork.Commit();
            return org.RequiresConfirmationForNewUsers
                ? InviteResult.RequiresConfirmation(org.Title)
                : InviteResult.ImmediateSuccess(org.Title);
        }

        private void AddDefaultLowestUserRoleToUserAsync(Guid userId, Guid orgId)
        {
            var defaultRole = new UserOrganizationRole(userId, orgId, _defaultLowestUserRoleId);
            _userOrgRoleRepo.Add(defaultRole);
        }

        private void AddDefaultAdminRoleToUserAsync(Guid userId, Guid orgId)
        {
            var defaultRole = new UserOrganizationRole(userId, orgId, _defaultAdminRoleId);
            _userOrgRoleRepo.Add(defaultRole);
        }
    }
}