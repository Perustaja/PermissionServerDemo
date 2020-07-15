using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly string _connectionString;
        public OrganizationRepository(IConfiguration config)
        {
          _connectionString = config.GetConnectionString("IdentityDb"); 
        }
        public async Task<Organization> GetByIdAsync(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                return await conn.QuerySingleOrDefaultAsync<Organization>(
                    @"SELECT * FROM Organizations
                    WHERE Id = @id
                    LIMIT 1", 
                    new { id }
                );
            }
        }

        public async Task<List<Organization>> GetUsersOrgsById(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var res = await conn.QueryAsync<Organization>(
                    @"SELECT * FROM Organizations
                    WHERE O.Id IN 
                    (SELECT OrganizationId FROM UserOrganizations
                    WHERE UserId = @userId)",
                    new { userId }
                );
                return res.ToList();
            }
        }
    }
}