using System.ComponentModel.DataAnnotations;
using AutoMapper;
using CoreMultiTenancy.Identity.Models;

namespace CoreMultiTenancy.Identity.ViewModels.Settings
{
    [AutoMap(typeof(User))]
    public class SettingsProfileViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "The {0} cannot be more than {1} characters long.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}