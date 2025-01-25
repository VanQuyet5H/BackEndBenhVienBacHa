using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Data;
using System.Linq.Dynamic.Core;
using DotLiquid.Tags;
using iText.StyledXmlParser.Jsoup.Select;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Services.NhomDichVuKyThuat
{
    [ScopedDependency(ServiceType = typeof(INhomDichVuKyThuatService))]
    public class NhomDichVuKyThuatService : MasterFileService<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat>, INhomDichVuKyThuatService
    {
        public NhomDichVuKyThuatService(IRepository<Core.Domain.Entities.DichVuKyThuats.NhomDichVuKyThuat> repository) : base(repository)
        {
        }

        public Task<List<NhomDichVuKyThuatGridVo>> GetDataTreeView(QueryInfo queryInfo)
       {
            var query = BaseRepository.TableNoTracking
                .Select(s => new NhomDichVuKyThuatGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    Ma = s.Ma,
                    CapNhom = s.CapNhom,
                    NhomDichVuKyThuatChaId = s.NhomDichVuKyThuatChaId
                });

            if (!string.IsNullOrEmpty(queryInfo.SearchTerms))
            {
                var list = query
                    .Select(k => new NhomDichVuKyThuatGridVo
                    {
                        Id = k.Id,
                        Ma = k.Ma,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomDichVuKyThuatChaId = k.NhomDichVuKyThuatChaId,
                        NhomDichVuKyThuatChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten).Where(x=>x.CapNhom ==1).Distinct();
                list.Select(a => new NhomDichVuKyThuatGridVo
                {
                    Id = a.Id,
                    Ma = a.Ma,
                    Ten = a.Ten,
                    CapNhom = a.CapNhom,
                    NhomDichVuKyThuatChaId = a.NhomDichVuKyThuatChaId,
                    NhomDichVuKyThuatChildren = null
                });
                return list.ToListAsync();
            }
            else
            {
                var list = query.Where(x => x.NhomDichVuKyThuatChaId == 0 || x.NhomDichVuKyThuatChaId == null)
                    .Select(k => new NhomDichVuKyThuatGridVo
                    {
                        Id = k.Id,
                        Ma = k.Ma,
                        Ten = k.Ten,
                        CapNhom = k.CapNhom,
                        NhomDichVuKyThuatChaId = k.NhomDichVuKyThuatChaId,
                        NhomDichVuKyThuatChildren = GetChildren(query.ToList(), k.Id, k.CapNhom)
                    }).ApplyLike(queryInfo.SearchTerms, g => g.Ten).Distinct();
                return list.ToListAsync();
            }
           
        }
        public static List<NhomDichVuKyThuatGridVo> GetChildren(List<NhomDichVuKyThuatGridVo> comments, long Id ,long CapNhom)
        {
                    var query = comments.Where(c=>c.Id != Id && c.CapNhom > CapNhom && c.NhomDichVuKyThuatChaId == Id)
                        .Select(c => new NhomDichVuKyThuatGridVo
                        {
                            Id = c.Id,
                            Ma = c.Ma,
                            Ten = c.Ten,
                            CapNhom = c.CapNhom,
                            NhomDichVuKyThuatChaId = c.NhomDichVuKyThuatChaId,
                            NhomDichVuKyThuatChildren= GetChildren(comments, c.Id,c.CapNhom)
                        });
                    return query.ToList();
        }
        
    }
}
