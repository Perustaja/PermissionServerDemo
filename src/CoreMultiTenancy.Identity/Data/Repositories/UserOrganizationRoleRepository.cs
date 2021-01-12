using System;
using System.Collections.Generic;
using CoreMultiTenancy.Identity.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Identity.Data.Repositories
{
    public class UserOrganizationRoleRepository : IUserOrganizationRoleRepository
    {
        private readonly ILogger<UserOrganizationRoleRepository> _logger;
        private readonly ApplicationDbContext _applicationContext;
        public IUnitOfWork UnitOfWork { get => _applicationContext; }

        public UserOrganizationRoleRepository(ILogger<UserOrganizationRoleRepository> logger,
            ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _applicationContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public UserOrganizationRole Add(UserOrganizationRole uor)
            => _applicationContext.Set<UserOrganizationRole>().Add(uor).Entity;

        public void UpdateBulk(List<UserOrganizationRole> uors)
            => _applicationContext.Set<UserOrganizationRole>().UpdateRange(uors);

        public void DeleteBulk(List<UserOrganizationRole> uors)
            => _applicationContext.Set<UserOrganizationRole>().RemoveRange(uors);
    }
}