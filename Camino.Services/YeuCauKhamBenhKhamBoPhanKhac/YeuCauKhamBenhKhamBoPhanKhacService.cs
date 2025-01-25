using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenhBoPhanKhac;
using Camino.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenhKhamBoPhanKhac
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhamBenhKhamBoPhanKhacService))]
    public class YeuCauKhamBenhKhamBoPhanKhacService  :MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac>, IYeuCauKhamBenhKhamBoPhanKhacService
    {

        public YeuCauKhamBenhKhamBoPhanKhacService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhKhamBoPhanKhac> repository) : base(repository)
        {
        }
        public async Task<ActionResult<GridDataSource>> GetDataGridBoPhanKhac(long idYCKB)
        {
            var query = BaseRepository.TableNoTracking
                .Where(p => p.YeuCauKhamBenhId == idYCKB)
                .Select(s => new YeuCauKhamBenhBoPhanKhacGridVo
                {
                    Id = s.Id,
                    Ten = s.Ten,
                    NoiDUng = s.NoiDUng,
                    YeuCauKhamBenhId = s.YeuCauKhamBenhId

                });
            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
    }
}
