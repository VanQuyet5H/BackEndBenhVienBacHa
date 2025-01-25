using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.XuatKhoQuaTangMarketing
{
    public partial interface IXuatKhoQuaTangService
    {
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetNhanVienXuat(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetNguoiNhan(DropDownListRequestModel queryInfo);
    }
}
