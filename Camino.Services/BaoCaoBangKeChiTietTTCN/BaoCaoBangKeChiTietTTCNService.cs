using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Services.BaoCaoBangKeChiTietTTCN
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoBangKeChiTietTTCNService))]

    public class BaoCaoBangKeChiTietTTCNService : MasterFileService<YeuCauTiepNhan>, IBaoCaoBangKeChiTietTTCNService
    {
        public BaoCaoBangKeChiTietTTCNService(IRepository<YeuCauTiepNhan> repository) : base(repository)
        {
        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            //var query = BaseRepository.TableNoTracking.Select(s => new ChucVuGridVo
            //{
            //    Id = s.Id,
            //    Ten = s.Ten,
            //    TenVietTat = s.TenVietTat,
            //    MoTa = s.MoTa,
            //    IsDisabled = s.IsDisabled
            //});
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenVietTat);
            //var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            //var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
            //    .Take(queryInfo.Take).ToArrayAsync();
            //await Task.WhenAll(countTask, queryTask);
            //return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
            return null;
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            //var query = BaseRepository.TableNoTracking.Select(s => new ChucVuGridVo
            //{
            //    Id = s.Id,
            //    Ten = s.Ten,
            //    TenVietTat = s.TenVietTat,
            //    MoTa = s.MoTa,
            //    IsDisabled = s.IsDisabled
            //});
            //query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenVietTat);
            //var countTask = query.CountAsync();
            //await Task.WhenAll(countTask);
            //return new GridDataSource { TotalRowCount = countTask.Result };

            return null;

        }
    }
}
