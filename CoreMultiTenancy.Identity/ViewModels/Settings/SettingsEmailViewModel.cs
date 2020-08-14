using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using CoreMultiTenancy.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace CoreMultiTenancy.Identity.ViewModels.Settings
{
    [AutoMap(typeof(User))]
    public class SettingsEmailViewModel
    {
        [SourceMember(nameof(User.Email))]
        [Display(Name = "Current Email")]
        public string CurrentEmail { get; set; }
        [Required]
        [EmailAddress]
        [IgnoreMap]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }
    }
}