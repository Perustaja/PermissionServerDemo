using PermissionServerDemo.Core.Interfaces;

namespace PermissionServerDemo.Identity.Interfaces
{
    /// <summary>
    /// Exposes an IUnitOfWork for transactions to be used by services. 
    /// </summary>
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}