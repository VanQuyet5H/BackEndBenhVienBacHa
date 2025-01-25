using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Camino.Core;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Data
{
    /// <summary>
    /// Represents the Entity Framework repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    [ScopedDependency(ServiceType = typeof(IRepository<>))]
    public partial class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly CaminoObjectContext _context;

        private DbSet<TEntity> _entities;

        #endregion

        #region Ctor

        public EfRepository(CaminoObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();

                return _entities;
            }
        }

        public IQueryable<TEntity> ApplyFulltext(string keySearch, string tableName, List<string> lstColumnName)
        {
            if (lstColumnName != null && !string.IsNullOrEmpty(keySearch))
            {
                var theFirst = true;
                var sql = "SELECT * FROM " + tableName + " inner join FREETEXTTABLE(" + tableName + ", (";
                foreach (var column in lstColumnName)
                {
                    if (theFirst)
                    {
                        sql = sql + column;
                        theFirst = false;
                    }
                    else
                    {
                        sql = sql + ", " + column;
                    }
                }
                sql = sql + "), N'\"" + keySearch + "\"') as key_a on [" + tableName +
                      "].ID = key_a.[KEY] Order By key_a.RANK DESC OFFSET 0 ROWS";

                var result = Entities.FromSql(sql);
                return result;
            }
            //var test = "SELECT * FROM [DichVuKyThuatBenhVien] inner join FREETEXTTABLE([DichVuKyThuatBenhVien], ([Ten], [Ma]),'xquang') as key_a on [DichVuKyThuatBenhVien].ID = key_a.[KEY] Order By key_a.RANK DESC";
            return Entities.AsNoTracking();
        }

        public virtual DbContext Context => _context;

        public bool? AutoCommitEnabled { get; set; }

        private bool AutoCommitEnabledInternal => AutoCommitEnabled ?? _context.AutoCommitEnabled;

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            var entity = includes != null ? includes(Table).FirstOrDefault(e => e.Id == id) : Entities.Find(id);
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return entity;
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual TEntity GetByIdFirstOrDefault(long id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            var entity = includes != null ? includes(Table).FirstOrDefault(e => e.Id == id) : Entities.Find(id);
            return entity;
        }
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="ids">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual IEnumerable<TEntity> GetByIds(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            var entitys = includes != null ? includes(Table).Where(e => ids.IndexOf(e.Id)>=0) : Entities.Where(e => ids.IndexOf(e.Id) >= 0);
            if (entitys == null)
                throw new ArgumentNullException(nameof(entitys));
            return entitys;
        }
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Add(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            this.Entities.Add(entity);

            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.AddRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if(entity.Id == 0)
            {
                Entities.Update(entity);
                if (this.AutoCommitEnabledInternal)
                {
                    _context.SaveChanges();
                }
            }
            else
            {
                TEntity exist = this.Entities.Find(entity.Id);
                _context.Entry(exist).CurrentValues.SetValues(entity);

                if (this.AutoCommitEnabledInternal)
                {
                    _context.SaveChanges();
                }
            }

            //Entities.Update(entity);
            //if (this.AutoCommitEnabledInternal)
            //{
            //    _context.SaveChanges();
            //}
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.UpdateRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete entity by id
        /// </summary>
        /// <param name="id"></param>
        public virtual void DeleteById(long id)
        {
            Delete(GetById(id));
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.RemoveRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                _context.SaveChanges();
            }
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
            var entity = includes != null ? await includes(Table).FirstOrDefaultAsync(e => e.Id == id) : await Entities.FindAsync(id);
            //var entity = await Entities.FindAsync(id);
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            return entity;
        }

        /// <summary>
        /// Get async entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="includes"></param>
        /// <returns>Entity</returns>
        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(long[] ids, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null)
        {
            var entitys = includes != null ? await includes(Table).Where(e => ids.IndexOf(e.Id) >= 0).ToListAsync() : await Entities.Where(e => ids.IndexOf(e.Id) >= 0).ToListAsync();
            //var entity = await Entities.FindAsync(id);
            if (entitys == null)
                throw new ArgumentNullException(nameof(entitys));
            return entitys;
        }

        /// <summary>
        /// Insert async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            this.Entities.Add(entity);

            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync(true);
            }
        }

        /// <summary>
        /// Insert async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.AddRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync(true);
            }
        }

        /// <summary>
        /// Update async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if(entity.Id == 0)
            {
                Entities.Update(entity);
                if (this.AutoCommitEnabledInternal)
                {
                    await _context.SaveChangesAsync(true);
                }
            }
            else
            {
                TEntity exist = this.Entities.Find(entity.Id);
                _context.Entry(exist).CurrentValues.SetValues(entity);

                if (this.AutoCommitEnabledInternal)
                {
                    await _context.SaveChangesAsync(true);
                }
            }
            //Entities.Update(entity);
            //if (this.AutoCommitEnabledInternal)
            //{
            //    await _context.SaveChangesAsync(true);
            //}
        }

        /// <summary>
        /// Update async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.UpdateRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync(true);
            }
        }

        /// <summary>
        /// Delete async entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Remove(entity);
            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync(true);
            }
        }

        /// <summary>
        /// Delete async entity by id
        /// </summary>
        /// <param name="id"></param>
        public virtual async Task DeleteByIdAsync(long id)
        {
            await DeleteAsync(await GetByIdAsync(id));
        }
        
        /// <summary>
        /// Delete async entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            Entities.RemoveRange(entities);
            if (this.AutoCommitEnabledInternal)
            {
                await _context.SaveChangesAsync(true);
            }
        }

        #endregion

    }
}
