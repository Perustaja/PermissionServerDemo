using System;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Interfaces;
using CoreMultiTenancy.Identity.Results;

namespace CoreMultiTenancy.Identity.Services
{
    public class OrganizationInviteCodeService : IOrganizationInviteCodeService
    {
        private readonly IOrganizationRepository _orgRepo;
        public OrganizationInviteCodeService(IOrganizationRepository orgRepo)
        {
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
        }
        public string GetInviteCode(Guid orgId) => Encode(orgId);
        public InviteDecodeResult DecodeInvitation(string code)
        {
            try
            {
                var guid = Decode(code);
                if (_orgRepo.GetByIdAsync(guid) == null)
                    return InviteDecodeResult.Invalid();
                return InviteDecodeResult.Success(guid);
            }
            catch
            {
                return InviteDecodeResult.Invalid();
            }
        }
        private string Encode(Guid id) =>
            Convert.ToBase64String(id.ToByteArray()).Replace("/", "_").Replace("+", "-").Substring(0, 22);

        private Guid Decode(string code)
        {
            var originalCode = code.Replace("_", "/").Replace("-", "+") + "==";
            byte[] buffer = Convert.FromBase64String(originalCode);
            return new Guid(buffer);
        }
    }
}