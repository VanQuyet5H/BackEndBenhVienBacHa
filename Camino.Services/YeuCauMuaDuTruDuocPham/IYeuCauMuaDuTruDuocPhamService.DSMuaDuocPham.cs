using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    public partial interface IYeuCauMuaDuTruDuocPhamService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        List<LookupItemVo> NhomDuocPhamDuTru(DropDownListRequestModel queryInfo);
        List<LookupItemVo> NhomDuocPhamDieuTriDuPhong(DropDownListRequestModel queryInfo);

        Task<List<LookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo, bool? laDuocPham = null);
        Task<List<LookupItemVo>> GetKyDuTru(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamTemplateLookupItem>> GetDuocPhamMuaDuTrus(DropDownListRequestModel queryInfo);
        ThongTinDuTruMuaDuocPham ThongTinDuTruMuaDuocPham(ThongTinChiTietDuocPhamTonKho thongTinChiTietDuocPhamTonKho);
        Task<TrangThaiDuyetDuTruMuaVo> GetTrangThaiPhieuMua(long phieuMuaId);
        Task<bool> CheckDuocPhamExists(long? duocPhamId, bool? laDuocPhamBHYT, List<DuocPhamDuTruViewModelValidator> duocPhams);
        Task<KyDuTruMuaDuocPhamVatTuVo> GetKyDuTruMuaDuocPhamVatTu(long kyDuTruMuaDuocPhamVatTuId);
        Task<string> GetSoPhieuDuTru();
        string InPhieuMuaDuTruDuocPham(PhieuMuaDuTruDuocPham phieuMuaDuTruDuocPham);


    }
}
