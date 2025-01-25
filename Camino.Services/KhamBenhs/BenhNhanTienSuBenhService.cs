using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BenhNhans;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.KhamBenhs
{
    [ScopedDependency(ServiceType = typeof(IBenhNhanTienSuBenhService))]
    public class BenhNhanTienSuBenhService
        : MasterFileService<BenhNhanTienSuBenh>
            , IBenhNhanTienSuBenhService
    {
        public BenhNhanTienSuBenhService
        (
            IRepository<BenhNhanTienSuBenh> repository
        )
            : base(repository)
        {
        }

        public async Task<ActionResult<GridDataSource>> GetDataGridTienSuBenh(QueryInfo queryInfo)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.BenhNhanId == Convert.ToInt64(queryInfo.AdditionalSearchString))
                .Select(source => new BenhNhanTienSuKhamBenhGridVo
                {
                    Id = source.Id,
                    TenBenh = source.TenBenh,
                });
            var countTask = queryInfo.LazyLoadPage == true ? Task.FromResult(0) : query.CountAsync();
            var queryTask = query.OrderBy(queryInfo.SortString).Skip(queryInfo.Skip)
                .Take(queryInfo.Take).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }

        public async Task<bool> CheckValidDate(DateTime? ngayPhatHien)
        {
            if (ngayPhatHien != null && ngayPhatHien.Value.Date > DateTime.Now.Date)
            {
                return true;
            }

            return false;
        }
    }
}
