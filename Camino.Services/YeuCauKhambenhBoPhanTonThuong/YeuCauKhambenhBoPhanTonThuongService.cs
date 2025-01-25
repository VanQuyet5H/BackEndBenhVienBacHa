using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenhBoPhanTonThuong;
using Camino.Data;
using Camino.Services.YeuCauKhamBenhKhamBoPhanKhac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhambenhBoPhanTonThuong
{
    [ScopedDependency(ServiceType = typeof(IYeuCauKhambenhBoPhanTonThuongService))]
    public class YeuCauKhambenhBoPhanTonThuongService : MasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhBoPhanTonThuong>, IYeuCauKhambenhBoPhanTonThuongService
    {
        public YeuCauKhambenhBoPhanTonThuongService(IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhBoPhanTonThuong> repository) : base(repository)
        {
        }
        public async Task<ActionResult<GridDataSource>> GetDataGridBoPhanTonThuong(long idYCKB)
        {
            var query = BaseRepository.TableNoTracking
                .Include(x => x.YeuCauKhamBenh)
                .Where(y => y.YeuCauKhamBenhId == idYCKB)
                .Select(s => new YeuCauKhamBenhBoPhanTonThuongGridVo()
                {
                    Id = s.Id,
                    HinhAnh =s.HinhAnh,
                    MoTa = s.MoTa
                });
            var countTask = query.CountAsync();
            var queryTask = query.ToArrayAsync();
            await Task.WhenAll(countTask, queryTask);

            return new GridDataSource { Data = queryTask.Result, TotalRowCount = countTask.Result };
        }
    }
}
