using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class AccessRevokedEventRepository : IAccessRevokedEventRepository
    {
        private readonly string _connectionString;
        public AccessRevokedEventRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("IdentityDb") 
                ?? throw new ArgumentNullException(nameof(config));
        }
        public async Task<AccessRevokedEvent> GetByUserIdAsync(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleOrDefaultAsync<AccessRevokedEvent>(
                    @"SELECT * FROM AccessRevokedEvents WHERE userid=@UserId",
                    new { UserId = userId }
                );
                return res;
            }
        }
        public async Task DeleteAsync(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var res = await conn.ExecuteAsync(
                    @"DELETE AccessRevokedEvent WHERE UserId=@UserId",
                    new { UserId = userId }
                );
            } 
        }
        public async Task AddAsync(Guid userId, Guid orgId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var res = await conn.ExecuteAsync(
                    @"INSERT INTO AccessRevokedEvents
                    (UserId, OrganizationId)
                    VALUES (@UserId, @OrgId)
                    ON DUPLICATE KEY UPDATE
                    OrganizationId=VALUES(OrganizationId)",
                    new { UserId = userId, OrgId = orgId }
                );
            } 
        }
    }
}