using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Camino.Core;
using Camino.Core.Domain.Entities;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> ApplyFulltext(string keySearch, string tableName, List<string> lstColumnName);
        #region Properties
        DbContext Context { get; }
        bool? AutoCommitEnabled { get; set; }
        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<TEntity> TableNoTracking { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        TEntity GetById(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        /// <summary>
        /// Get entity by identifier or no
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        TEntity GetByIdFirstOrDefault(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="ids">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        IEnumerable<TEntity> GetByIds(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Add(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="id"></param>
        void DeleteById(long id);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<TEntity> entities);

        #endregion

        #region Async Methods

        /// <summary>
        /// Get async entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        Task<TEntity> GetByIdAsync(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        /// <summary>
        /// Get async entitys by identifier
        /// </summary>
        /// <param name="ids">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        Task<IEnumerable<TEntity>> GetByIdsAsync(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null);

        /// <summary>
        /// Insert async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Insert async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Update async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Update async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        Task UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete async entity by id
        /// </summary>
        /// <param name="id"></param>
        Task DeleteByIdAsync(long id);

        /// <summary>
        /// Delete async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        Task DeleteAsync(IEnumerable<TEntity> entities);

        #endregion
    }
}
