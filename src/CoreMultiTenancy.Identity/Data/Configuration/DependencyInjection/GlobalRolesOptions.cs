using CoreMultiTenancy.Identity.Entities;

namespace CoreMultiTenancy.Identity.Data.Configuration.DependencyInjection
{
    public class GlobalRolesOptions
    {
        private List<GlobalRoleBuilder> _builders = new List<GlobalRoleBuilder>();
        public List<Role> Roles => _builders.Select(b => b.Build()).ToList();
        public void AddGlobalRole(Action<GlobalRoleBuilder> ba)
        {
            var b = new GlobalRoleBuilder();
            ba?.Invoke(b);
            _builders.Add(b);
        }
    }
}