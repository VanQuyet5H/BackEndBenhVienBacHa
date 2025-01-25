using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoiDan;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.LoiDan
{
    [ScopedDependency(ServiceType = typeof(ILoiDanService))]
    public class LoiDanService : MasterFileService<ICD>, ILoiDanService
    {
        public LoiDanService
            (IRepository<ICD> repository) : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var query = BaseRepository.TableNoTracking.Select(s => new LoiDanGridVo
            {
                Id = s.Id,
                LoiDanCuaBacSi = s.LoiDanCuaBacSi,
                ICD = s.Ma + " - " + s.TenTiengViet
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.LoiDanCuaBacSi,
                g => g.ICD);

            var queryNew = query.Where(p => !string.IsNullOrEmpty(p.LoiDanCuaBacSi));

            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                queryNew.CountAsync();
            var queryTask = queryNew.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new LoiDanGridVo
            {
                Id = s.Id,
                LoiDanCuaBacSi = s.LoiDanCuaBacSi,
                ICD = s.Ma + " - " + s.TenTiengViet
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.LoiDanCuaBacSi,
                g => g.ICD);

            var queryNew = query.Where(p => !string.IsNullOrEmpty(p.LoiDanCuaBacSi));

            var countTask = queryNew.CountAsync();

            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        private void RenameSortForFormatColumn(QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.SortString) &&
                queryInfo.SortString.Contains("Format"))
            {
                queryInfo.SortStringFormat = queryInfo.SortString?
                    .Replace("Format", "");
            }
        }

        public async Task<List<LoiDanTemplateVo>> GetICD(DropDownListRequestModel model)
        {
            var listIcd = await BaseRepository.TableNoTracking
                .Where(p => (p.TenTiengViet.Contains(model.Query ?? "") || p.Ma.Contains(model.Query ?? "")) && string.IsNullOrEmpty(p.LoiDanCuaBacSi))
                .Take(model.Take)
                .ToListAsync();

            var query = listIcd.Select(item => new LoiDanTemplateVo
            {
                DisplayName = item.Ma + " - " + item.TenTiengViet,
                KeyId = item.Id,
                Ma = item.Ma,
                Ten = item.TenTiengViet
            }).ToList();

            return query;
        }
    }
}
