using PermissionServer;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Core.Attributes
{
    public class RemoteAuthorizeAttribute : RemoteAuthorizeAttribute<PermissionEnum>
    {
        public RemoteAuthorizeAttribute(params PermissionEnum[] permissions) : base(permissions) { }
    }
}