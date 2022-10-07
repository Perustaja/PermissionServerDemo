using AutoMapper;
using PermissionServerDemo.Identity.Entities;
using PermissionServerDemo.Identity.Entities.Dtos;

namespace PermissionServerDemo.Identity.Mapping
{
    public class RolePermissionConverter : ITypeConverter<RolePermission, PermissionGetDto>
    {
        public PermissionGetDto Convert(RolePermission source, PermissionGetDto destination, ResolutionContext context)
            => context.Mapper.Map<Permission, PermissionGetDto>(source.Permission);
    }
}