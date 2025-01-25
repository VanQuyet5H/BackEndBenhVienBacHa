using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCao;
using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<List<LookupItemVo>> GetTatCaKhoaThongKeSoLuongThuThuat(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetTatCaPhongTheoKhoaThongKeSoLuongThuThuat(DropDownListRequestModel queryInfo, long khoaId);
        Task<GridDataSource> GetDataKHTHBaoCaoThongKeSLThuThuatForGridAsync(KHTHBaoCaoThongKeSLThuThuatQueryInfo queryInfo, bool exportExcel = false);
        byte[] ExportKHTHBaoCaoThongKeSLThuThuatGridVo(GridDataSource gridDataSource, KHTHBaoCaoThongKeSLThuThuatQueryInfo query);
    }
}