using Microsoft.AspNetCore.Http;

namespace CoreMultiTenancy.Identity.Entities.Dtos
{
    public class OrganizationPostDto
    {
        public string Title { get; private set; }
        public IFormFile Logo { get; private set; }
    }
}