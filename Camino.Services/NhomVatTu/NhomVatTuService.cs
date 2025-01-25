using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NhomVatTu;
using Camino.Core.Domain.ValueObject.VatTu;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.NhomVatTu
{
    [ScopedDependency(ServiceType = typeof(INhomVatTuService))]
    public class NhomVatTuService : MasterFileService<Core.Domain.Entities.NhomVatTus.NhomVatTu>, INhomVatTuService
    {
        public NhomVatTuService(IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> repository) : base(repository)
        {
        }
        public Task<List<NhomVatTuGridVo>> GetDataTreeView(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new NhomVatTuGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    CapNhom = s.CapNhom,
                    NhomVatTuChaId = s.NhomVatTuChaId
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var list = query
                    .Select(k => new NhomVatTuGridVo
                    {
                        Id = k.Id,
                        Ma = k.Ma,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomVatTuChaId = k.NhomVatTuChaId,
                        ListNhomVatTuChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
                return list.ToListAsync();
            }
            else
            {
                var list = query
                    .Where(x => x.NhomVatTuChaId == 0 || x.NhomVatTuChaId == null)
                    .Select(k => new NhomVatTuGridVo
                    {
                        Id = k.Id,
                        Ma = k.Ma,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomVatTuChaId = k.NhomVatTuChaId,
                       ListNhomVatTuChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
                return list.ToListAsync();
            }

        }
        public static List<NhomVatTuGridVo> GetChildren(List<NhomVatTuGridVo> comments, long Id, long CapNhom)
        {
            var query = comments
                .Where(c => c.Id != Id && c.CapNhom > CapNhom && c.NhomVatTuChaId == Id)
                .Select(c => new NhomVatTuGridVo
                {
                    Id = c.Id,
                    Ma = c.Ma,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    NhomVatTuChaId = c.NhomVatTuChaId,
                    ListNhomVatTuChildren = GetChildren(comments, c.Id, c.CapNhom)
                });
            return query.ToList();
        }


        public async Task<List<LookupTreeItemVo>> GetTreeTemp(DropDownListRequestModel model) //todo: cần xóa
        {
            var query = await BaseRepository.TableNoTracking
                .Select(c => new LookupTreeItemVo
                {
                    KeyId = c.Id,
                    DisplayName = c.Ma + " - " + c.Ten,
                    Level = c.CapNhom + 1,
                    ParentId = c.NhomVatTuChaId
                }).ToListAsync();

            var lst = query
                .Select(c => new LookupTreeItemVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level + 1,
                    ParentId = c.ParentId,
                    Items = GetChildrenTree(query, c.KeyId, c.Level, model.Query, c.DisplayName)
                })
                //.Where(x => x.ParentId == null && (string.IsNullOrEmpty(model.Query) || (!string.IsNullOrEmpty(model.Query) && (x.DisplayName.Trim().ToLower().Contains(model.Query.Trim().ToLower()) || x.Items.Any()))))
                .Where(x => x.ParentId == null && (string.IsNullOrEmpty(model.Query) || (!string.IsNullOrEmpty(model.Query) &&  x.Items.Any())))
                .Take(model.Take)
                .ToList();
            return lst;
        }

        public static List<LookupTreeItemVo> GetChildrenTree(List<LookupTreeItemVo> comments, long Id, long level, string queryString, string parentDisplay) //todo: cần xóa
        {
            var query = comments
                .Where(c => c.ParentId != null && c.ParentId == Id && c.Level == level + 1)
                .Select(c => new LookupTreeItemVo
                {
                    KeyId = c.KeyId,
                    DisplayName = c.DisplayName,
                    Level = c.Level,
                    ParentId = Id,
                    Items = GetChildrenTree(comments, c.KeyId, c.Level, queryString, c.DisplayName)
                })
                .Where(c => string.IsNullOrEmpty(queryString) || (!string.IsNullOrEmpty(queryString) && (parentDisplay.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.DisplayName.Trim().ToLower().Contains(queryString.Trim().ToLower()) || c.Items.Any())))
                .ToList();
            return query;
        }
    }
}
