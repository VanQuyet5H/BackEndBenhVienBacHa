using Camino.Core.Data;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.Marketing;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Camino.Core.DependencyInjection.Attributes;

namespace Camino.Services.QuaTang
{
    [ScopedDependency(ServiceType = typeof(IQuaTangService))]
    public class QuaTangService : MasterFileService<Core.Domain.Entities.QuaTangs.QuaTang>, IQuaTangService
    {
        public QuaTangService(IRepository<Core.Domain.Entities.QuaTangs.QuaTang> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(p => new QuaTangMarketingGridVo
            {
                Id = p.Id,
                Ten = p.Ten,
                DonViTinh = p.DonViTinh,
                MoTa = p.MoTa,
                HieuLuc = p.HieuLuc
            })
            .ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.DonViTinh);

            var queryTask = query.OrderBy(queryInfo.SortString)
                                 .Skip(queryInfo.Skip)
                                 .Take(queryInfo.Take)
                                 .ToArrayAsync();

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();

            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource
            {
                Data = queryTask.Result,
                TotalRowCount = countTask.Result
            };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(p => new QuaTangMarketingGridVo
            {
                Id = p.Id,
                Ten = p.Ten,
                DonViTinh = p.DonViTinh,
                MoTa = p.MoTa,
                HieuLuc = p.HieuLuc
            })
            .ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.DonViTinh);

            var countTask = query.CountAsync();

            await Task.WhenAll(countTask);

            return new GridDataSource
            {
                TotalRowCount = countTask.Result
            };
        }

        public async Task<bool> IsTenExists(string tenQuaTang = null, long quaTangId = 0)
        {
            var result = false;
            if (quaTangId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenQuaTang));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenQuaTang) && p.Id != quaTangId);
            }
            if (result)
                return false;
            return true;
        }
    }
}
