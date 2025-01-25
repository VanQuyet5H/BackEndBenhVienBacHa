using Camino.Core.DependencyInjection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TrieuChungs;
using Camino.Data;
using Camino.Services.NhomDichVuKyThuat;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;

namespace Camino.Services.TrieuChung
{
    [ScopedDependency(ServiceType = typeof(ITrieuChungService))]
    public class TrieuChungService : MasterFileService<Core.Domain.Entities.TrieuChungs.TrieuChung>, ITrieuChungService
    {
        private IRepository<Core.Domain.Entities.ICDs.DanhMucChuanDoan> _repositoryDanhMucChuanDoan;
        public TrieuChungService(IRepository<Core.Domain.Entities.TrieuChungs.TrieuChung> repository, IRepository<Core.Domain.Entities.ICDs.DanhMucChuanDoan> repositoryDanhMucChuanDoan) : base(repository)
        {
            _repositoryDanhMucChuanDoan = repositoryDanhMucChuanDoan;
        }

    public Task<List<TrieuChungGridVo>> GetDataTreeView(QueryInfo queryInfo)

    {
        var query = BaseRepository.TableNoTracking
            .Select(s => new TrieuChungGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                CapNhom = s.CapNhom,
                TrieuChungChaId = s.TrieuChungChaId
            });
            List<TrieuChungGridVo> listTree = new List<TrieuChungGridVo>();
            listTree.AddRange(query);

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
        {
                var list = query
                    .Select(k => new TrieuChungGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        TrieuChungChaId = k.TrieuChungChaId,
                        TrieuChungChildren = GetChildrenTreeSearch(listTree, k.Id, k.CapNhom, queryInfo.SearchTerms.RemoveDiacritics(), k.Ten)
                    }).Where(x => x.TrieuChungChaId == null && (string.IsNullOrEmpty(queryInfo.SearchTerms) ||
                                                                !string.IsNullOrEmpty(queryInfo.SearchTerms) && (x.TrieuChungChildren.Any()
                                                                || x.Ten.RemoveDiacritics().Trim().ToLower().Contains(queryInfo.SearchTerms.RemoveDiacritics().Trim().ToLower())))).ToListAsync();

