using System;
using System.Threading;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Identity.Data.Repositories;
using Hangfire;

namespace CoreMultiTenancy.Identity.Jobs
{
    /// <summary>
    /// Idempotently requests that the Api removes a tenant which encountered an error during initialization. If some unspecified 
    /// error or crash occurs during tenant initilization, a database for the tenant may have been created but the full request 
    /// transaction did not finish. This job just ensures that a tenant that was not completely created is deleted fully.
    /// </summary>
    public class TenantCleanupJob
    {
        private readonly IOrganizationRepository _orgRepo;
        private readonly DeleteTenant.DeleteTenantClient _deleteTenantClient;

        public TenantCleanupJob(IOrganizationRepository orgRepo,
            DeleteTenant.DeleteTenantClient deleteTenantClient)
        {
            _orgRepo = orgRepo ?? throw new ArgumentNullException(nameof(orgRepo));
            _deleteTenantClient = deleteTenantClient ?? throw new ArgumentNullException(nameof(deleteTenantClient));
        }
        /// <summary>
        /// Performs a transaction, ensuring that the given tenant is fully deleted across servers.
        /// </summary>
        [AutomaticRetry]
        public async Task RunAsync(Guid id, CancellationToken cancellationToken)
        {
            var org = await _orgRepo.GetByIdAsync(id);
            if (org.IsSome())
            {
                var reply = await _deleteTenantClient.DeleteAsync(new TenantDeletionRequest() { TenantId = id.ToString() });
                if (reply.Success)
                {
                    _orgRepo.Delete(org.Unwrap());
                    await _orgRepo.UnitOfWork.Commit();
                }
                else
                    throw new Exception("Tenant deletion request did not end in success."); // Throwing an exception sets the task to be retried
            }
        }
    }
}