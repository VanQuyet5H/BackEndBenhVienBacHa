using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.PhauThuatThuThuat
{
    public partial interface IPhauThuatThuThuatService
    {
        YeuCauTiepNhan GetYeuCauTiepNhanForGhiNhanVatTuThuoc(long yeuCauTiepNhanId);
        Task<List<GhiNhanVatTuTieuHaoThuocGridVo>> GetGridDataGhiNhanVTTHThuocAsync(long yeuCauDichVuKyThuatId);
        Task<List<GhiNhanVatTuTieuHaoThuocGroupParentGridVo>> GetGridDataGhiNhanVTTHThuocAsyncVer2(long yeuCauDichVuKyThuatId);
        Task XuLyThemGhiNhanVatTuBenhVienAsync(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhanChiTiet);
        void XuLyXoaYeuCauGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, string yeuCauGhiNhanId);
        Task XuLyXuatYeuCauGhiNhanVTTHThuocAsync(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo);
        Task CapNhatGridItemGhiNhanVTTHThuocAsync(YeuCauTiepNhan yeuCauTiepNhanChiTiet, ChiDinhGhiNhanVatTuThuocPTTTVo ghiNhanVo);
        void XuLyXuatYeuCauGhiNhanVTTHThuocChoNhungYeuCauQuenNhanXuat(ChiDinhGhiNhanVatTuThuocPTTTVo yeuCauVo);
        Task<ThongTinHoanTraVatTuThuocPTTTVo> GetThongTinHoanTraVatTuThuocPTTT(HoanTraVatTuThuocVo hoanTraVatTuThuocVo);
        Task<GridDataSource> GetDataForGridDanhSachVatTuThuocHoanTraPTTT(QueryInfo queryInfo);
        Task XuLyHoanTraYeuCauGhiNhanVTTHThuocAsync(YeuCauHoanTraVatTuThuocPTTTVo yeuCauHoanTraVatTuThuoc);
        Task<List<KhoSapXepUuTienLookupItemVo>> GetListKhoGhiNhanPTTTAsync(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetGridDataGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPagesGoiDuocPhamVatTuTrongDichVu(QueryInfo queryInfo);
        Task<List<LookupItemVo>> GetKhoLeNhanVienQuanLy(DropDownListRequestModel queryInfo);
        Task XuLyThemGhiNhanThuocVatTusAsync(ChiDinhGhiNhanVatTuThuocPTTTTheoGoiDVKTVo model, YeuCauTiepNhan yeuCauTiepNhan);

        Task<long> GetYeuCauTiepNhanIdTheoYeuCauKyThuatIdAsync(long yeuCauDichVuKyThuatId);

    }
}