using System;
using System.Threading;
using System.Threading.Tasks;
using CoreMultiTenancy.Identity.Data.Repositories;
using Hangfire;

namespace CoreMultiTenancy.Identity.Jobs
{
    public class TenantCleanupBatcher
    {
        private readonly IOrganizationRepository _orgRepo;

        public TenantCleanupBatcher(IOrganizationRepository orgRepo)
        {
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
        }

        /// <summary>
        /// Searches for unsuccessfully created tenants and creates cleanup jobs for each, using a grace period so
        /// tenants that may actually still be being setup are not deleted.
        /// </summary>
        public async Task EnqueueJobs(CancellationToken cancellationToken)
        {
            var orgs = await _orgRepo.GetUnsuccessfullyCreatedAsync();
            foreach (var o in orgs)
                BackgroundJob.Enqueue<TenantCleanupJob>(tcj => tcj.RunAsync(o.Id, CancellationToken.None));
        }
    }
}