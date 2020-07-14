using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class OrganizationAccessTokenRepository : IOrganizationAccessTokenRepository
    {
        private readonly ILogger<OrganizationAccessTokenRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public OrganizationAccessTokenRepository(ILogger<OrganizationAccessTokenRepository> logger,
            ApplicationDbContext applicationContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }
        public async Task AddTokenAsync(OrganizationAccessToken token)
        {
            _logger.LogInformation($"Attempting to save new OrganizationAccessToken:{token.Value} for Organization:{token.OrganizationId}");
            await _applicationContext.AddAsync(token);
            await _applicationContext.SaveChangesAsync();
            _logger.LogInformation($"Successfully saved new OrganizationAccessToken:{token.Value} for Organization:{token.OrganizationId}");
        }
        public async Task DeleteTokenAsync(OrganizationAccessToken token)
        {
            _applicationContext.Remove(token);
            await _applicationContext.SaveChangesAsync();
        }
        public async Task<OrganizationAccessToken> GetTokenByValueAsync(string value)
        {
            return await _applicationContext.Set<OrganizationAccessToken>()
                .Where(t => t.Value == value)
                .SingleOrDefaultAsync();
        }
    }
}