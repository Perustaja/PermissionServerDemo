using System;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class UserOrganizationRepository : IUserOrganizationRepository
    {
        private readonly string _connectionString;
        private readonly ApplicationDbContext _applicationContext;
        public UserOrganizationRepository(IConfiguration config,
            ApplicationDbContext applicationContext)
        {
          _connectionString = config.GetConnectionString("IdentityDb"); 
          _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }
        public async Task AddAsync(UserOrganization uo)
        {
            await _applicationContext.Set<UserOrganization>().AddAsync(uo);
            await _applicationContext.SaveChangesAsync();          
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
        public async Task<UserOrganization> GetByIdsAsync(Guid userId, Guid orgId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .FirstOrDefaultAsync(uo => uo.UserId == userId && uo.OrganizationId == orgId);
        }
    }
}