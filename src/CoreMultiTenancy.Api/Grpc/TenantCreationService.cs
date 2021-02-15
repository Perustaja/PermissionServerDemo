using System;
using System.Threading.Tasks;
using Cmt.Protobuf;
using CoreMultiTenancy.Api.Data;
using CoreMultiTenancy.Api.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CoreMultiTenancy.Api.Grpc
{
    public class TenantCreationService : CreateTenant.CreateTenantBase
    {
        private readonly ILogger<TenantCreationService> _logger;
        private readonly ITenantInfrastructureManager<TenantedDbContext> _tenantInfraManager;

        public TenantCreationService(ILogger<TenantCreationService> logger,
            ITenantInfrastructureManager<TenantedDbContext> tenantInfraManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantInfraManager = tenantInfraManager ?? throw new ArgumentNullException(nameof(tenantInfraManager));
        }

        public override async Task<TenantCreationOutcome> Create(TenantCreationRequest request, ServerCallContext ctx)
        => new TenantCreationOutcome()
        {
            Success = await _tenantInfraManager.InitializeTenantAsync(request.TenantId)
        };
    }
}