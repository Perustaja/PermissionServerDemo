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
        private readonly IBackgroundJobClient _jobClient;

        public TenantCleanupBatcher(IOrganizationRepository orgRepo,
            IBackgroundJobClient jobClient)
        {
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _jobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
        }

        /// <summary>
        /// Searches for unsuccessfully created tenants and creates cleanup jobs for each, using a grace period so
        /// tenants that may actually still be being setup are not deleted.
        /// </summary>
        public async Task EnqueueJobs(CancellationToken cancellationToken)
        {
            var orgs = await _orgRepo.GetUnsuccessfullyCreatedAsync();
            foreach (var o in orgs)
                _jobClient.Enqueue<TenantCleanupJob>(tcj => tcj.RunAsync(o.Id, CancellationToken.None));
        }
    }
}