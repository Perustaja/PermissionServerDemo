using System;
using System.Collections.Generic;
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

        public async Task<List<UserOrganization>> GetAllByOrgId(Guid orgId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId && uo.AwaitingApproval == false)
                .Include(uo => uo.User)
                .ToListAsync();
        }

        public async Task<List<UserOrganization>> GetAwaitingAccessByOrgId(Guid orgId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId && uo.AwaitingApproval == true)
                .Include(uo => uo.User)
                .ToListAsync();
        }

        public async Task<Option<UserOrganization>> GetByIdsAsync(Guid orgId, Guid userId)
        {
            var res = await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.OrgId == orgId && uo.UserId == userId)
                .FirstOrDefaultAsync();
            return res != null
                ? Option<UserOrganization>.Some(res)
                : Option<UserOrganization>.None();
        }

        public async Task<Option<Error>> AddAsync(UserOrganization uo)
        {
            try
            {
                await _applicationContext.AddAsync(uo);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None();
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e.ToString());
                return Option<Error>.Some(new Error("", ErrorType.Unspecified));
            }
        }

        public async Task UpdateAsync(UserOrganization uo)
        {
            // Verify this will change Roles as well as the main uo record
            try
            {
                _applicationContext.Update(uo);
                await _applicationContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogInformation(e.ToString());
            }
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
    }
}