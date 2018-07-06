using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VPS.Common.Collections;
using VPS.Common.Domain;

namespace VPS.Common.Repositories
{
    /// <summary>
    /// Generic interface for Entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : Entity<long>
    {
        /// <summary>
        /// Returns a collection of entities.
        /// </summary>
        /// <returns></returns>
        ICollection<TEntity> List();

        /// <summary>
        /// LINQ Queryable interface.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Queryable();

        /// <summary>
        /// Returns a collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">The related entities to eagerly load.</param>
        /// <returns></returns>
        ICollection<TEntity> List(Expression<Func<TEntity, bool>> filter, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] relatedEntitiesToEagerlyLoad);

        /// <summary>
        /// Returns a list entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        ICollection<TEntity> List<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad);

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">The related entities to eagerly load.</param>
        /// <returns></returns>
        IPagedCollection<TEntity> List(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] relatedEntitiesToEagerlyLoad);

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="relatedEntitiesToEagerlyLoad">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        IPagedCollection<TEntity> List<TProperty>(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> relatedEntitiesToEagerlyLoad);

        /// <summary>
        /// Gets the specified entity by identifier.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        TEntity Get(object entityId);

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entityToSave">The entity to save.</param>
        void Save(TEntity entityToSave);

        /// <summary>
        /// Saves the specified entities.
        /// </summary>
        /// <param name="entitiesToSave">The entities to save.</param>
        void Save(TEntity[] entitiesToSave);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entitiesToUpdate">The entities to update.</param>
        void Update(TEntity[] entitiesToUpdate);

        /// <summary>
        /// Deletes the entity represented by the entityToDeleteId.
        /// </summary>
        /// <param name="entityToDeleteId">The entity to delete identifier.</param>
        void Delete(object entityToDeleteId);

        /// <summary>
        /// Checks if an instance of the TEntity exists that, optionally, matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        bool Exists(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        ICollection<TEntity> ExecuteSql(string sql, params SqlParameter[] parameters);
    }
}
