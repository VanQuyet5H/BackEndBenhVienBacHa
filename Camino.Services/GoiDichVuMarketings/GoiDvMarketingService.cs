using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GoiDvMarketings;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.GoiDichVuMarketings
{
    [ScopedDependency(ServiceType = typeof(IGoiDvMarketingService))]
    public class GoiDvMarketingService : MasterFileService<GoiDichVu>, IGoiDvMarketingService
    {
        public GoiDvMarketingService
            (IRepository<GoiDichVu> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Where(w => w.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing).Select(s => new GoiDvMarketingGridVo
            {
                Id = s.Id,
                TenGoiDv = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled.GetValueOrDefault()
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.TenGoiDv);

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
            var query = BaseRepository.TableNoTracking.Where(w => w.LoaiGoiDichVu == Enums.EnumLoaiGoiDichVu.Marketing).Select(s => new GoiDvMarketingGridVo
            {
                Id = s.Id,
                TenGoiDv = s.Ten,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled.GetValueOrDefault()
            }).ApplyLike(queryInfo.SearchTerms,
                g => g.TenGoiDv);
            var countTask = query.CountAsync();
            return new GridDataSource
            {
                TotalRowCount = await countTask
            };
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

        public async Task<bool> IsTenExists(Enums.EnumLoaiGoiDichVu loaiGoiDv, string ten = null, long id = 0)
        {
            bool result;

            if (id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.LoaiGoiDichVu == loaiGoiDv);
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(ten) && p.Id != id && p.LoaiGoiDichVu == loaiGoiDv);
            }

            return result;
        }
    }
}
