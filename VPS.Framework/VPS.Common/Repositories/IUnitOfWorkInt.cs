using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPS.Common.Domain;

namespace VPS.Common.Repositories
{
    /// <summary>
    /// Defines the interface for an implementation of the Unit of Work design pattern
    /// http://martinfowler.com/eaaCatalog/unitOfWork.html
    /// </summary>
    public interface IUnitOfWorkInt : IDisposable
    {
        /// <summary>
        /// Returns a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        IRepositoryInt<TEntity> Repository<TEntity>() where TEntity : Entity<int>;
        /// <summary>
        /// Starts a Unit of Work if one isn't already running.
        /// </summary>
        void Start();
        /// <summary>
        /// Flushes all changes to the data store.
        /// </summary>
        void Complete();
    }
}
