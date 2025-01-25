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
using Camino.Core.Domain.ValueObject.HDPP;

namespace Camino.Services.HDPP
{
    [ScopedDependency(ServiceType = typeof(IHDPPService))]
    public class HDPPService : MasterFileService<Core.Domain.Entities.HDPP.HDPP>, IHDPPService
    {
        public HDPPService(IRepository<Core.Domain.Entities.HDPP.HDPP> repository) : base(repository)
        {

        }
       
        public async Task<bool> KiemTraTenTrung(long id, string ten)
        {
            if (string.IsNullOrEmpty(ten))
            {
                return false;
            }

            var kiemTra = await BaseRepository.TableNoTracking.AnyAsync(x => (id == 0 || x.Id != id)
                                                                             && x.Ten.ToLower().Trim() == ten.ToLower().Trim());
            return kiemTra;
        }

        public async Task<List<LookupItemVo>> GetListHDPPAsync(DropDownListRequestModel model)
        {
            var lst = await
                BaseRepository.TableNoTracking
                    .Select(item => new LookupItemVo()
                    {
                        DisplayName = item.Ten,
                        KeyId = item.Id
                    })
                    .ApplyLike(model.Query, x => x.DisplayName)
                    .OrderByDescending(x => x.KeyId == model.Id).ThenBy(x => x.DisplayName)
                    .Take(model.Take)
                    .ToListAsync();
            return lst;
        }

        public async Task<GridDataSource> GetDataHDPPForGridAsync(QueryInfo queryInfo)
        {
            BuildDefaultSortExpression(queryInfo);
            var query = BaseRepository.TableNoTracking
                .Select(s => new HDPPVo()
                {
                    Id = s.Id,
                    Ten = s.Ten,
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();

            await Task.WhenAll(countTask, queryTask);
            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<GridDataSource> GetHDPPTotalPageForGridAsync(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Select(s => new NoiDungGhiChuMiemGiamGridVo()
                {
                    Id = s.Id,
                    Ten = s.Ten,
                }).ApplyLike(queryInfo.SearchTerms, g => g.Ten);

            var countTask = query.CountAsync();
            await Task.WhenAll(countTask);

            return new GridDataSource { TotalRowCount = countTask.Result };
        }

    }
}
