using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using CoreMultiTenancy.Identity.ViewModels.Shared;

namespace CoreMultiTenancy.Identity.ViewModels.Portal
{
    public class SelectOrganizationViewModel
    {
        public List<OrganizationViewModel> OrganizationViewModels { get; set; }
        public string ReturnUrl { get; set; }
    }
}