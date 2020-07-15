using System;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Interfaces
{
    public interface IOrganizationInviteCodeService
    {
        /// <summary>
        /// Returns the invitation code for the Organization based on the given id.
        /// </summary>
        string GetInviteCode(Guid orgId);
        /// <summary>
        /// Returns an InviteDecodeResult which contains either an error message or the Organization
        /// that the given code belongs to.
        /// </summary>
        InviteDecodeResult DecodeInvitation(string code);
    }
}