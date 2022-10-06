using CoreMultiTenancy.Core.Interfaces;
using CoreMultiTenancy.Identity.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OrganizationRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }
        public OrganizationRepository(IConfiguration config, ILogger<OrganizationRepository> logger,
            ApplicationDbContext applicationContext)
        {
            _connectionString = config.GetConnectionString("IdentityDb");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        public async Task<List<Organization>> GetUsersOrgsByIdAsync(Guid userId)
        {
            return await _applicationContext.Set<UserOrganization>()
            .Where(uo => uo.UserId == userId)
            .Include(uo => uo.Organization)
            .Select(uo => uo.Organization)
            .ToListAsync();
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            using (var conn = new SqliteConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<int>(
                    @"SELECT COUNT(*) FROM Organizations 
                    WHERE Id = @Id AND IsActive = true",
                    new { Id = id }
                );
                return res > 0;
            }
        }

        public async Task<Option<Organization>> GetByIdAsync(Guid id)
        {
            var res = await _applicationContext.Set<Organization>()
            .FirstOrDefaultAsync(o => o.Id == id);
            return res != null
                ? Option<Organization>.Some(res)
                : Option<Organization>.None;
        }

        public Organization Add(Organization o)
            => _applicationContext.Set<Organization>().Add(o).Entity;

        public Organization Update(Organization o)
            => _applicationContext.Set<Organization>().Update(o).Entity;

        public void Delete(Organization o)
            => _applicationContext.Set<Organization>().Remove(o);
    }
}