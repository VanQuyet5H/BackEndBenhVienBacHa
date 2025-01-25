using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.ValueObject.ChucVu;
using System.Collections.Generic;
using Camino.Core.Helpers;

namespace Camino.Services.ChucVu
{
    [ScopedDependency(ServiceType = typeof(IChucVuService))]

    public class ChucVuService : MasterFileService<Core.Domain.Entities.ChucVus.ChucVu>, IChucVuService
    {
        public ChucVuService(IRepository<Core.Domain.Entities.ChucVus.ChucVu> repository) : base(repository)
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

            var query = BaseRepository.TableNoTracking.Select(s => new ChucVuGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenVietTat);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new ChucVuGridVo
            {
                Id = s.Id,
                Ten = s.Ten,
                TenVietTat = s.TenVietTat,
                MoTa = s.MoTa,
                IsDisabled = s.IsDisabled
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.Ten, g => g.TenVietTat);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsTenExists(string tenChucVu = null, long chucVuId = 0)
        {
            var result = false;
            if (chucVuId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenChucVu));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.Ten.Equals(tenChucVu) && p.Id != chucVuId);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> IsTenVietTatExists(string tenChucVuVietTat = null, long chucVuId = 0)
        {
            var result = false;
            if (chucVuId == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenChucVuVietTat));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenVietTat.Equals(tenChucVuVietTat) && p.Id != chucVuId);
            }
            if (result)
                return false;
            return true;
        }
    }
}
