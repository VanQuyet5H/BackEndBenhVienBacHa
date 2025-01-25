using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camino.Services.KetQuaXetNghiem
{
    public interface IKetQuaXetNghiemService : IMasterFileService<PhienXetNghiem>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? PhienXetNghiemId = null, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListMayXetNghiem(DropDownListRequestModel model);
        Task<int> TrangThaiKQXNGanNhat(long phienXetNghiemId);
    }
}
