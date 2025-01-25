using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Camino.Core;
using Camino.Core.Domain.Entities;
using Camino.Core.Domain.ValueObject;
using Camino.Data;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services
{
    public class MasterFileService<TEntity> : IMasterFileService<TEntity> where TEntity : BaseEntity
    {
        protected readonly IRepository<TEntity> BaseRepository;
        public MasterFileService(IRepository<TEntity> repository)
        {
            BaseRepository = repository;
        }

        //public static Expression<Func<T, bool>> LikeCondition<T>(
        //    string searchTerms, params Expression<Func<T, string>>[] expressions)
        //{
        //    var entityParam = Expression.Parameter(typeof(T), "entity");
        //    Expression[] valueExpressions = new Expression[expressions.Length];

        //    for (int i = 0; i < expressions.Length; i++)
        //    {
        //        valueExpressions[i] = expressions[i].Body.ReplaceParameter(expressions[i].Parameters[0], entityParam);
        //    }


        //    Expression<Func<T, bool>> lastCondition = x => false;

        //    for (int i = 0; i < valueExpressions.Length; i++)
        //    {
        //        Expression<Func<string, bool>> baseExpr = d => d.Contains(searchTerms);

        //        var expr = baseExpr.Body.ReplaceParameter(baseExpr.Parameters[0], valueExpressions[i]);

        //        var condition = Expression.Lambda<Func<T, bool>>(expr, entityParam);
        //        lastCondition = Or(lastCondition, condition);
        //    }
        //    return lastCondition;
        //}

        protected virtual void BuildDefaultSortExpression(IQueryInfo queryInfo)
        {
            if (queryInfo.Sort == null || queryInfo.Sort.Count == 0)
            {
                queryInfo.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }
        }

        protected virtual void ReplaceDisplayValueSortExpression(IQueryInfo queryInfo)
        {
            if (queryInfo.Sort == null) return;
            foreach (var item in queryInfo.Sort)
            {
                if(!string.IsNullOrEmpty(item.Field))
                {
                    item.Field = item.Field.Replace("Display", "");
                }
            }
        }

        public string GetSearchStringForGrid(IQueryInfo queryInfo, ICollection<string> searchColumns)
        {
            var searchString = string.Empty;
            if (!string.IsNullOrEmpty(queryInfo.SearchString))
            {
                var searchConditionList = new List<string>();
                if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
                {
                    var keyword = queryInfo.SearchTerms;
                    var searchCondition = new StringBuilder();

                    searchCondition.Append("(");
                    searchCondition.Append(String.Join(" OR ", searchColumns.Select(column => string.Format(" {0}.Contains(\"{1}\")", column, keyword)).ToArray()));

                    searchCondition.Append(")");
                    searchConditionList.Add(searchCondition.ToString());
                    searchString = String.Join(" OR ", searchConditionList.ToArray<string>());
                }
            }
            return string.IsNullOrEmpty(searchString) ? " 1 = 1" : searchString;
        }

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            return BaseRepository.GetById(id, includes);
        }

        /// <summary>
        /// Get entity by identifier or no
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual TEntity GetByIdFirstOrDefault(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            return BaseRepository.GetByIdFirstOrDefault(id, includes);
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual IEnumerable<TEntity> GetByIds(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            return BaseRepository.GetByIds(ids, includes);
        }
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Add(TEntity entity)
        {
            BaseRepository.Add(entity);
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            BaseRepository.AddRange(entities);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            BaseRepository.Update(entity);
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            BaseRepository.Update(entities);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            BaseRepository.Delete(entity);
        }

        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="id"></param>
        public virtual void DeleteById(long id)
        {
            BaseRepository.DeleteById(id);
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            BaseRepository.Delete(entities);
        }

        #endregion

        #region Async Methods

        /// <summary>
        /// Get async entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual async Task<TEntity> GetByIdAsync(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            return await BaseRepository.GetByIdAsync(id, includes);
        }

        /// <summary>
        /// Get async entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            return await BaseRepository.GetByIdsAsync(ids, includes);
        }
        /// <summary>
        /// Insert async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task AddAsync(TEntity entity)
        {
            await BaseRepository.AddAsync(entity);
        }

        /// <summary>
        /// Insert async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await BaseRepository.AddRangeAsync(entities);
        }

        /// <summary>
        /// Update async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task UpdateAsync(TEntity entity)
        {
            await BaseRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Update async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            await BaseRepository.UpdateAsync(entities);
        }

        /// <summary>
        /// Delete async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            await BaseRepository.DeleteAsync(entity);
        }

        /// <summary>
        /// Delete async entity by id
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task DeleteByIdAsync(long id)
        {
            await BaseRepository.DeleteByIdAsync(id);
        }

        /// <summary>
        /// Delete async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            await BaseRepository.DeleteAsync(entities);
        }

        #endregion
    }
}
