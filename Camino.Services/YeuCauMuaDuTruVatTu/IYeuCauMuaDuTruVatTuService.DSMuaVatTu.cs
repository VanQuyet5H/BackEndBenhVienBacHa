using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruVatTu
{
    public partial interface IYeuCauMuaDuTruVatTuService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<VatTuTemplateLookupItem>> GetVatTuMuaDuTrus(DropDownListRequestModel queryInfo);
        ThongTinDuTruMuaVatTu ThongTinDuTruMuaVatTu(ThongTinChiTietVatTuTonKho thongTinChiTietVatTuTonKho);
        Task<TrangThaiDuyetDuTruMuaVo> GetTrangThaiPhieuMuaVatTu(long phieuMuaId);
        Task<bool> CheckVatTuExists(long? vatTuId, bool? laVatTuBHYT, List<VatTuDuTruViewModelValidator> vatTus);
        string InPhieuMuaDuTruVatTu(PhieuMuaDuTruVatTu phieuMuaDuTruVatTu);
        Task<List<LookupItemVo>> GetKyDuTruVatTu(DropDownListRequestModel queryInfo);
        Task<List<LookupTreeItemVo>> GetNhomVatTuTreeView(DropDownListRequestModel model);
        Task<string> GetSoPhieuDuTruVatTu();
    }
}
