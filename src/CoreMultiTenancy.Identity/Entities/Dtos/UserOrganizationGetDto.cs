using System;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    [AutoMap(typeof(UserOrganization))]
    public class UserOrganizationGetDto {
        public bool AwaitingApproval { get; set; }
        public bool Blacklisted { get; set; }
        public DateTime DateSubmitted { get; set; }
        
        [SourceMember(nameof(UserOrganization.Organization))]
        public OrganizationGetDto Organization { get; set; }
    }
}