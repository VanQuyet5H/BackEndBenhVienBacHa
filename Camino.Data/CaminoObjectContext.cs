using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Camino.Core;
using Camino.Core.Audit;
using Camino.Core.AuditModel;
using Camino.Core.DependencyInjection;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities;
using Camino.Core.Domain.Entities.Audit;
using Camino.Core.Domain.Entities.GachNos;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT;
using Camino.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Camino.Data
{
    public class CaminoObjectContext : DbContext
    {
        public DbQuery<BaoCaoBienBanKiemKeDPVTDbQuery> BaoCaoBienBanKiemKeDPVTDbQuery { get; set; }
        public DbQuery<BaoCaoBienBanKiemKeXuatDPDbQuery> BaoCaoBienBanKiemKeXuatDPDbQuery { get; set; }
        public DbQuery<BaoCaoBienBanKiemKeXuatVTDbQuery> BaoCaoBienBanKiemKeXuatVTDbQuery { get; set; }
        public DbQuery<BaoCaoBienBanKiemKeKTXuatVTDbQuery> BaoCaoBienBanKiemKeKTXuatVTDbQuery { get; set; }
        public DbQuery<BaoCaoBienBanKiemKeKTXuatDPDbQuery> BaoCaoBienBanKiemKeKTXuatDPDbQuery { get; set; }
        public DbSet<AuditTable> _auditTables { get; set; }
        public DbSet<AuditColumn> _auditColumns { get; set; }
        public AssemblyLoader AssemblyLoader;
        public DbSet<AuditGachNo> _AuditGachNos { get; set; }

        private readonly long _currentUserId;
        public CaminoObjectContext(DbContextOptions<CaminoObjectContext> options, IPrincipal principal) : base(options)
        {
            AutoCommitEnabled = true;
            var claimsPrincipal = principal as ClaimsPrincipal;
            _currentUserId = long.Parse(claimsPrincipal?.Claims.FirstOrDefault(o => o.Type == Core.Helpers.Constants.JwtClaimTypes.Id)?.Value ?? "1");
            AssemblyLoader = new AssemblyLoader();
        }

        private void RepairForSave()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.Entity.WillDelete)
                {
                    entry.State = EntityState.Deleted;
                }
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.Now;
                        entry.Entity.LastTime = DateTime.Now;
                        entry.Entity.CreatedById = _currentUserId;
                        entry.Entity.LastUserId = _currentUserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastTime = DateTime.Now;
                        entry.Entity.LastUserId = _currentUserId;
                        entry.OriginalValues["LastModified"] = entry.Entity.LastModified;
                        break;
                }
            }
        }

        public override int SaveChanges()
        {
            var auditEntries = new AuditResultModel();
            var isAuditSuccess = true;
            try
            {
                auditEntries = OnBeforeSaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isAuditSuccess = false;
            }
            RepairForSave();
            var result = base.SaveChanges();
            if (isAuditSuccess)
            {
                try
                {
                    OnAfterSaveChanges(auditEntries);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var temoraryAuditEntities = AuditNonTemporaryProperties().Result;
            //var auditEntries = OnBeforeSaveChanges();

            RepairForSave();
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            //AuditTemporaryProperties(temoraryAuditEntities);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        /// <summary>
        /// Further configuration the model
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //dynamically load all entity and query type configurations
            var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                (type.BaseType?.IsGenericType ?? false)
                && type.BaseType.GetGenericTypeDefinition() == typeof(CaminoEntityTypeConfiguration<>));

            foreach (var typeConfiguration in typeConfigurations)
            {
                dynamic configuration = Activator.CreateInstance(typeConfiguration);
                modelBuilder.ApplyConfiguration(configuration);
            }

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Modify the input SQL query by adding passed parameters
        /// </summary>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>Modified raw SQL query</returns>
        protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                    continue;

                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                    sql = $"{sql} output";
            }

            return sql;
        }

        #region Methods

        public bool AutoCommitEnabled { get; set; }

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A set for the given entity type</returns>
        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }
        /// <summary>
        /// Generate a script to create all tables for the current model
        /// </summary>
        /// <returns>A SQL script</returns>
        public virtual string GenerateCreateScript()
        {
            return Database.GenerateCreateScript();
        }
        /// <summary>
        /// Creates a LINQ query for the query type based on a raw SQL query
        /// </summary>
        /// <typeparam name="TQuery">Query type</typeparam>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>An IQueryable representing the raw SQL query</returns>
        public virtual IQueryable<TQuery> QueryFromSql<TQuery>(string sql, params object[] parameters) where TQuery : class
        {
            return Query<TQuery>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        /// <summary>
        /// Creates a LINQ query for the entity based on a raw SQL query
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>An IQueryable representing the raw SQL query</returns>
        public virtual IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            return Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        /// <summary>
        /// Executes the given SQL against the database
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <param name="doNotEnsureTransaction">true - the transaction creation is not ensured; false - the transaction creation is ensured.</param>
        /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string</param>
        /// <param name="parameters">Parameters to use with the SQL</param>
        /// <returns>The number of rows affected</returns>
        public virtual int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            //set specific command timeout
            var previousTimeout = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            var result = 0;
            if (!doNotEnsureTransaction)
            {
                //use with transaction
                using (var transaction = Database.BeginTransaction())
                {
                    result = Database.ExecuteSqlCommand(sql, parameters);
                    transaction.Commit();
                }
            }
            else
                result = Database.ExecuteSqlCommand(sql, parameters);

            //return previous timeout back
            Database.SetCommandTimeout(previousTimeout);

            return result;
        }

        /// <summary>
        /// Detach an entity from the context
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        public virtual void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entityEntry = Entry(entity);
            if (entityEntry == null)
                return;

            //set the entity is not being tracked by the context
            entityEntry.State = EntityState.Detached;
        }

        #endregion

        #region Audit

        private AuditResultModel OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            var lstTableToAudit = _auditTables.ToList();
            var lstTableRoot = new List<Dictionary<dynamic, dynamic>>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (/*entry.Entity is AuditHopDongThueVanPhong || entry.Entity is AuditHopDongThueQuay ||*/ entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Metadata.Relational().TableName;

                //

                if (lstTableToAudit.Any(p => p.TableName.Equals(auditEntry.TableName) && p.IsRoot) && !auditEntry.HasTemporaryProperties)
                {
                    var root = new Dictionary<dynamic, dynamic>();
                    int.TryParse(entry.Properties.Where(p => p.Metadata.IsPrimaryKey()).Select(p => p.CurrentValue).First().ToString(), out int valueRoot);
                    root[auditEntry.TableName] = valueRoot;
                    lstTableRoot.Add(root);
                }
                //

                if (lstTableToAudit.All(p => p.TableName != auditEntry.TableName)) continue;

                auditEntries.Add(auditEntry);

                //foreach (var collection in entry.Collections)
                //{
                //    var metaData = collection.Metadata;
                //    var collectionName = collection.Metadata.Name;
                //}



                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    //property ignore
                    if ((entry.State == EntityState.Modified && propertyName.Equals("CreatedOn"))
                        || (entry.State == EntityState.Modified && propertyName.Equals("LastModified") || property?.Metadata?.PropertyInfo?.GetCustomAttribute(typeof(AuditIgnoreAttribute)) != null))
                    {
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.Action = Enums.EnumAudit.Added;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.Action = Enums.EnumAudit.Deleted;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                if (property.OriginalValue == null && property.CurrentValue == null)
                                    continue;

                                if (property.OriginalValue == null ||
                                    property.CurrentValue == null ||
                                    !property.OriginalValue.Equals(property.CurrentValue))
                                {
                                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                                }
                            }
                            auditEntry.Action = Enums.EnumAudit.Modified;
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            //foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            //{
            //    if (auditEntry.OldValues?.Count > 0 || auditEntry.NewValues?.Count > 0)
            //    {
            //        //_audits.Add(auditEntry.ToAudit());
            //        auditEntry.ToAudit();
            //    }
            //}

            // keep a list of entries where the value of some properties are unknown at this step
            //return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
            var result = new AuditResultModel();
            result.AuditEntry = auditEntries.ToList();
            result.LstRoot = lstTableRoot;
            return result;
        }

        private Task OnAfterSaveChanges(AuditResultModel auditResult)
        {
            if (auditResult.AuditEntry == null || auditResult.AuditEntry.Count == 0 || auditResult.LstRoot.Count == 0)
                return Task.CompletedTask;

            long idTemp = 0;
            foreach (var auditEntry in auditResult.AuditEntry.Where(_ => _.HasTemporaryProperties))
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                        if (prop.CurrentValue != null && idTemp == 0)
                        {
                            idTemp = (long) prop.CurrentValue;
                        }
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    //Try to edit Id new for entity children
                    var getListChildrenEntity = GetListStringChildrentName(auditEntry.TableName);
                    foreach (var item in auditResult.AuditEntry)
                    {
                        if (getListChildrenEntity.Any(p => p == item.TableName))
                        {
                            var propertyReference = GetChildReferenceKey(auditEntry.TableName, item.TableName);
                            if (!string.IsNullOrEmpty(propertyReference))
                            {
                                if (auditEntry.Action == Enums.EnumAudit.Added)
                                {
                                    item.NewValues[propertyReference] = prop.CurrentValue;
                                }
                                else if (auditEntry.Action == Enums.EnumAudit.Added)
                                {
                                    item.OldValues[propertyReference] = prop.CurrentValue;
                                }
                            }

                        }
                    }
                }

                // Save the Audit entry
                //_audits.Add(auditEntry.ToAudit());
            }
            //foreach (var audit in auditEntries.Where(_ => !_.HasTemporaryProperties))
            //{
            //    _audits.Add(audit.ToAudit());
            //}
            var resultValue = new Dictionary<dynamic, dynamic>();
            foreach (var root in auditResult.LstRoot)
            {
                var rootName = root.Keys.First().ToString();
                string rootIdStr = root.Values.First().ToString();
                long.TryParse(rootIdStr, out var rootId);
                rootId = rootId == 0 ? idTemp : rootId;
                //get tree
                var rootEntity = _auditTables.ToList().First(p => p.TableName == rootName);

                var rootTree = GetRootTree(rootEntity).OrderByDescending(p => p.Values.First()).ToList();
                var rootTreeForNew = GetRootTree(rootEntity).OrderByDescending(p => p.Values.First()).ToList();

                //var jsonResult = new Dictionary<dynamic, dynamic>();
                var index = -1;


                //var preValue = new Dictionary<dynamic, dynamic>();
                //var nextValue = new Dictionary<dynamic, dynamic>();

                /////////////////////////////// Get assembly of root ///////////////////////
                var assembly = AssemblyLoader.LoadFromAssemblyName(new AssemblyName("Camino.Core"));
                TypeInfo entityAudit = null;
                var isExistAudit = false;
                foreach (var typeInfo in assembly.DefinedTypes)
                {
                    if (typeInfo.Name == "Audit" + rootName)
                    {
                        //entityAudit = Convert.ChangeType(item.Entry.Entity, typeInfo);
                        //long.TryParse(
                        //    entityChild.GetType().GetProperty((string)referenceKey)
                        //        .GetValue(entityChild).ToString(), out var parentID);
                        entityAudit = typeInfo;
                        isExistAudit = true;
                        break;
                    }
                }

                if (!isExistAudit) break;

                var actionEntity = auditResult.AuditEntry.First(p => p.TableName == rootName).Action;
                var classCreated = Activator.CreateInstance(entityAudit);
                classCreated.GetType().GetProperty("Action").SetValue(classCreated, actionEntity);
                classCreated.GetType().GetProperty("CreatedOn").SetValue(classCreated, DateTime.Now);
                classCreated.GetType().GetProperty("TableName").SetValue(classCreated, rootName);
                classCreated.GetType().GetProperty("KeyValues").SetValue(classCreated, rootId);

                //
                //var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == "Audit" + rootName);
                //var auditTest = base.Find(type);
                //

                switch (actionEntity)
                {
                    case Enums.EnumAudit.Added:
                        var valueNewAdd = AddValueResult(rootId, rootName, rootTreeForNew, index, auditResult,
                            resultValue, true, false);
                        var newValueSerializeAdd = JsonConvert.SerializeObject(valueNewAdd);
                        newValueSerializeAdd = newValueSerializeAdd.Replace("[[", "[");
                        newValueSerializeAdd = newValueSerializeAdd.Replace("]]", "]");

                        //rootResult.NewValues = newValueSerializeAdd;
                        //rootResult.OldValues = null;

                        classCreated.GetType().GetProperty("NewValues").SetValue(classCreated, newValueSerializeAdd);
                        classCreated.GetType().GetProperty("OldValues").SetValue(classCreated, null);

                        break;
                    case Enums.EnumAudit.Deleted:
                        var valueOldDeleted = AddValueResult(rootId, rootName, rootTree, index, auditResult,
                            resultValue, true);
                        //try resolve
                        var oldValueSerializeDeleted = JsonConvert.SerializeObject(valueOldDeleted);
                        oldValueSerializeDeleted = oldValueSerializeDeleted.Replace("[[", "[");
                        oldValueSerializeDeleted = oldValueSerializeDeleted.Replace("]]", "]");
                        //rootResult.OldValues = oldValueSerializeDeleted;
                        //rootResult.NewValues = null;

                        classCreated.GetType().GetProperty("NewValues").SetValue(classCreated, null);
                        classCreated.GetType().GetProperty("OldValues")
                            .SetValue(classCreated, oldValueSerializeDeleted);

                        break;
                    case Enums.EnumAudit.Modified:
                        var valueNewModified = AddValueResult(rootId, rootName, rootTreeForNew, index, auditResult,
                            resultValue, true, false);
                        var newValueSerializeModified = JsonConvert.SerializeObject(valueNewModified);
                        newValueSerializeModified = newValueSerializeModified.Replace("[[", "[");
                        newValueSerializeModified = newValueSerializeModified.Replace("]]", "]");

                        var valueOldModified = AddValueResult(rootId, rootName, rootTree, index, auditResult,
                            resultValue, true);
                        //try resolve
                        var oldValueSerializeModified = JsonConvert.SerializeObject(valueOldModified);
                        oldValueSerializeModified = oldValueSerializeModified.Replace("[[", "[");
                        oldValueSerializeModified = oldValueSerializeModified.Replace("]]", "]");

                        //rootResult.NewValues = newValueSerializeModified;
                        //rootResult.OldValues = oldValueSerializeModified;

                        classCreated.GetType().GetProperty("NewValues")
                            .SetValue(classCreated, newValueSerializeModified);
                        classCreated.GetType().GetProperty("OldValues")
                            .SetValue(classCreated, oldValueSerializeModified);

                        break;
                    default:
                        break;
                }

                if (!(classCreated.GetType().GetProperty("OldValues")?.GetValue(classCreated) ?? "").Equals("{}")
                    && !(classCreated.GetType().GetProperty("NewValues")?.GetValue(classCreated) ?? "").Equals("{}"))
                {
                    //_AuditGachNos
                    if (rootName == "GachNo")
                    {
                        if (classCreated is AuditGachNo)
                        {
                            _AuditGachNos.Add(classCreated as AuditGachNo);
                        }
                    }

                    //if (rootName == ConstantsData.HopDongThueVanPhongStr)
                    //{
                    //    if (classCreated is AuditHopDongThueVanPhong)
                    //    {
                    //        _AuditHopDongThueVanPhongs.Add(classCreated as AuditHopDongThueVanPhong);
                    //    }
                    //}
                    //else if (rootName == ConstantsData.LoaiTheXeStr)
                    //{
                    //    if (classCreated is AuditLoaiTheXe)
                    //    {
                    //        _AuditLoaiTheXes.Add(classCreated as AuditLoaiTheXe);
                    //    }
                    //}
                    //else if (rootName == ConstantsData.HopDongThueQuayStr)
                    //{
                    //    if (classCreated is AuditHopDongThueQuay)
                    //    {
                    //        _AuditHopDongThueQuays.Add(classCreated as AuditHopDongThueQuay);
                    //    }
                    //}
                    //else if (rootName == ConstantsData.HopDongThueKhoStr)
                    //{
                    //    if (classCreated is AuditHopDongThueKho)
                    //    {
                    //        _AuditHopDongThueKhos.Add(classCreated as AuditHopDongThueKho);
                    //    }
                    //}
                }

            }


            return SaveChangesAsync();
        }

        private List<string> GetListStringChildrentName(string tableName)
        {
            var result = _auditTables.Where(p => p.TableName == tableName).Select(p => p.ChildReferenceName).ToList();
            return result;
        }

        private string GetChildReferenceKey(string tableName, string childRenferenceName)
        {
            var entity = _auditTables
                .FirstOrDefault(p => p.TableName == tableName && p.ChildReferenceName == childRenferenceName);
            return entity?.ChildReferenceKey;
        }
        private Dictionary<dynamic, dynamic> AddValueResult(long rootId,string rootName, List<Dictionary<dynamic, dynamic>> rootTree
            , int index, AuditResultModel auditResult, Dictionary<dynamic, dynamic> result, bool theFirstLoop = false
            , bool valueOld = true)
        {
            //var preValue = new Dictionary<dynamic, dynamic>();
            var nextValue = new Dictionary<dynamic, dynamic>();
            var indexLoop = 0;
            foreach (var child in rootTree)
            {
                if (index == -1)
                {
                    index = child.Values.First();
                }
                if (index != child.Values.First())
                {
                    if (theFirstLoop)
                    {
                        index = child.Values.First();
                        rootTree.RemoveRange(0 ,indexLoop);
                        var m = AddValueResult(rootId, rootName, rootTree, index, auditResult, nextValue, false, valueOld);
                        return m;
                    }
                    else
                    {

                        var tempValue = new Dictionary<dynamic, dynamic>();
                        //gan result = result gop vao nextValue
                        foreach (var item in nextValue)
                        {
                            //check 1-n ---- cheat
                            if (IsReference1nCheat(item.Key))
                            {
                                if (!result.Any())
                                {
                                    var tempListValue = new List<Dictionary<dynamic, dynamic>>();
                                    foreach (var itemChild in item.Value)
                                    {
                                        foreach(var itemChildChild in itemChild)
                                        {

                                            tempListValue.Add(itemChildChild);
                                        }
                                    }

                                    tempValue[item.Key] = tempListValue;
                                }
                                else
                                {
                                    var tempValueChange = new Dictionary<dynamic, dynamic>();
                                    var listTempValue = new List<Dictionary<dynamic, dynamic>>();
                                    var m = new Dictionary<dynamic, dynamic>();
                                    string className = nextValue.Keys.FirstOrDefault()?.ToString();
                                    foreach (var childRef in result)
                                    {
                                        string keyString = childRef.Key;
                                        var auditTableChild = _auditTables
                                            .FirstOrDefault(p =>
                                                p.TableName == keyString);
                                        if (auditTableChild?.ParentName == className || auditTableChild?.ChildReferenceName == className)
                                        {
                                            if (IsReference1nCheat(childRef.Key))
                                            {
                                                //var lop = new Dictionary<dynamic, dynamic>
                                                //{
                                                //    [childRef.Key] = childRef.Value
                                                //};
                                                //listTempValue.Add(lop);
                                                m[childRef.Key] = childRef.Value;
                                            }
                                            else
                                            {
                                                m[childRef.Key] = childRef.Value;

                                            }
                                        }
                                    }
                                    listTempValue.Add(m);

                                    if (listTempValue.Any())
                                    {
                                        tempValueChange[className] = listTempValue;
                                    }

                                    //Gop result vao nextValue
                                    foreach (var itemChildNext in nextValue)
                                    {
                                        if (IsReference1nCheat(itemChildNext.Key))
                                        {
                                            var array = itemChildNext.Value;
                                            foreach (var itemTemp in array)
                                            {
                                                var n = itemTemp[0] as Dictionary<dynamic, dynamic>;
                                                foreach (var itemChildResult in result)
                                                {
                                                    n[itemChildResult.Key] = itemChildResult.Value;
                                                }
                                            }
                                        }
                                        //foreach (var itemChildResult in result)
                                        //{
                                        //    itemChildNext[itemChildResult.Key] = itemChildResult.Value;
                                        //}
                                    }

                                    if (nextValue.Any())
                                    {
                                        result = nextValue;
                                    }
                                }
                            }
                            //need fix for not 1-n
                            else
                            {
                                if (!result.Any())
                                {
                                    var tempListValue = new List<Dictionary<dynamic, dynamic>>();
                                    foreach (var itemChild in item.Value)
                                    {
                                        tempListValue.Add(itemChild);
                                    }

                                    tempValue[item.Key] = tempListValue;
                                }
                                else
                                {
                                    var tempValueChange = new Dictionary<dynamic, dynamic>();
                                    var listTempValue = new List<Dictionary<dynamic, dynamic>>();
                                    var m = new Dictionary<dynamic, dynamic>();
                                    string className = nextValue.Keys.FirstOrDefault()?.ToString();
                                    foreach (var childRef in result)
                                    {
                                        string keyString = childRef.Key;
                                        var auditTableChild = _auditTables
                                            .FirstOrDefault(p =>
                                                p.TableName == keyString);
                                        if (auditTableChild?.ParentName == className || auditTableChild?.ChildReferenceName == className)
                                        {
                                            if (IsReference1nCheat(childRef.Key))
                                            {
                                                //var lop = new Dictionary<dynamic, dynamic>
                                                //{
                                                //    [childRef.Key] = childRef.Value
                                                //};
                                                //listTempValue.Add(lop);
                                                m[childRef.Key] = childRef.Value;
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                    listTempValue.Add(m);

                                    if (listTempValue.Any())
                                    {
                                        tempValueChange[className] = listTempValue;
                                    }

                                    //Gop result vao nextValue
                                    //cheat neu 1-n moi gop
                                    if (IsReference1nCheat(item.Key))
                                    {
                                        foreach (var itemChildNext in nextValue)
                                        {
                                            if (IsReference1nCheat(itemChildNext.Key))
                                            {
                                                var array = itemChildNext.Value;
                                                foreach (var itemTemp in array)
                                                {
                                                    var n = itemTemp[0] as Dictionary<dynamic, dynamic>;
                                                    foreach (var itemChildResult in result)
                                                    {
                                                        n[itemChildResult.Key] = itemChildResult.Value;
                                                    }
                                                }
                                            }
                                            //foreach (var itemChildResult in result)
                                            //{
                                            //    itemChildNext[itemChildResult.Key] = itemChildResult.Value;
                                            //}
                                        }

                                        if (nextValue.Any())
                                        {
                                            result = nextValue;
                                        }
                                    }
                                    
                                }
                            }
                        }
                        index = child.Values.First();
                        rootTree.RemoveRange(0, indexLoop);
                        if (tempValue.Any())
                        {
                            //rootTree.RemoveRange(0, indexLoop);
                            var m = AddValueResult(rootId, rootName, rootTree, index, auditResult, tempValue, false, valueOld);
                            return m;
                        }
                        else
                        {
                            //rootTree.RemoveRange(0, indexLoop);
                            var m = AddValueResult(rootId, rootName, rootTree, index, auditResult, result, false, valueOld);
                            return m;
                        }
                    }
                }
                var referenceKey = child["ReferenceKey"];
                var childReferenceKey = child["ChildReferenceKey"];
                if ((bool)child["Reference1n"] && index != 0)
                {
                    string className = child.Keys.First();

                    var entryModel = auditResult.AuditEntry.Where(p => p.TableName == className).ToList();
                    if (entryModel.Any())
                    {
                        //var groupEntry = entryModel.GroupBy(p => p.Entry.Entity.Where(p => p.))
                        var lstRefs = new List<List<Dictionary<dynamic, dynamic>>>();
                        var refs = new List<Dictionary<dynamic, dynamic>>();
                        //refs.Add(item.OldValues);
                        if (!string.IsNullOrEmpty((string)referenceKey))
                        {
                            foreach (var item in entryModel)
                            {
                                var assembly = AssemblyLoader.LoadFromAssemblyName(new AssemblyName("Camino.Core"));
                                foreach (var typeInfo in assembly.DefinedTypes)
                                {
                                    if (typeInfo.Name == item.TableName)
                                    {
                                        var entityChild = Convert.ChangeType(item.Entry.Entity, typeInfo);
                                        long.TryParse(
                                            entityChild.GetType().GetProperty((string)referenceKey)
                                                .GetValue(entityChild).ToString(), out var parentID);
                                        item.ReferenceKey = parentID;
                                        //if (item.OldValues.Count > 0) item.OldValues["ReferenceKey"] = parentID;
                                        //if (item.NewValues.Count > 0) item.NewValues["ReferenceKey"] = parentID;
                                        item.OldValues["ReferenceKey"] = parentID;
                                        item.NewValues["ReferenceKey"] = parentID;
                                        item.OldValues["Action"] = item.Action;
                                        item.NewValues["Action"] = item.Action;
                                        item.OldValues["TableName"] = item.Action;
                                        item.NewValues["TableName"] = item.Action;
                                        item.OldValues["Id"] = (long)item.KeyValues["Id"];
                                        item.NewValues["Id"] = (long)item.KeyValues["Id"];
                                        break;
                                    }
                                }
                            }
                        }
                        //Group child
                        var entryGroup = entryModel.GroupBy(p => p.ReferenceKey);
                        foreach (var item in entryGroup)
                        {
                            if (item.Key != null)
                            {
                                foreach (var childItem in item)
                                {
                                    if (valueOld)
                                    {
                                        refs.Add(childItem.OldValues);
                                    }
                                    else
                                    {
                                        refs.Add(childItem.NewValues);
                                    }
                                }
                                lstRefs.Add(refs);
                            }
                        }
                        nextValue[className] = lstRefs;
                    }
                    //Add child reference if parent not modified
                    else
                    {
                        if (result.Any())
                        {
                            var tempValue = new Dictionary<dynamic, dynamic>();
                            var listTempValue = new List<Dictionary<dynamic, dynamic>>();
                            var m = new Dictionary<dynamic, dynamic>();
                            foreach (var childRef in result)
                            {
                                string keyString = childRef.Key;
                                var auditTableChild = _auditTables
                                    .FirstOrDefault(p =>
                                        p.TableName == keyString);
                                if (auditTableChild?.ParentName == className || auditTableChild?.ChildReferenceName == className)
                                {
                                    if (IsReference1nCheat(childRef.Key))
                                    {
                                        var lop = new Dictionary<dynamic, dynamic>
                                        {
                                            [childRef.Key] = childRef.Value
                                        };
                                        //listTempValue.Add(lop);
                                        m[childRef.Key] = childRef.Value;
                                        m["Action"] = Enums.EnumAudit.Modified;
                                        try
                                        {
                                            m["Id"] = childRef.Value[0][0]["ReferenceKey"];
                                        }
                                        catch (Exception)
                                        {
                                            m["Id"] = 0;
                                        }
                                    }
                                    else
                                    {
                                        m[childRef.Key] = childRef.Value;
                                        m["Action"] = Enums.EnumAudit.Modified;
                                        m["Id"] = childRef.Value[0]["ReferenceKey"];
                                    }
                                }
                            }
                            if (m.Count > 0)
                            {
                                listTempValue.Add(m);
                                if (listTempValue.Any())
                                {
                                    tempValue[className] = listTempValue;
                                }

                                if (tempValue.Any())
                                {
                                    result = tempValue;
                                }
                            }
                        }
                    }
                }
                else
                {

                    var refs = new Dictionary<dynamic, dynamic>();
                    var lstRefs = new List<Dictionary<dynamic, dynamic>>();
                    var className = child.Keys.First();

                    var entryModel = auditResult.AuditEntry.Where(p => p.TableName == className).ToList();
                    if (entryModel.Any())
                    {
                        if (!string.IsNullOrEmpty((string)referenceKey))
                        {
                            foreach (var item in entryModel)
                            {
                                var assembly = AssemblyLoader.LoadFromAssemblyName(new AssemblyName("Camino.Core"));
                                foreach (var typeInfo in assembly.DefinedTypes)
                                {
                                    if (typeInfo.Name == item.TableName)
                                    {
                                        var entityChild = Convert.ChangeType(item.Entry.Entity, typeInfo);
                                        long.TryParse(
                                            entityChild.GetType().GetProperty((string)referenceKey)
                                                .GetValue(entityChild).ToString(), out var parentID);

                                        item.ReferenceKey = parentID;
                                        if (item.OldValues.Count > 0) item.OldValues["ReferenceKey"] = parentID;
                                        if (item.NewValues.Count > 0) item.NewValues["ReferenceKey"] = parentID;
                                        item.OldValues["Action"] = item.Action;
                                        item.NewValues["Action"] = item.Action;
                                        item.OldValues["TableName"] = item.Action;
                                        item.NewValues["TableName"] = item.Action;
                                        item.OldValues["Id"] = (long)item.KeyValues["Id"];
                                        item.NewValues["Id"] = (long)item.KeyValues["Id"];
                                    }
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty((string)childReferenceKey))
                        {
                            foreach (var item in entryModel)
                            {
                                item.ReferenceKey = rootId;
                                if (item.OldValues.Count > 0) item.OldValues["ReferenceKey"] = rootId;
                                if (item.NewValues.Count > 0) item.NewValues["ReferenceKey"] = rootId;
                                item.OldValues["Action"] = item.Action;
                                item.NewValues["Action"] = item.Action;
                                item.OldValues["TableName"] = item.Action;
                                item.NewValues["TableName"] = item.Action;
                                item.OldValues["Id"] = (long)item.KeyValues["Id"];
                                item.NewValues["Id"] = (long)item.KeyValues["Id"];
                            }
                        }
                    }
                    if (index != 0)
                    {
                        //Group child
                        var entryGroup = entryModel.GroupBy(p => p.ReferenceKey);
                        foreach (var item in entryGroup)
                        {
                            if (item.Key != null)
                            {
                                foreach (var childItem in item)
                                {
                                    refs = valueOld ? childItem.OldValues : childItem.NewValues;
                                }
                                lstRefs.Add(refs);
                            }
                        }
                        nextValue[className] = lstRefs;
                    }
                    else
                    {
                        foreach (var item in entryModel)
                        {
                            refs = valueOld ? item.OldValues : item.NewValues;
                        }
                        lstRefs.Add(refs);
                        //nextValue[className] = lstRefs;

                        nextValue = refs;
                        foreach (var item in result)
                        {
                            if (IsReference1nCheat(item.Key))
                            {
                                if (result.Any())
                                {
                                    var tempListValue = new List<Dictionary<dynamic, object>>();
                                    foreach (var itemChild in item.Value)
                                    {
                                        //if(itemChild.GetType())
                                        try
                                        {
                                            tempListValue.Add(itemChild);
                                        }
                                        catch (Exception)
                                        {
                                            foreach (var itemChildEx in itemChild)
                                            {
                                                tempListValue.Add(itemChildEx);
                                            }
                                        }
                                        
                                    }

                                    nextValue[item.Key] = tempListValue;
                                }
                            }
                            //need fix
                            else
                            {
                                if (result.Any())
                                {
                                    //var tempListValue = new List<Dictionary<dynamic, object>>();
                                    foreach (var itemChild in item.Value)
                                    {
                                        //if(itemChild.GetType())
                                        try
                                        {
                                            //tempListValue.Add(itemChild);
                                            nextValue[item.Key] = itemChild;
                                        }
                                        catch (Exception)
                                        {
                                            //foreach (var itemChildEx in itemChild)
                                            //{
                                            //    tempListValue.Add(itemChildEx);
                                            //}
                                        }

                                    }

                                    //nextValue[item.Key] = tempListValue;
                                }
                            }
                        }

                        return nextValue;
                    }
                }

                indexLoop++;
            }

            return result;
        }

        private bool IsChildRoot(string tableName, string rootName)
        {
            return _auditTables.Any(p =>
                p.TableName == tableName && (p.ParentName == rootName || p.ChildReferenceName == rootName));
        }
        private bool IsReference1n(string tableName, string parentName)
        {
            if (!string.IsNullOrEmpty(parentName))
            {
                return _auditTables.First(p => p.TableName == tableName && (p.ParentName == parentName || p.ChildReferenceName == parentName)).IsReference1n ?? false;
            }
            return false;
        }

        private bool IsReference1nCheat(string tableName)
        {
            var result = _auditTables.FirstOrDefault(p => p.TableName == tableName)?.IsReference1n ?? false;
            return result;
        }

        private List<Dictionary<dynamic, dynamic>> GetRootTree(AuditTable root, List<Dictionary<dynamic, dynamic>> result = null, int index = 0, List<AuditTable> child = null)
        {
            var childNew = new List<AuditTable>();
            if (result == null)
            {
                result = new List<Dictionary<dynamic, dynamic>>
                {
                    new Dictionary<dynamic, dynamic>
                    {
                        [root.TableName] = index,
                        [""] = "Parent",
                        ["Reference1n"] = false,
                        ["ReferenceKey"] = "",
                        ["ChildReferenceKey"] = "",
                    }
                };
            }
            index++;
            if (child == null)
            {
                var lstChild = _auditTables.Where(p => p.ParentName == root.TableName || p.ChildReferenceName == root.TableName).ToList();
                if (lstChild.Any())
                {
                    result.AddRange(lstChild.Select(p => new Dictionary<dynamic, dynamic>
                    {
                        [p.TableName] = index,
                        [root.TableName] = "Parent",
                        ["Reference1n"] = p.IsReference1n ?? false,
                        ["ReferenceKey"] = p.ReferenceKey ?? "",
                        ["ChildReferenceKey"] = p.ChildReferenceKey ?? "",
                    }));
                    GetRootTree(root, result, index, lstChild);
                }
                else
                {
                    return result;
                }
            }
            else
            {
                foreach (var item in child)
                {
                    var lstChild = _auditTables.Where(p => p.ParentName == item.TableName || p.ChildReferenceName == item.TableName).ToList();
                    if (lstChild.Any())
                    {
                        result.AddRange(lstChild.Select(p => new Dictionary<dynamic, dynamic>
                        {
                            [p.TableName] = index,
                            [item.TableName] = "Parent",
                            ["Reference1n"] = p.IsReference1n ?? false,
                            ["ReferenceKey"] = p.ReferenceKey ?? "",
                            ["ChildReferenceKey"] = p.ChildReferenceKey ?? "",
                        }));
                        childNew.AddRange(lstChild);
                        //GetRootTree(root, result, index, lstChild);
                    }
                }
                if (childNew.Any())
                {
                    GetRootTree(root, result, index, childNew);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }

        private bool IsReference(List<string> listString, string tableName)
        {
            var result = false;

            var listParentName = new List<string>();
            foreach (var item in listString)
            {
                var entity = _auditTables.FirstOrDefault(p => p.TableName == item);
                if (entity == null) continue;
                var parentName = entity.ParentName ?? (entity.ChildReferenceName ?? "");
                listParentName.Add(parentName);
            }

            if (listParentName.Any(p => p == tableName)) result = true;
            return result;
        }
        #endregion Audit

    }
}
