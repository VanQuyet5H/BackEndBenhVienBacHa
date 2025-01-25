using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham;
using Camino.Core.Domain.ValueObject.YeuCauMuaKSNK;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    public partial interface IYeuCauMuaDuTruKiemSoatNhiemKhuanService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<List<KSNKTemplateLookupItem>> GetKSNKMuaDuTrus(DropDownListRequestModel queryInfo);
        ThongTinDuTruMuaKSNK ThongTinDuTruMuaKSNK(ThongTinChiTietKSNKTonKho thongTinChiTietVatTuTonKho);
        Task<TrangThaiDuyetDuTruMuaVo> GetTrangThaiPhieuMuaKSNK(long phieuMuaId);
        Task<bool> CheckVatTuExists(long? vatTuId, bool? laVatTuBHYT, List<KSNKDuTruViewModelValidator> vatTus);

        string InPhieuMuaDuTruKSNK(PhieuMuaDuTruKSNK phieuMuaDuTruVatTu);
        string InPhieuMuaDuTruDuocPham(PhieuMuaDuTruKSNK phieuMuaDuTruDuocPham);

        Task<List<LookupItemVo>> GetKyDuTruKSNK(DropDownListRequestModel queryInfo);
        Task<List<LookupTreeItemVo>> GetNhomKSNKTreeView(DropDownListRequestModel model);
        string GetSoPhieuDuTruKSNK();
    }
}
