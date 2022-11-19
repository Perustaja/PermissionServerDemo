using System.ComponentModel.DataAnnotations;

namespace PermissionServerDemo.Api.Entities
{
    public class Aircraft
    {
        [Required]
        public string RegNumber { get; private set; }
        public string Model { get; private set; }
        public string ThumbnailUri { get; private set; }
        public Guid? TenantId { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsShadowOwned { get; private set; }
        public bool IsGlobal { get; private set; }
        public Aircraft() { }
        public Aircraft(string regNum, Guid tenantId, string thumbUri, string model)
        {
            RegNumber = regNum;
            Model = model;
            TenantId = tenantId;
            ThumbnailUri = thumbUri;
            IsGrounded = false;
            IsShadowOwned = false;
            IsGlobal = false;
        }
        public void Ground() => IsGrounded = true;
        public static Aircraft GlobalAircraft(string regNum, string thumbUri, string model, bool isShadow)
        {
            return new Aircraft()
            {
                RegNumber = regNum,
                Model = model,
                TenantId = Guid.Empty,
                ThumbnailUri = thumbUri,
                IsGrounded = false,
                IsShadowOwned = isShadow,
                IsGlobal = true,
            };
        }
    }
}