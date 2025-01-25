using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.HeThong;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.HeThong
{
    public interface INhatKyHeThongService:IMasterFileService<NhatKyHeThong>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetHoatDongAsync();
        Task<List<LookupItemVo>> GetNguoiTaoAsync(DropDownListRequestModel queryInfo);
    }
}
