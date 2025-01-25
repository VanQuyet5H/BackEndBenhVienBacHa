using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DinhMucVatTuTonKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DinhMucDuocPhamTonKho;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DinhMucVatTuTonKhos
{
    public interface IDinhMucVatTuTonKhoService : IMasterFileService<DinhMucVatTuTonKho>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetListKhoAsync(DropDownListRequestModel queryInfo);

        Task<List<VatTuDinhMucVo>> GetListVatTu(DropDownListRequestModel queryInfo);

        Task<bool> IsTenVatTuExists(long vatTuId = 0, long id = 0, long khoId = 0);

        Task<bool> CheckVatTuExist(long id);

        Task<bool> CheckHieuLuc(long id);
    }
}
