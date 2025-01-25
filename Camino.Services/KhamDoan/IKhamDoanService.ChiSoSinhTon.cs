using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Microsoft.EntityFrameworkCore.Query;

namespace Camino.Services.KhamDoan
{
    public partial interface IKhamDoanService
    {
        Task<GridDataSource> GetDataListForChiSoSinhTon(long id);

        Task<HopDongKhamSucKhoeNhanVien> GetByHopDongKhamSucKhoeIdAsync(long id,
            Func<IQueryable<HopDongKhamSucKhoeNhanVien>, IIncludableQueryable<HopDongKhamSucKhoeNhanVien, object>> includes = null);

        Task UpdateChiSoSinhTonAsync(HopDongKhamSucKhoeNhanVien entity);
    }
}
