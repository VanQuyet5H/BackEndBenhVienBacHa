using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.InputStringStored;
using Camino.Core.Helpers;

namespace Camino.Services.InputStringStored
{
    [ScopedDependency(ServiceType = typeof(IInputStringStoredService))]
    public class InputStringStoredService : MasterFileService<Core.Domain.Entities.InputStringStoreds.InputStringStored>, IInputStringStoredService
    {
        public InputStringStoredService(IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> repository) : base(repository)
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
            var query = BaseRepository.TableNoTracking.Select(s => new InputStringStoredGridVo
            {
                Id = s.Id,
                Key = s.Set,
                KeyDescription = s.Set.GetDescription(),
                Value = s.Value,
            })
            .ApplyLike(queryInfo.SearchTerms, g => g.Value);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new InputStringStoredGridVo
            {
                Id = s.Id,
                Key = s.Set,
                KeyDescription = s.Set.GetDescription(),
                Value = s.Value,
            })
            .ApplyLike(queryInfo.SearchTerms, g => g.Value);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
    }
}
