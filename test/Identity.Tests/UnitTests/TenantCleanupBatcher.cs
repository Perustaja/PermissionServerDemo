using System.Collections.Generic;
using System.Threading;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;

namespace UnitTests
{
    public class TenantCleanupBatcherTests
    {
        [Fact]
        public async void Enqueues_Unsuccessfully_Created_Tenants()
        {
            var fakeOrg = new Organization("Mock", false);
            // Ensure repo returns a valid org
            var mockRepo = new Mock<IOrganizationRepository>();
            mockRepo.Setup(m => m.GetUnsuccessfullyCreatedAsync())
                .ReturnsAsync(new List<Organization>() { fakeOrg });
            var mockJobClient = new Mock<IBackgroundJobClient>();

            var batcher = new TenantCleanupBatcher(mockRepo.Object, mockJobClient.Object);
            await batcher.EnqueueJobs(CancellationToken.None);

            // Verify job is enqueued
            mockJobClient.Verify(c => c.Create(It.Is<Job>(j => j.Type == typeof(TenantCleanupJob) && j.Args[0].ToString() == fakeOrg.Id.ToString()),
                It.IsAny<EnqueuedState>()));
        }
    }
}