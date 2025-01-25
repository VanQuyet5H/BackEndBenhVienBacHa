using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Linq;
using Camino.Core.Data;
using Camino.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.ValueObject.Thuoc;

namespace Camino.Services.Thuocs
{
    [ScopedDependency(ServiceType = typeof(INhomThuocService))]
    public class NhomThuocService : MasterFileService<NhomThuoc>, INhomThuocService
    {
        public NhomThuocService(IRepository<NhomThuoc> repository) : base(repository)
        { }

      public Task<List<LookupItemVo>> GetListNhomThuoc(LookupQueryInfo queryInfo)
        {
            var listNhomThuoc = BaseRepository.TableNoTracking
                .Select(i => new LookupItemVo
                {
                    DisplayName = i.Ten,
                    KeyId = i.Id
                }).ToList();

            if (!string.IsNullOrEmpty(queryInfo.Query))
            {
                listNhomThuoc = listNhomThuoc.Where(p => p.DisplayName != null && p.DisplayName.ToLower()
                                                             .Contains(queryInfo.Query.ToLower().TrimEnd().TrimStart())).ToList();
            }

            return Task.FromResult(listNhomThuoc);
        }
        public Task<List<NhomThuocGridVo>> GetDataTreeView(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new NhomThuocGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    CapNhom = s.CapNhom,
                    NhomChaId = s.NhomChaId,
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var list = query
                    .Select(k => new NhomThuocGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomChaId = k.NhomChaId,
                        NhomThuocChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
                return list.ToListAsync();
            }
            else
            {
                var list = query.Where(x => x.NhomChaId == 0 || x.NhomChaId == null)
                    .Select(k =>  new NhomThuocGridVo
                    {
                        Id = k.Id,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomChaId = k.NhomChaId,
                        NhomThuocChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);
                return list.ToListAsync();
            }

        }
        public static List<NhomThuocGridVo> GetChildren(List<NhomThuocGridVo> comments, long Id, long CapNhom)
        {
            var query = comments.Where(c => c.Id != Id && c.CapNhom > CapNhom && c.NhomChaId == Id)
                .Select(c => new NhomThuocGridVo()
                {
                    Id = c.Id,
                    Ten = c.Ten,
                    CapNhom = c.CapNhom,
                    NhomChaId = c.NhomChaId,
                    NhomThuocChildren = GetChildren(comments, c.Id, c.CapNhom)
                });
            return query.ToList();
        }
    }
}
