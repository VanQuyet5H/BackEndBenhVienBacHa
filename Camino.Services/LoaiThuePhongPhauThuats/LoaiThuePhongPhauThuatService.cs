using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.CauHinhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.LoaiThuePhongPhauThuats;
using Camino.Data;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.LoaiThuePhongPhauThuats
{
    [ScopedDependency(ServiceType = typeof(ILoaiThuePhongPhauThuatService))]
    public class LoaiThuePhongPhauThuatService : MasterFileService<LoaiThuePhongPhauThuat>, ILoaiThuePhongPhauThuatService
    {
        public LoaiThuePhongPhauThuatService(IRepository<LoaiThuePhongPhauThuat> repository) : base(repository)
        {

        }

        #region Grid
        public async Task<GridDataSource> GetDataForGridCauHinhThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = BaseRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten)
                .Select(s => new LoaiThuePhongPhauThuatGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };

        }
        public async Task<GridDataSource> GetTotalPageForGridCauHinhThuePhongAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);

            var query = BaseRepository.TableNoTracking
                .ApplyLike(queryInfo.SearchTerms?.Trim(), x => x.Ten)
                .Select(s => new LoaiThuePhongPhauThuatGridVo
                {
                    Id = s.Id
                });
            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);
            return new GridDataSource { TotalRowCount = countTask.Result };
        }
        #endregion

        #region Check data
        public async Task<bool> KiemTraTrungTenAsync(long loaiThuePhongPhauThuatId, string ten)
        {
            if (string.IsNullOrEmpty(ten))
            {
                return true;
            }

            var check = await BaseRepository.TableNoTracking
                .AnyAsync(x => x.Ten.Equals(ten)
                               && (loaiThuePhongPhauThuatId == 0 || x.Id != loaiThuePhongPhauThuatId));
            return !check;
        }


        #endregion

        #region Get data
        public async Task<List<LookupItemVo>> GetListLoaiThuePhongPhauThuatAsync(DropDownListRequestModel model)
        {
            var lst = await BaseRepository.TableNoTracking
                .ApplyLike(model.Query?.Trim(), x => x.Ten)
                .OrderByDescending(x => x.Id == model.Id).ThenBy(x => x.Id)
                .Select(x => new LookupItemVo()
                {
                    KeyId = x.Id,
                    DisplayName = x.Ten
                })
                .Take(model.Take)
                .ToListAsync();
            return lst;
        }


        #endregion
    }
}
