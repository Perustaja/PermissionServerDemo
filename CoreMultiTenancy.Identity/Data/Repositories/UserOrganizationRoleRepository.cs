using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Results.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Perustaja.Polyglot.Option;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class UserOrganizationRoleRepository : IUserOrganizationRoleRepository
    {
        private readonly ILogger<UserOrganizationRoleRepository> _logger;
        private ApplicationDbContext _applicationContext;

        public UserOrganizationRoleRepository(ILogger<UserOrganizationRoleRepository> logger,
            ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Option<Error>> AddAsync(UserOrganizationRole uor)
        {
            try
            {
                await _applicationContext.Set<UserOrganizationRole>().AddAsync(uor);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None;
            }
            catch (DbUpdateException)
            {
                return Option<Error>.Some(new Error("", ErrorType.Unspecified));
            }
        }

        public async Task<Option<Error>> UpdateBulkAsync(List<UserOrganizationRole> uors)
        {
            try
            {
                _applicationContext.Set<UserOrganizationRole>().UpdateRange(uors);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None;
            }
            catch (DbUpdateException)
            {
                return Option<Error>.Some(new Error("", ErrorType.Unspecified));
            }
        }
        public async Task<Option<Error>> DeleteBulkAsync(List<UserOrganizationRole> uors)
        {
            try
            {
                _applicationContext.Set<UserOrganizationRole>().RemoveRange(uors);
                await _applicationContext.SaveChangesAsync();
                return Option<Error>.None;
            }
            catch (DbUpdateException)
            {
                return Option<Error>.Some(new Error("", ErrorType.Unspecified));
            }
        }
    }
}