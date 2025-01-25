using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.ValueObject.ICDs;

namespace Camino.Services.ICDs
{
    [ScopedDependency(ServiceType = typeof(IDanhMucChuanDoanService))]
    public class DanhMucChuanDoanService : MasterFileService<DanhMucChuanDoan>, IDanhMucChuanDoanService
    {
        public DanhMucChuanDoanService(IRepository<DanhMucChuanDoan> repository) : base(repository)
        {

        }
        public async Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel)
        {
            BuildDefaultSortExpression(queryInfo);

            if(exportExcel)
            {
                queryInfo.Skip = 0;
                queryInfo.Take = int.MaxValue;
            }

            var query = BaseRepository.TableNoTracking.Select(s => new DanhMucChuanDoanGridVo
            {
                Id = s.Id,
                TenTiengViet = s.TenTiengViet,
                TenTiengAnh = s.TenTiengAnh,
                GhiChu = s.GhiChu
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenTiengViet, g => g.TenTiengAnh, g => g.GhiChu);
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking.Select(s => new DanhMucChuanDoanGridVo
            {
                Id = s.Id,
                TenTiengViet = s.TenTiengViet,
                TenTiengAnh = s.TenTiengAnh,
                GhiChu = s.GhiChu
            });
            query = query.ApplyLike(queryInfo.SearchTerms, g => g.TenTiengViet, g => g.TenTiengAnh, g => g.GhiChu);
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }

        public async Task<bool> IsTenViExists(string TenVi = null, long Id = 0)
        {
            var result = false;
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(TenVi));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengViet.Equals(TenVi) && p.Id != Id);
            }
            if (result)
                return false;
            return true;
        }

        public async Task<bool> IsTenEngExists(string TenEng = null, long Id = 0)
        {
            var result = false;
            if (Id == 0)
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.Equals(TenEng));
            }
            else
            {
                result = await BaseRepository.TableNoTracking.AnyAsync(p => p.TenTiengAnh.Equals(TenEng) && p.Id != Id);
            }
            if (result)
                return false;
            return true;
        }
    }
}
