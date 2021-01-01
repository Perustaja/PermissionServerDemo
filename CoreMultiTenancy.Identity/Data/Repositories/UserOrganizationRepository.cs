using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Results.Errors;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class UserOrganizationRepository : IUserOrganizationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UserOrganizationRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public UserOrganizationRepository(IConfiguration config,
            ILogger<UserOrganizationRepository> logger,
            ApplicationDbContext applicationContext)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }
        public async Task<Option<Error>> AddAsync(UserOrganization uo)
        {
            try
            {
                await _applicationContext.Set<UserOrganization>().AddAsync(uo);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None();
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e.ToString());
                return Option<Error>.Some(new Error(String.Empty, ErrorType.Unspecified));
            }
        }

        public async Task DeleteAsync(UserOrganization uo)
        {
            _applicationContext.Set<UserOrganization>().Remove(uo);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid orgId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizations
                    WHERE UserId = @UserId
                    AND OrganizationId = @OrgId",
                    new { UserId = userId, OrgId = orgId }
                );
                return res > 0;
            }
        }
        public async Task<bool> ExistsWithAccessAsync(Guid userId, Guid orgId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizations
                    WHERE UserId = @UserId
                    AND OrganizationId = @OrgId
                    AND AwaitingApproval = false
                    AND Blacklisted = false",
                    new { UserId = userId, OrgId = orgId }
                );
                return res > 0;
            }
        }
        public async Task<Option<UserOrganization>> GetByIdsAsync(Guid userId, Guid orgId)
        {
            var uo = await _applicationContext.Set<UserOrganization>()
                .FirstOrDefaultAsync(uo => uo.UserId == userId && uo.OrganizationId == orgId);
            return uo != null
                ? Option<UserOrganization>.Some(uo)
                : Option<UserOrganization>.None();
        }
    }
}