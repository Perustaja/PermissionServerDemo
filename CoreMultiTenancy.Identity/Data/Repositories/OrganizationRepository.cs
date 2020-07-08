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
        public Task<Organization> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Organization>> GetUsersOrgsById(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var result = await conn.QueryAsync<Organization>(
                    @"SELECT * FROM Organizations
                    WHERE O.Id IN 
                    (SELECT OrganizationId FROM UserOrganizations
                    WHERE UserId = @UserId)",
                    new { UserId = userId }
                );
                return result.ToList();
            }
        }
    }
}