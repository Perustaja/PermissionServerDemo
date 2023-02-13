using PermissionServer.Common;
using PermissionServerDemo.Core.Authorization;

namespace PermissionServerDemo.Core.Attributes
{
    public class PermissionAttribute : PermissionDataAttribute<PermissionCategoryEnum>
    {
        public PermissionAttribute(PermissionCategoryEnum cat, string name, string desc) : base(cat, name, desc) { }
    }
}