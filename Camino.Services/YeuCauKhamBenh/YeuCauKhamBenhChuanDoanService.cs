using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Camino.Services.YeuCauKhamBenh
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhChuanDoanService))]
    public class YeuCauKhamBenhChuanDoanService
        : MasterFileService<YeuCauKhamBenhChuanDoan>
            , IYeuCauKhamBenhChuanDoanService
    {
        public YeuCauKhamBenhChuanDoanService
        (
            IRepository<YeuCauKhamBenhChuanDoan> repository
        )
            : base(repository)
        {
        }

        public async Task<ActionResult<GridDataSource>> GetDataGridYeuCauKhamBenhChuanDoan(long id)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == id)
                .Select(source => new YeuCauKhamBenhChuanDoanGridVo
                {
                    Id = source.Id,
                    YeuCauKhamBenhId = source.YeuCauKhamBenhId,
                    ChuanDoanId = source.ChuanDoanId
                });
            var countTask = Task.FromResult(0);
            var queryTask = query.OrderBy("Id asc").Skip(0)
                .Take(1000).ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = 0 };
        }

        public async Task<long> GetIdTask(long yeuCauKhamBenhId, long chuanDoanId)
        {
            var idFirst = BaseRepository.TableNoTracking.Where(p => p.ChuanDoanId == chuanDoanId && p.YeuCauKhamBenhId == yeuCauKhamBenhId).Select(p => p.Id)
                .FirstOrDefault();
            return await Task.FromResult(idFirst);
        }
    }
}
