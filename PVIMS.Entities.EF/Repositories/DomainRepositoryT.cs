using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Domain;
using VPS.Common.Repositories;

namespace PVIMS.Entities.EF.Repositories
{
    public class DomainRepository<TEntity> : IRepositoryInt<TEntity> where TEntity : Entity<int>
    {
        private readonly DbContext context;
        private readonly DbSet<TEntity> dbSet;

        public DomainRepository(DbContext dbContext)
        {
            context = dbContext;
            dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Returns a collection of entities.
        /// </summary>
        /// <returns></returns>
        public ICollection<TEntity> List()
        {
            return List(null, null, (string)null);
        }

        /// <summary>
        /// LINQ Queryable interface.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Queryable()
        {
            return dbSet;
        }

        /// <summary>
        /// Returns a list entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Names of properties representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        public ICollection<TEntity> List(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad.Where(s => !string.IsNullOrEmpty(s)))
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        /// <summary>
        /// Returns a list entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        public ICollection<TEntity> List<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query.Include(relatedEntitiesToEagerlyLoad);

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Names of properties representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        public IPagedCollection<TEntity> List(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;
            ICollection<TEntity> data;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var relatedEntityToEagerlyLoad in relatedEntitiesToEagerlyLoad)
            {
                query.Include(relatedEntityToEagerlyLoad);
            }

            var count = query.Count();

            if (orderBy != null)
            {
                data = orderBy(query)
                    .Skip(pagingInfo.FirstResult)
                    .Take(pagingInfo.MaxResults)
                    .ToList();
            }
            else
                data = query.OrderBy(h => h.Id)
                    .Skip(pagingInfo.FirstResult)
                    .Take(pagingInfo.MaxResults)
                    .ToList();

            return new PagedCollection<TEntity>
            {
                TotalRowCount = count,
                Data = data
            };
        }

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        public IPagedCollection<TEntity> List<TProperty>(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad)
        {
            IQueryable<TEntity> query = dbSet;
            ICollection<TEntity> data;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query.Include(relatedEntitiesToEagerlyLoad);

            var count = query.Count();

            if (orderBy != null)
            {
                data = orderBy(query)
                    .Skip(pagingInfo.FirstResult)
                    .Take(pagingInfo.MaxResults)
                    .ToList();
            }
            else
                data = query.OrderBy(h => h.Id)
                    .Skip(pagingInfo.FirstResult)
                    .Take(pagingInfo.MaxResults)
                    .ToList();

            return new PagedCollection<TEntity>
            {
                TotalRowCount = count,
                Data = data
            };
        }

        /// <summary>
        /// Gets the specified entity by identifier.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        public TEntity Get(object entityId)
        {
            return dbSet.Find(entityId);
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entityToSave">The entity to save.</param>
        public void Save(TEntity entityToSave)
        {
            dbSet.Add(entityToSave);

            // Need the Id to be returned.
            context.SaveChanges();
        }

        /// <summary>
        /// Saves the specified entities.
        /// </summary>
        /// <param name="entitiesToSave">The entities to save.</param>
        public void Save(TEntity[] entitiesToSave)
        {
            dbSet.AddRange(entitiesToSave).ToArray();
            // This should be saved in the UnitOfWork.
            //context.SaveChanges();
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        public void Update(TEntity entityToUpdate)
        {
            var previous = context.Configuration.AutoDetectChangesEnabled;
            try
            {

                context.Configuration.AutoDetectChangesEnabled = false;
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            finally
            {
                context.Configuration.AutoDetectChangesEnabled = previous;
            }
        }

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entitiesToUpdate">The entities to update.</param>
        public void Update(TEntity[] entitiesToUpdate)
        {
            foreach (var entityToUpdate in entitiesToUpdate)
            {
                Update(entityToUpdate);
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Deletes the entity represented by the entityToDeleteId.
        /// </summary>
        /// <param name="entityToDeleteId">The entity to delete identifier.</param>
        public void Delete(object entityToDeleteId)
        {
            TEntity entityToDelete = dbSet.Find(entityToDeleteId);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Checks if an instance of the TEntity exists that, optionally, matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? dbSet.Any()
                : dbSet.Any(filter);
        }

        public ICollection<TEntity> ExecuteSql(string sql, params System.Data.SqlClient.SqlParameter[] parameters)
        {
            context.Database.CommandTimeout = 120;
            return context.Database.SqlQuery<TEntity>(sql, parameters).ToList();
        }

        public void ExecuteSqlCommand(string sql, params SqlParameter[] parameters)
        {
            context.Database.ExecuteSqlCommand(sql, parameters);
        }
    }

}
