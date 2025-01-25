using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DinhMucDuocPhamTonKho
{
    public interface IDinhMucDuocPhamTonKhoService : IMasterFileService<Core.Domain.Entities.DinhMucDuocPhamTonKhos.DinhMucDuocPhamTonKho>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetTenKhoDuocPhamAsync(DropDownListRequestModel queryInfo);
        Task<List<VatTuDropdownTemplateVo>> GetListDuocPham(DropDownListRequestModel queryInfo);
        Task<bool> IsTenExists(long duocPhamId = 0 ,long Id = 0, long khoDuocPhamId = 0);
        Task<bool> CheckHoatChatExist(long id);
        Task<bool> CheckHieuLuc(long id);


    }
}
