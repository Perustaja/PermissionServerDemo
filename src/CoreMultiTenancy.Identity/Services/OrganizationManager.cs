using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results;
using Perustaja.Polyglot.Option;
using CoreMultiTenancy.Identity.Results.Errors;
using Microsoft.AspNetCore.Identity;
using CoreMultiTenancy.Core.Authorization;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationManager : IOrganizationManager
    {
        private readonly string _connectionString;
        private readonly UserManager<User> _userManager;
        private readonly IUserOrganizationRepository _userOrgRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRolePermissionRepository _rolePermissionRepo;
        private readonly IUserOrganizationRoleRepository _userOrgRoleRepo;
        private readonly IOrganizationInviteService _inviteSvc;

        public OrganizationManager(IConfiguration config,
            UserManager<User> userManager,
            IUserOrganizationRepository userOrgRepo,
            IOrganizationRepository orgRepo,
            IRoleRepository roleRepo,
            IRolePermissionRepository rolePermRepo,
            IUserOrganizationRoleRepository userOrgRoleRepo,
            IOrganizationInviteService inviteSvc)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userOrgRepo = userOrgRepo ?? throw new ArgumentNullException(nameof(userOrgRepo));
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _roleRepo = roleRepo ?? throw new ArgumentNullException(nameof(roleRepo));
            _rolePermissionRepo = rolePermRepo ?? throw new ArgumentNullException(nameof(rolePermRepo));
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
            _userOrgRoleRepo.Update(uors);
            await _userOrgRepo.UnitOfWork.Commit();
            return Option<Error>.None;
        }
        #endregion

        #region RoleManagement
        public async Task<List<Role>> GetRolesOfOrgAsync(Guid orgId)
            => await _roleRepo.GetRolesOfOrgAsync(orgId);

        public async Task<Option<Role>> GetRoleOfOrgByIdsAsync(Guid orgId, Guid roleId)
            => await _roleRepo.GetRoleOfOrgByIdsAsync(orgId, roleId);

        public async Task<Role> AddRoleToOrgAsync(Guid orgId, Role role, List<PermissionEnum> perms)
        {
            role.SetOrganization(orgId);
            role = _roleRepo.Add(orgId, role);
            var rps = perms.Select(p => new RolePermission(role.Id, p)).ToArray();
            _rolePermissionRepo.Add(rps);
            await _roleRepo.UnitOfWork.Commit();
            return role;
        }

        public async Task UpdateRoleOfOrgAsync(Guid orgId, Role role)
        {
            _roleRepo.Update(role);
            await _roleRepo.UnitOfWork.Commit();
        }

        public async Task<Option<Error>> DeleteRoleOfOrgAsync(Role role)
        {
            // verify role is not global or only role for any user before deletion
            if (role.IsGlobal)
                return Option<Error>.Some(new Error("Cannot delete a global role.", ErrorType.DomainLogic));
            if (await _userOrgRoleRepo.RoleIsOnlyRoleForAnyUserAsync(role))
                return Option<Error>.Some(new Error("This Role cannot be deleted because it is the last Role for at least one User.", ErrorType.DomainLogic));
            _roleRepo.Delete(role);
            // TODO: setting up a cascade delete here isn't super simple, so ideally 
            // all UserOrganizationRoles need to be deleted here with this role id so no users have 
            // this role. This is trivial but not necessary for this demo so it is left omitted.
            await _roleRepo.UnitOfWork.Commit();
            return Option<Error>.None;
        }

        public async Task<Option<Error>> RemoveRoleFromUserAsync(Guid userId, Guid orgId, Guid roleId)
        {
            var uors = await _userOrgRoleRepo.GetUsersRolesAsync(userId, orgId);
            var roleToRemove = uors.FirstOrDefault(r => r.RoleId == roleId);
            if (roleToRemove != null)
                if (uors.Count > 1)
                {
                    _userOrgRoleRepo.Delete(roleToRemove);
                    await _userOrgRoleRepo.UnitOfWork.Commit();
                    return Option<Error>.None;
                }
                else
                    return Option<Error>.Some(new Error($"Role {roleId} is this user's last role and cannot be deleted.", ErrorType.DomainLogic));
            else
                return Option<Error>.Some(new Error("User does not have this role within this tenant.", ErrorType.NotFound));
        }

        public async Task<List<UserOrganization>> GetUserOrganizationsByUserIdAsync(Guid userId)
            => await _userOrgRepo.GetByUserIdAsync(userId);

        public async Task<Option<Error>> RevokeAccessAsync(Guid userId, Guid orgId)
        {
            var userOrgsOpt = await _userOrgRepo.GetByIdsAsync(orgId, userId);
            if (userOrgsOpt.IsSome())
            {
                var uo = userOrgsOpt.Unwrap();
                if (uo.Organization.OwnerUserId != userId)
                {
                    _userOrgRepo.Delete(uo);
                    await _userOrgRepo.UnitOfWork.Commit();
                    return Option<Error>.None;         
                }
                else
                    return Option<Error>.Some(new Error($"User {userId} cannot have access revoked as it is the owner of the organization.", ErrorType.DomainLogic));

            }
            return Option<Error>.Some(new Error($"User {userId} does not have access to organization {orgId}", ErrorType.NotFound));
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

        private async void AddDefaultLowestUserRoleToUserAsync(Guid userId, Guid orgId)
        {
            var defaultRole = await _roleRepo.GetGlobalDefaultNewUserRoleAsync();
            _userOrgRoleRepo.Add(new UserOrganizationRole(userId, orgId, defaultRole.Id));
        }

        private async void AddDefaultAdminRoleToUserAsync(Guid userId, Guid orgId)
        {
            var defaultRole = await _roleRepo.GetGlobalDefaultOwnerRoleAsync();
            _userOrgRoleRepo.Add(new UserOrganizationRole(userId, orgId, defaultRole.Id));
        }
    }
}