            return list;
            }
        else
        {
            var list = query.Where(x => x.TrieuChungChaId == 0 || x.TrieuChungChaId == null)
                .Select(k => new TrieuChungGridVo
                {
                    Id = k.Id,
                    Ten = k.Ten,
                    CapNhom = k.CapNhom,
                    TrieuChungChaId = k.TrieuChungChaId,
                    TrieuChungChildren = GetChildrenLoadLastFirst(query.ToList(), k.Id, k.CapNhom)
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
            return list.ToListAsync();
        }

    }
        public static List<TrieuChungGridVo> GetChildrenTreeSearch(List<TrieuChungGridVo> comments, long Id, long level, string queryString, string parentDisplay) //todo: cần xóa
        {
            var query = comments
                .Where(c => c.TrieuChungChaId != null && c.TrieuChungChaId == Id && c.CapNhom == level + 1)
                .Select(c => new TrieuChungGridVo
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    TrieuChungChaId = c.TrieuChungChaId,
                    TrieuChungChildren = GetChildrenTreeSearch(comments, c.Id, c.CapNhom, queryString, c.Ten)
                })
                .Where(c => string.IsNullOrEmpty(queryString) || !string.IsNullOrEmpty(queryString) && (parentDisplay.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Ten.RemoveDiacritics().Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.TrieuChungChildren.Any()))
                .ToList();
            return query;
        }
        public bool IsKiemTra(List<long> ids)
    {
            if(ids.Count() == 0) return true;
            if(ids.Count() != 0)
            {
                foreach(var item in ids)
                {
                    var query = _repositoryDanhMucChuanDoan.TableNoTracking
                                .Where(z => z.Id == item).ToList();
                    if (query.Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
    }
    public async Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> FindChildren(long Id)
    {
            var query = await BaseRepository.Table.Include(p=>p.TrieuChungDanhMucChuanDoans)
                .Where(z => z.Id == Id).ToListAsync();
            return query;
    }
    public async Task<IEnumerable<LookupItemVo>> GetDataTreeViewChildren(long Id)
    {
            var query = BaseRepository.TableNoTracking
                .Include(r=>r.TrieuChungDanhMucChuanDoans)
                .Select(s => new TrieuChungGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    TrieuChungChaId = s.TrieuChungChaId
                });

            var queryData = await BaseRepository.TableNoTracking.Where(r => r.TrieuChungChaId == Id)
                .Select(k => new TrieuChungGridVo
                {
                    Id = k.Id,
                    Ten = k.Ten,
                    CapNhom = k.CapNhom,
                    TrieuChungChaId = k.TrieuChungChaId,
                    TrieuChungChildren = GetChildrenLoadLastFirst(query.ToList(), k.Id, k.CapNhom)
                }).ToListAsync();
            var list = new List<LookupItemVo>();
            if (queryData != null && queryData.Count() > 0)
            {
                foreach (var item in queryData)
                {
                    if(item.TrieuChungChildren.Count() > 0)
                    {
                        foreach(var itemChildren in item.TrieuChungChildren)
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
        public async Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> GetCapNhom(long? Id)
    {
            var query = BaseRepository.TableNoTracking.Where(r => r.Id == Id)
                .Select(c => new Core.Domain.Entities.TrieuChungs.TrieuChung
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    TrieuChungChaId = c.TrieuChungChaId
                }); 

            return await query.ToListAsync();
    }
        public async Task<List<Core.Domain.Entities.TrieuChungs.TrieuChung>> GetNameTrieuChung (long? TrieuChungChaId)
        {
            var query = BaseRepository.TableNoTracking.Where(r => r.Id == TrieuChungChaId)
                .Select(c => new Core.Domain.Entities.TrieuChungs.TrieuChung
                {
                    Id = c.Id,
                    Ten = c.Ten,
                   
                });

            return await query.ToListAsync();
        }
        public static List<TrieuChungGridVo> GetChildrenLoadLastFirst(List<TrieuChungGridVo> comments, long Id, long CapNhom)
        {
            var query = comments.Where(c => c.Id != Id && c.CapNhom > CapNhom && c.TrieuChungChaId == Id)
                .Select(c => new TrieuChungGridVo
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    TrieuChungChaId = c.TrieuChungChaId,
                    TrieuChungChildren = GetChildrenLoadLastFirst(comments, c.Id, c.CapNhom)
                });
            return query.ToList();
        }
        public static List<TrieuChungGridVo> GetChildren(List<TrieuChungGridVo> comments, long Id, long CapNhom)
    {
        var query = comments.Where(c => c.Id != Id && c.CapNhom > CapNhom && c.TrieuChungChaId == Id)
            .Select(c => new TrieuChungGridVo
            {
                Id = c.Id,
                Ten = c.Ten,
                CapNhom = c.CapNhom,
                TrieuChungChaId = c.TrieuChungChaId,
                TrieuChungChildren= GetChildren(comments, c.Id, c.CapNhom)
            });
        return query.ToList();
    }
    public async Task<bool> IsTenExists(string ten, long id = 0)
    {
        var result = false;
        if (id == 0)
        {
            result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));

        }
        else
        {
            result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
        }
        if (result)
            return false;
        return true;
    }
     public bool KiemTraExists(long id)
     {
         var result = false;
           var query =  BaseRepository.TableNoTracking.Where(p => p.TrieuChungChaId == id).ToList();
            if (query.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
     }
    public Task<List<LookupItemTemplate>> GetListTrieuChungCha(DropDownListRequestModel model)
    {
            if (long.Parse(model.ParameterDependencies) != 0)
            {
                var query = BaseRepository.TableNoTracking
           .Select(s => new TrieuChungGridVo
           {
               Id = s.Id,
               Ten = s.Ten,
               CapNhom = s.CapNhom,
               TrieuChungChaId = s.TrieuChungChaId
           });
                var data = query.Where(x => (x.TrieuChungChaId == 0 || x.TrieuChungChaId == null) && x.Id != long.Parse(model.ParameterDependencies))
                    .Select(k => new TrieuChungGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        TrieuChungChaId = k.TrieuChungChaId,
                        TrieuChungChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).AsQueryable().ApplyLike(model.Query, g => g.Ten);

                var list = new List<LookupItemTemplate>();
                if (data != null && data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        list.Add(new LookupItemTemplate()
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            CapNhom = item.CapNhom,
                            NhomChaId = item.TrieuChungChaId
                        });
                        AddChildToLookup(list, item.TrieuChungChildren);
                    }
                }
                return Task.FromResult(list);
            }
            else
            {
                var query = BaseRepository.TableNoTracking
           .Select(s => new TrieuChungGridVo
           {
               Id = s.Id,
               Ten = s.Ten,
               CapNhom = s.CapNhom,
               TrieuChungChaId = s.TrieuChungChaId
           });
                var data = query.Where(x => x.TrieuChungChaId == 0 || x.TrieuChungChaId == null)
                    .Select(k => new TrieuChungGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        TrieuChungChaId = k.TrieuChungChaId,
                        TrieuChungChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(model.Query, g => g.Ten);

                var list = new List<LookupItemTemplate>();
                if (data != null && data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        list.Add(new LookupItemTemplate()
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            CapNhom = item.CapNhom,
                            NhomChaId = item.TrieuChungChaId
                        });
                        AddChildToLookup(list, item.TrieuChungChildren);
                    }
                }
                return Task.FromResult(list);
            }
            return null;
        }
        public Task<List<LookupItemTemplate>> GetListTrieuChungCha1(DropDownListRequestModel model)
        {
                var query = BaseRepository.TableNoTracking
           .Select(s => new TrieuChungGridVo
           {
               Id = s.Id,
               Ten = s.Ten,
               CapNhom = s.CapNhom,
               TrieuChungChaId = s.TrieuChungChaId
           });
            var data = query.Where(x => (x.TrieuChungChaId == 0 || x.TrieuChungChaId == null))
                 .Select(k => new TrieuChungGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        TrieuChungChaId = k.TrieuChungChaId,
                        TrieuChungChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(model.Query, g => g.Ten);

                var list = new List<LookupItemTemplate>();
                if (data != null && data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        list.Add(new LookupItemTemplate()
                        {
                            DisplayName = item.Ten,
                            KeyId = item.Id,
                            CapNhom = item.CapNhom,
                            NhomChaId = item.TrieuChungChaId
                        });
                        AddChildToLookup(list, item.TrieuChungChildren);
                    }
                }
                return Task.FromResult(list);
        }
        private List<LookupItemTemplate> AddChildToLookup(List<LookupItemTemplate> list, List<TrieuChungGridVo> trieuChungChildrens ) {
            if (trieuChungChildrens != null && trieuChungChildrens.Count > 0)
            {
                foreach (var item in trieuChungChildrens)
                {
                    list.Add(new LookupItemTemplate()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id,
                        CapNhom = item.CapNhom,
                        NhomChaId = item.TrieuChungChaId
                    });
                    AddChildToLookup(list, item.TrieuChungChildren);
                }
            }
            return list;
    }
    public Task<List<LookupItemVo>> GetListDanhMucChuanDoan(DropDownListRequestModel model)
    {
        var list = _repositoryDanhMucChuanDoan.TableNoTracking
            .Select(i => new LookupItemVo()
            {
                DisplayName = i.TenTiengViet,
                KeyId = i.Id
            }).ApplyLike(model.Query, g => g.DisplayName).ToList();
        return Task.FromResult(list);
    }
        // update 24/7 2020
        public TrieuChungGridVo GetThongTinCha(long id)
        {
            var query = BaseRepository.TableNoTracking.Where(z => z.Id == id).
                Select(s => new TrieuChungGridVo() {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    TrieuChungChaId = s.TrieuChungChaId,
                });
            return query.FirstOrDefault();
        }
    }
}
