using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using Camino.Core.Domain.ValueObject.YeuCauMuaVatTu;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachVatTu(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachVatTu(QueryInfo queryInfo);
        GridDataSource GetDataForGridDanhSachVatTuKhoTong(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachVatTuKhoTong(QueryInfo queryInfo);
        Task<List<KhoLookupItemVo>> GetKhoVatTuCurrentUser(DropDownListRequestModel queryInfo);
        ThongTinPhieuDieuTriVatTu GetVatTuInfoById(ThongTinChiTietVatTuTonKhoPDT thongTinVatTuVo);
        Task ThemVatTu(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task CapNhatVatTu(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task XoaVatTu(string ids, YeuCauTiepNhan yeuCauTiepNhan);

        Task<string> CapNhatVatTuChoTuTruc(VatTuBenhVienVo donVatTuChiTiet, YeuCauTiepNhan yeuCauTiepNhan);

        Task<CoDonThuocKhoLeKhoTong> KiemTraCoDonVT(long noiTruPhieuDieuTriId);
        Task<ThongTinHoanTraVTVo> GetThongTinHoanTraVatTu(HoanTraVTVo hoanTraVTVo);
        Task<GridDataSource> GetDataForGridDanhSachVatTuHoanTra(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDanhSachVatTuHoanTra(QueryInfo queryInfo);
        Task<string> HoanTraVatTuTuBenhNhan(YeuCauTraVatTuTuBenhNhanChiTietVo yeuCauTraVT);
        Task<List<DichVuKyThuatDaThemLookupItem>> GetDichVuKyThuatDaThem(DropDownListRequestModel queryInfo);

        Task<KetQuaApDungNoiTruDonVTYTTongHopVo> ApDungDonVTYTChoCacNgayDieuTriAsync(NoiTruDonVTYTTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan);
        Task<string> ApDungDonVTYTChoCacNgayDieuTriConfirmAsync(NoiTruDonVTYTTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan);

    }
}
