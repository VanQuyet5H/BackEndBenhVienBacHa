using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<LookupItemVo> GetTatCaKhoPhieuNhapHoaChat(DropDownListRequestModel queryInfo);
        Task<List<LookupItemDuocPhamVo>> GetTenDuocPhamTheoPhieuNhap(DropDownListRequestModel queryInfo, long? khoId);

        Task<GridDataSource> GetDataPhieuNhapHoaChatForGrid(PhieuNhapHoaChatQueryInfo queryInfo);
        Task<GridDataSource> GetDataPhieuNhapHoaChatChiTietForGrid(PhieuNhapHoaChatQueryInfo queryInfo);
        byte[] ExportPhieuNhapHoaChat(GridDataSource gridDataSource, PhieuNhapHoaChatQueryInfo query);
    }
}
