using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

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

        public async Task<bool> UserHasAccess(Guid userId, Guid orgId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM UserOrganizations
                    WHERE UserId = @userId
                    AND OrganizationId = @orgId",
                    new { userId, orgId }
                );
                return res > 0;
            }
        }
    }
}