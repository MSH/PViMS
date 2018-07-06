using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Domain;
using VPS.Common.Repositories;

namespace PVIMS.Entities.EF.Repositories
{
    public class EntityFrameworkUnitOfWork : IUnitOfWorkInt, IDisposable
    {
        // Need to make this public for the demo as associations aren't loading without the Include().
        private PVIMSDbContext dbContext;
        private Hashtable repositories;
        private bool disposed;

        /// <summary>
        /// Returns a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        public IRepositoryInt<TEntity> Repository<TEntity>() where TEntity : Entity<int>
        {
            if (dbContext == null)
            {
                Start();
            }

            if (repositories == null)
                repositories = new Hashtable();

            var entityTypeName = typeof(TEntity).Name;

            if (!repositories.ContainsKey(entityTypeName))
            {
                var repositoryType = typeof(DomainRepository<>);

                // Could replace this with call to DependencyDepency
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), dbContext);

                repositories.Add(entityTypeName, repositoryInstance);
            }

            return (IRepositoryInt<TEntity>)repositories[entityTypeName];
        }

        /// <summary>
        /// Starts a Unit of Work if one isn't already running.
        /// </summary>
        public void Start()
        {
            if (dbContext == null)
                dbContext = new PVIMSDbContext();
        }

        /// <summary>
        /// Flushes all changes to the data store.
        /// </summary>
        public void Complete()
        {
            if (dbContext != null)
            {
                try
                {
                    dbContext.SaveChanges();
                }
                catch (DbEntityValidationException exception)
                {
                    foreach (var error
                        in exception.EntityValidationErrors)
                    {
                        foreach (var validationError
                            in error.ValidationErrors)
                        {
                            Debug.WriteLine("{0}.{1} - {2}",
                                error.Entry.Entity.GetType().Name,
                                validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                    // Clean up any references to objects that need disposal (loop through hash set)
                    // If not using DepencyResovler.Resolve for Repo's
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
