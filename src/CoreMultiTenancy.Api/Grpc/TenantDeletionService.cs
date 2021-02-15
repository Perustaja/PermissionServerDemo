using System;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Grpc
{
    public class TenantDeletionService : DeleteTenant.DeleteTenantBase
    {
        private readonly ILogger<TenantDeletionService> _logger;
        private readonly ITenantInfrastructureManager<TenantedDbContext> _tenantInfraManager;

        public TenantDeletionService(ILogger<TenantDeletionService> logger,
            ITenantInfrastructureManager<TenantedDbContext> tenantInfraManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantInfraManager = tenantInfraManager ?? throw new ArgumentNullException(nameof(tenantInfraManager));
        }

        public override async Task<TenantDeletionOutcome> Delete(TenantDeletionRequest request, ServerCallContext ctx)
        => new TenantDeletionOutcome()
        {
            Success = await _tenantInfraManager.DeleteTenantAsync(request.TenantId)
        };
    }
}