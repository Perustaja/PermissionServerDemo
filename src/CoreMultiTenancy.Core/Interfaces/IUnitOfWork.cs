using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreMultiTenancy.Core.Interfaces
{
    /// <summary>
    /// Contains functionality to allow for atomic transactions coordinated by services.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Saves all current changes tracked by entity state, or discards if any exception occurs.
        /// </summary>
        Task<int> Commit(CancellationToken cancellationToken = default(CancellationToken));
    }
}