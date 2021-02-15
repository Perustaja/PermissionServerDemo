using System;
using System.Threading;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Identity.Data.Repositories;
using CoreMultiTenancy.Identity.Entities;
using CoreMultiTenancy.Identity.Jobs;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;
using Perustaja.Polyglot.Option;
using Xunit;

namespace UnitTests
{
    public class TenantCleanupJobTests
    {
        [Fact]
        public async Task Throws_If_Request_FailsAsync()
        {
            var mockOrg = new Organization("Mock", false);
            var mockRepo = new Mock<IOrganizationRepository>();
            // Ensure repo returns a valid org
            mockRepo.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Option<Organization>.Some(mockOrg));
            // Construct fake reply
            var failedResult = new TenantDeletionOutcome() { Success = false };
            var mockReply = TestCalls.AsyncUnaryCall<TenantDeletionOutcome>(Task.FromResult(failedResult), Task.FromResult(new Metadata()),
                 () => Status.DefaultSuccess, () => new Metadata(), () => { });
            // Ensure client returns fake reply (https://github.com/grpc/grpc/tree/master/src/csharp/Grpc.Examples.Tests for examples)
            var mockClient = new Mock<DeleteTenant.DeleteTenantClient>();
            mockClient.Setup(m => m.DeleteAsync(It.IsAny<TenantDeletionRequest>(), null, null, CancellationToken.None)).Returns(mockReply);

            var job = new TenantCleanupJob(mockRepo.Object, mockClient.Object);

            await Assert.ThrowsAsync<Exception>(() => job.RunAsync(mockOrg.Id, CancellationToken.None));
        }

        [Fact]
        public async Task Succeeds_If_Org_Not_Found()
        {
            var mockOrg = new Organization("Mock", false);
            var mockRepo = new Mock<IOrganizationRepository>();
            // Ensure repo returns None
            mockRepo.Setup(m => m.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Option<Organization>.None);
            // Client shouldn't need any behavior but cannot be null
            var mockClient = new Mock<DeleteTenant.DeleteTenantClient>();

            var job = new TenantCleanupJob(mockRepo.Object, mockClient.Object);

            var exception = await Record.ExceptionAsync(() => job.RunAsync(mockOrg.Id, CancellationToken.None));
            Assert.Null(exception);
        }
    }
}