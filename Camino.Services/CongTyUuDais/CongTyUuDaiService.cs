using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.CongTyUuDais;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.CongTyUuDais
{
    [ScopedDependency(ServiceType = typeof(ICongTyUuDaiService))]
    public class CongTyUuDaiService : MasterFileService<CongTyUuDai>, ICongTyUuDaiService
    {
        public CongTyUuDaiService
            (IRepository<CongTyUuDai> repository) : base(repository)
        { }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if (exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new CongTyUuDaiGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten, g=> g.MoTa);

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
            var query = BaseRepository.TableNoTracking.Select(s => new CongTyUuDaiGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.Ten, g => g.MoTa);
            var countTask = query.CountAsync();
            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
        }

        public async Task<bool> IsTenExists(string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id);
            }

            return result;
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
