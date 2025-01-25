using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<LookupItemVo> GetTatCaKhoPhieuXuatHoaChat(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetTenMayXetNghiem(DropDownListRequestModel queryInfo);
        Task<List<LookupItemDuocPhamVo>> GetTenDuocPhamTheoKhoXuat(DropDownListRequestModel queryInfo, long? khoId);

        Task<GridDataSource> GetDataPhieuXuatHoaChatForGrid(PhieuXuatHoaChatQueryInfo queryInfo);
        Task<GridDataSource> GetDataPhieuXuatHoaChatChiTietForGrid(PhieuXuatHoaChatQueryInfo queryInfo);
        byte[] ExportPhieuXuatHoaChat(GridDataSource gridDataSource, PhieuXuatHoaChatQueryInfo query);
    }
}
