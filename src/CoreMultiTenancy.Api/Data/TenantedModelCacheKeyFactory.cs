using CoreMultiTenancy.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CoreMultiTenancy.Api.Data
{
    /// <summary>
    /// Provides caching when changing db contexts frequently throughout the application lifetime.
    /// See https://stackoverflow.com/questions/41979215/how-to-implement-imodelcachekeyfactory-in-ef-core
    /// for implementation by Microsoft employee.
    /// </summary>
    public class TenantedModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var tdb = context as ITenantedDbContext 
                ?? throw new System.Exception("Unable to cast context to ITenantedDbContext");
            return tdb.TenantModelCacheKey;
        }
    }
}