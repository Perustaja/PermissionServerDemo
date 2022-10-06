using AutoMapper;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Entities.Dtos;

namespace CoreMultiTenancy.Identity.Mapping
{
    public class RolePermissionConverter : ITypeConverter<RolePermission, PermissionGetDto>
    {
        public PermissionGetDto Convert(RolePermission source, PermissionGetDto destination, ResolutionContext context)
            => context.Mapper.Map<Permission, PermissionGetDto>(source.Permission);
    }
}