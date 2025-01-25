using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuanHeThanNhan;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.QuanHeThanNhan
{
    [ScopedDependency(ServiceType = typeof(IQuanHeThanNhanService))]
    public class QuanHeThanNhanService : MasterFileService<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan>, IQuanHeThanNhanService
    {
        public QuanHeThanNhanService
             (IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> repository)
            : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);
            if (forExportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = 20000;
            }
            var query = BaseRepository.TableNoTracking.Select(s => new QuanHeThanNhanGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.TenVietTat,
                g => g.MoTa);
            var countTask = queryInfo.LazyLoadPage == true ?
                Task.FromResult(0) :
                query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
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
            var result = BaseRepository.TableNoTracking.Select(s => new QuanHeThanNhanGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten,
                g => g.TenVietTat,
                g => g.MoTa);

            var countTask = result.CountAsync();
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
    }
}
