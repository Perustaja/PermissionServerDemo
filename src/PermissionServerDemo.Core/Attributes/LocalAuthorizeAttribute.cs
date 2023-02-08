using PermissionServer;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Core.Attributes
{
    public class LocalAuthorizeAttribute : LocalAuthorizeAttribute<PermissionEnum>
    {
        public LocalAuthorizeAttribute(params PermissionEnum[] permissions) : base(permissions) { }
    }
}