using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Data;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Camino.Services.NoiDungGhiChuMiemGiam
{
    [ScopedDependency(ServiceType = typeof(INoiDungGhiChuMiemGiamService))]
    public class NoiDungGhiChuMiemGiamService : MasterFileService<Core.Domain.Entities.NoiDungGhiChuMiemGiams.NoiDungGhiChuMiemGiam>, INoiDungGhiChuMiemGiamService
    {
        public NoiDungGhiChuMiemGiamService(IRepository<Core.Domain.Entities.NoiDungGhiChuMiemGiams.NoiDungGhiChuMiemGiam> repository) : base(repository)
        {

        }

        public async Task<bool> KiemTraTrungMa(long id, string ma)
        {
            if (string.IsNullOrEmpty(ma))
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking.AnyAsync(x => (id == 0 || x.Id != id) 
                                                                             && x.Ma.ToLower().Trim() == ma.ToLower().Trim());
            return kiemTra;
        }

        public async Task<List<NoiDungGhiChuMiemGiamLookupItemVo>> GetListNoiDungMauAsync(DropDownListRequestModel model)
        {
            var lst = await
                BaseRepository.TableNoTracking
                    .Select(item => new NoiDungGhiChuMiemGiamLookupItemVo()
                    {
                        DisplayName = item.Ten,
                        Ma = item.Ma,
                        Ten = item.Ten,
                        NoiDungMiemGiam = item.NoiDungMiemGiam,
                        KeyId = item.Id
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Take(model.Take)
                    .ToListAsync();
            return lst;
        }

        public async Task<GridDataSource> GetDataForGridNoiDungGhiChuMiemGiamAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);          
            var query = BaseRepository.TableNoTracking
                .Select(s => new NoiDungGhiChuMiemGiamGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten,                   
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetTotalPageForGridNoiDungGhiChuMiemGiamAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new NoiDungGhiChuMiemGiamGridVo()
                {
                    Id = s.Id,
                    Ma = s.Ma,
                    Ten = s.Ten
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ma, g => g.Ten);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
