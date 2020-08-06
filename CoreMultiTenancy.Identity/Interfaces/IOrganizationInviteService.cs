using System;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IOrganizationInviteService
    {
        /// <summary>
        /// Returns a permanent invitation link based on the id passed.
        /// <summary>
        string CreatePermInvitationLink(Guid orgId);
        /// <summary>
        /// Attempts to use the activation link and grant the User access to the corresponding
        /// Organization.
        /// <summary>
        Task<InviteResult> UsePermInvitationLink(User user, string link);
    }
}