using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Core.Authorization;
using CoreMultiTenancy.Identity.Interfaces;

namespace CoreMultiTenancy.Identity.Authorization
{
    public class RemoteAuthorizationEvaluator : IRemoteAuthorizationEvaluator
    {
        private readonly IOrganizationManager _orgManager;
        private readonly IPermissionService _permSvc;

        public RemoteAuthorizationEvaluator(IOrganizationManager orgManager,
            IPermissionService permSvc)
        {
            _orgManager = orgManager ?? throw new ArgumentNullException(nameof(orgManager));
            _permSvc = permSvc ?? throw new ArgumentNullException(nameof(permSvc));
        }
        public async Task<AuthorizeDecision> EvaluateAsync(string userId, string orgId, params string[] perms)
        {
            var parsedPerms = new List<PermissionEnum>();
            foreach (string s in perms)
            {
                if (Enum.TryParse<PermissionEnum>(s, out var p))
                    parsedPerms.Add(p);
                else
                    return new AuthorizeDecision()
                    {
                        Allowed = false,
                        FailureReason = failureReason.Permissionformat,
                        FailureMessage = $"Unable to parse {s} to PermissionEnum."
                    };
            }

            Guid userIdGuid = new Guid(userId);
            Guid orgIdGuid = new Guid(orgId);
            // Check if Organization exists so a 404 can be returned on request for non-existent org
            if (!await _orgManager.ExistsAsync(orgIdGuid))
                return new AuthorizeDecision() { Allowed = false, FailureReason = failureReason.Tenantnotfound };
            // If no permissions specified, simply check whether the User has access
            // If permissions specified, ensure user has all and access in one query
            if (parsedPerms.Count == 0 && await _orgManager.UserHasAccessAsync(userIdGuid, orgIdGuid))
                return Ok();
            else if (await _permSvc.UserHasPermissionsAsync(userIdGuid, orgIdGuid, parsedPerms))
                return Ok();
            return UnAuthorized();
        }
        
        private AuthorizeDecision UnAuthorized()
            => new AuthorizeDecision() { Allowed = false, FailureReason = failureReason.Unauthorized };
        private AuthorizeDecision Ok()
            => new AuthorizeDecision() { Allowed = true };
    }
}