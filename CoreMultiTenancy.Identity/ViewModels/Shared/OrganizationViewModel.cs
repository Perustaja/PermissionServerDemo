using System;
using AutoMapper;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.ViewModels.Shared
{
    [AutoMap(typeof(Organization))]
    public class OrganizationViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }
}