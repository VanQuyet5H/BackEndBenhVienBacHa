using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.NgheNghiep;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Camino.Core.Data;
using Camino.Data;

namespace Camino.Services.NgheNghiep
{
    [ScopedDependency(ServiceType = typeof(INgheNghiepService))]
    public class NgheNghiepService : MasterFileService<Core.Domain.Entities.NgheNghieps.NgheNghiep>, INgheNghiepService
    {
        public NgheNghiepService
            (IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> repository) : base(repository)
        {

        }

        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);
            RenameSortForFormatColumn(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue; 
            }

            var query = BaseRepository.TableNoTracking.Select(s => new NgheNghiepGridVo
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
            var result = BaseRepository.TableNoTracking.Select(s => new NgheNghiepGridVo
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

        public async Task<bool> IsTenNgheNghiepExists(string tenNgheNghiep = null, long ngheNghiepId = 0)
        {
            bool result;

            if(ngheNghiepId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenNgheNghiep));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenNgheNghiep) && p.Id != ngheNghiepId);
            }

            return result;
        }

        public async Task<bool> IsTenVietTatExists(string tenVietTat = null, long ngheNghiepId = 0)
        {
            bool result;

            if (ngheNghiepId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenVietTat));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenVietTat) && p.Id != ngheNghiepId);
            }

            return result;
        }
    }
}
