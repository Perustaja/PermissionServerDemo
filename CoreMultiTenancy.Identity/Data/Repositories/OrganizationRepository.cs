using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _applicationContext;
        public OrganizationRepository(ApplicationDbContext applicationContext)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }
        public async Task<Organization> GetByIdAsync(Guid id)
        {
            return await _applicationContext.Set<Organization>()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Organization>> GetUsersOrgsById(Guid userId)
        {
            return await _applicationContext.Set<UserOrganization>()
                .Where(uo => uo.UserId == userId)
                .Include(uo => uo.Organization)
                .Select(uo => uo.Organization)
                .ToListAsync();
        }
    }
}