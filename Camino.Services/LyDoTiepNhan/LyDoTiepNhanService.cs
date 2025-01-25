using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.LyDoTiepNhan;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.LyDoTiepNhan
{
    [ScopedDependency(ServiceType = typeof(ILyDoTiepNhanService))]

    public class LyDoTiepNhanService : MasterFileService<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>, ILyDoTiepNhanService
    {
        public LyDoTiepNhanService(IRepository<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan> repository) : base(repository)
        {
        }
        //public Task<List<LyDoTiepNhanGridVo>> GetDataTreeView(QueryInfo queryInfo)
        //{
        //    var query = BaseRepository.TableNoTracking
        //        .Select(s => new LyDoTiepNhanGridVo
        //        {
        //            Id = s.Id,
        //            Ten = s.Ten,
        //            CapNhom = s.CapNhom,
        //            LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
        //            MoTa = s.MoTa
        //        });
        //    if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
        //    {
        //        var list = query
        //            .Select(s => new LyDoTiepNhanGridVo
        //            {
        //                Id = s.Id,
        //                Ten = s.Ten,
        //                CapNhom = s.CapNhom,
        //                LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
        //                MoTa = s.MoTa,
        //                LyDoTiepNhanChildList = GetChildren(query.ToList(), s.Id, s.CapNhom)
        //            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten).Distinct();
        //        return list.ToListAsync();
        //    }
        //    else
        //    {
        //        var list = query
        //            .Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null)
        //            .Select(s => new LyDoTiepNhanGridVo
        //            {
        //                Id = s.Id,
        //                Ten = s.Ten,
        //                CapNhom = s.CapNhom,
        //                LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
        //                MoTa = s.MoTa,
        //                LyDoTiepNhanChildList = GetChildren(query.ToList(), s.Id, s.CapNhom)
        //            }).ApplyLike(queryInfo.SearchTerms, g => g.Ten).Distinct();
        //        return list.ToListAsync();
        //    }

        //}

        //public static List<LyDoTiepNhanGridVo> GetChildren(List<LyDoTiepNhanGridVo> lstChild, long Id, long CapNhom)
        //{
        //    var query = lstChild
        //        .Where(c => c.Id != Id && c.CapNhom > CapNhom && c.LyDoTiepNhanChaId == Id)
        //        .Select(c => new LyDoTiepNhanGridVo
        //        {
        //            Id = c.Id,
        //            Ten = c.Ten,
        //            MoTa = c.MoTa,
        //            CapNhom = c.CapNhom,
        //            LyDoTiepNhanChaId = c.LyDoTiepNhanChaId,
        //            LyDoTiepNhanChildList = GetChildren(lstChild, c.Id, c.CapNhom)
        //        });
        //    return query.ToList();
        //}

        #region test tìm kiếm con có theo kèm theo cha. => OK
        public List<LyDoTiepNhanGridVo> GetDataTreeView(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new LyDoTiepNhanGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
                    MoTa = s.MoTa
                }).ToList();
            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var list = query
                    .Select(s => new LyDoTiepNhanGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        CapNhom = s.CapNhom + 1,
                        LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
                        MoTa = s.MoTa,
                        LyDoTiepNhanChildList = GetChildren(query, s.Id, s.CapNhom, queryInfo.SearchTerms, s.Ten)
                    }).Where(x =>
                        x.LyDoTiepNhanChaId == null && (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                                               !string.IsNullOrEmpty(queryInfo.SearchTerms) && (x.LyDoTiepNhanChildList.Any()
                                               || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(queryInfo.SearchTerms.RemoveDiacritics().Trim().ToLower()))))
                        .Take(queryInfo.Take).ToList();
                return list;
            }
            else
            {
                var list = query
                    .Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null)
                    .Select(s => new LyDoTiepNhanGridVo
                    {
                        Id = s.Id,
                        Ten = s.Ten,
                        CapNhom = s.CapNhom + 1,
                        LyDoTiepNhanChaId = s.LyDoTiepNhanChaId,
                        MoTa = s.MoTa,
                        LyDoTiepNhanChildList = GetChildren(query, s.Id, s.CapNhom, queryInfo.SearchTerms.RemoveDiacritics(), s.Ten)
                    }).Where(x =>
                        x.LyDoTiepNhanChaId == null && (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                                               !string.IsNullOrEmpty(queryInfo.SearchTerms) && (x.LyDoTiepNhanChildList.Any()
                                               || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(queryInfo.SearchTerms.RemoveDiacritics().Trim().ToLower()))))
                        .Take(queryInfo.Take);
                return list.ToList();
            }

        }

        public static List<LyDoTiepNhanGridVo> GetChildren(List<LyDoTiepNhanGridVo> lstChild, long Id, long capNhom, string queryString = null, string parentDisplay = null)
        {
            var query = lstChild
                   //.Where(c => c.Id != Id && c.CapNhom > CapNhom && c.LyDoTiepNhanChaId == Id)
                   .Where(c => c.LyDoTiepNhanChaId == Id && c.CapNhom == capNhom + 1)
                .Select(c => new LyDoTiepNhanGridVo
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    MoTa = c.MoTa,
                    CapNhom = c.CapNhom,
                    LyDoTiepNhanChaId = c.LyDoTiepNhanChaId,
                    LyDoTiepNhanChildList = GetChildren(lstChild, c.Id, c.CapNhom, queryString, parentDisplay)
                }).Where(c => string.IsNullOrEmpty(queryString) || !string.IsNullOrEmpty(queryString) && (parentDisplay.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Ten.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.LyDoTiepNhanChildList.Any()));

            return query.ToList();
        }
        #endregion


        public Task<List<LookupItemTemplate>> GetListLyDoTiepNhanCha(DropDownListRequestModel model)
        {
            if (long.Parse(model.ParameterDependencies) != 0)
            //if (model.Id != 0)
            {
                var query = BaseRepository.TableNoTracking
                      .Select(s => new LyDoTiepNhanGridVo
                      {
                          Id = s.Id,
                          Ten = s.Ten,
                          CapNhom = s.CapNhom,
                          LyDoTiepNhanChaId = s.LyDoTiepNhanChaId
                      }).ToList();

                var data = query
                    .Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null && x.Id != long.Parse(model.ParameterDependencies))
                    //.Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null && x.Id != model.Id)

                    .Select(k => new LyDoTiepNhanGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        LyDoTiepNhanChaId = k.LyDoTiepNhanChaId,
                        LyDoTiepNhanChildList = GetChildren(query, k.Id, k.CapNhom, model.Query.RemoveDiacritics(), k.Ten)
                    }).Where(x =>
                       x.LyDoTiepNhanChaId == null && (string.IsNullOrEmpty(model.Query) ||
                                              !string.IsNullOrEmpty(model.Query) && (x.LyDoTiepNhanChildList.Any()
                                              || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(model.Query.RemoveDiacritics().Trim().ToLower()))))
                       .Take(model.Take).ToList();

                var list = new List<LookupItemTemplate>();
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        list.Add(new LookupItemTemplate()
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            CapNhom = item.CapNhom,
                            NhomChaId = item.LyDoTiepNhanChaId
                        });
                        AddChildToLookup(list, item.LyDoTiepNhanChildList);
                    }
                }
                return Task.FromResult(list);
            }
            else
            {
                var query = BaseRepository.TableNoTracking
                   .Select(s => new LyDoTiepNhanGridVo
                   {
                       Id = s.Id,
                       Ten = s.Ten,
                       CapNhom = s.CapNhom,
                       LyDoTiepNhanChaId = s.LyDoTiepNhanChaId
                   }).ToList();


                var data = query.Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null)
                   .Select(k => new LyDoTiepNhanGridVo
                   {
                       Id = k.Id,
                       Ten = k.Ten,
                       CapNhom = k.CapNhom,
                       LyDoTiepNhanChaId = k.LyDoTiepNhanChaId,
                       LyDoTiepNhanChildList = GetChildren(query, k.Id, k.CapNhom, model.Query.RemoveDiacritics(), k.Ten)
                   }).Where(x =>
                      x.LyDoTiepNhanChaId == null && (string.IsNullOrEmpty(model.Query) ||
                                             !string.IsNullOrEmpty(model.Query) && (x.LyDoTiepNhanChildList.Any()
                                             || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(model.Query.RemoveDiacritics().Trim().ToLower()))))
                       .Take(model.Take).ToList();

                var list = new List<LookupItemTemplate>();
                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        list.Add(new LookupItemTemplate()
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            CapNhom = item.CapNhom,
                            NhomChaId = item.LyDoTiepNhanChaId
                        });
                        AddChildToLookup(list, item.LyDoTiepNhanChildList);
                    }
                }
                return Task.FromResult(list);
            }
        }

        public Task<List<LookupItemTemplate>> GetListLyDoTiepNhanChaChinhSua(DropDownListRequestModel model)
        {
            var query = BaseRepository.TableNoTracking
               .Select(s => new LyDoTiepNhanGridVo
               {
                   Id = s.Id,
                   Ten = s.Ten,
                   CapNhom = s.CapNhom,
                   LyDoTiepNhanChaId = s.LyDoTiepNhanChaId
               }).ToList();

            var data = query.Where(x => x.LyDoTiepNhanChaId == 0 || x.LyDoTiepNhanChaId == null)
               .Select(k => new LyDoTiepNhanGridVo
               {
                   Id = k.Id,
                   Ten = k.Ten,
                   CapNhom = k.CapNhom,
                   LyDoTiepNhanChaId = k.LyDoTiepNhanChaId,
                   LyDoTiepNhanChildList = GetChildren(query, k.Id, k.CapNhom, model.Query.RemoveDiacritics(), k.Ten)
               }).Where(x =>
                  x.LyDoTiepNhanChaId == null && (string.IsNullOrEmpty(model.Query) ||
                                         !string.IsNullOrEmpty(model.Query) && (x.LyDoTiepNhanChildList.Any()
                                         || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(model.Query.RemoveDiacritics().Trim().ToLower()))))
                       .Take(model.Take).ToList();

            var list = new List<LookupItemTemplate>();
            if (data.Any())
            {
                foreach (var item in data)
                {
                    list.Add(new LookupItemTemplate()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        CapNhom = item.CapNhom,
                        NhomChaId = item.LyDoTiepNhanChaId
                    });
                    AddChildToLookup(list, item.LyDoTiepNhanChildList);
                }
            }
            return Task.FromResult(list);
        }

        private List<LookupItemTemplate> AddChildToLookup(List<LookupItemTemplate> list, List<LyDoTiepNhanGridVo> lyDoChildrens)
        {
            if (lyDoChildrens != null && lyDoChildrens.Count > 0)
            {
                foreach (var item in lyDoChildrens)
                {
                    list.Add(new LookupItemTemplate()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        CapNhom = item.CapNhom,
                        NhomChaId = item.LyDoTiepNhanChaId
                    });
                    AddChildToLookup(list, item.LyDoTiepNhanChildList);
                }
            }
            return list;
        }

        public async Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> GetCapNhom(long? Id)
        {
            var query = BaseRepository.TableNoTracking.Where(r => r.Id == Id)
                .Select(c => new Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    LyDoTiepNhanChaId = c.LyDoTiepNhanChaId
                });

            return await query.ToListAsync();
        }
        //public async Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> GetLyDoTiepNhanName(long? LyDoTiepNhanId)
        //{
        //    var query = await BaseRepository.TableNoTracking.Where(r => r.Id == LyDoTiepNhanId)
        //        .Select(c => new Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan
        //        {
        //            Id = c.Id,
        //            Ten = c.Ten,
        //        }).ToListAsync();

        //    return query;
        //}

        public async Task<List<Core.Domain.Entities.LyDoTiepNhans.LyDoTiepNhan>> FindChildren(long Id)
        {
            var query = await BaseRepository.Table
                .Include(q => q.LyDoTiepNhanCha)
                .Include(q => q.LyDoTiepNhans)
                .Where(z => z.Id == Id).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<LookupItemVo>> GetDataTreeViewChildren(long Id)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new LyDoTiepNhanGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    LyDoTiepNhanChaId = s.LyDoTiepNhanChaId
                });

            var queryData = await BaseRepository.TableNoTracking.Where(r => r.LyDoTiepNhanChaId == Id)
                .Select(k => new LyDoTiepNhanGridVo
                {
                    Id = k.Id,
                    Ten = k.Ten,
                    CapNhom = k.CapNhom,
                    LyDoTiepNhanChaId = k.LyDoTiepNhanChaId,
                    LyDoTiepNhanChildList = GetChildren(query.ToList(), k.Id, k.CapNhom, null, null)
                }).ToListAsync();

            var list = new List<LookupItemVo>();
            if (queryData != null && queryData.Any())
            {
                foreach (var item in queryData)
                {
                    if (item.LyDoTiepNhanChildList.Any())
                    {
                        foreach (var itemChildren in item.LyDoTiepNhanChildList)
                            list.Add(new LookupItemVo()
                            {
                                KeyId = itemChildren.Id,
                            });
                    }
                    list.Add(new LookupItemVo()
                    {
                        KeyId = item.Id,
                    });
                }
            }
            return list;
        }
        public async Task<bool> IsTenExists(string ten, long id = 0, long? lyDoTiepNhanChaId = null)
        {
            var result = false;
            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.LyDoTiepNhanChaId == lyDoTiepNhanChaId);

            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.LyDoTiepNhanChaId == lyDoTiepNhanChaId && p.Id != id);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<string> XoaLyDoTiepNhan(long Id)
        {
            var lyDo = await BaseRepository.GetByIdAsync(Id, s => s.Include(q => q.LyDoTiepNhans));
            if (lyDo != null)
            {
                var gridData = await GetDataTreeViewChildren(lyDo.Id);
                foreach (var model in gridData)
                {
                    var data = await FindChildren(model.KeyId);
                    foreach (var item in data)
                    {
                        item.WillDelete = true;
                    }
                }
                await BaseRepository.DeleteByIdAsync(Id);
            }
            return null;
        }
    }
}
