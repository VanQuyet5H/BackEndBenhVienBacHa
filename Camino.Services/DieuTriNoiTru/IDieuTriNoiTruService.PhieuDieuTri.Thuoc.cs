using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        GridDataSource GetDataForGridDanhSachThuocKhoLe(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachThuocKhoLe(QueryInfo queryInfo);
        GridDataSource GetDataForGridDanhSachThuocKhoTong(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridDanhSachThuocKhoTong(QueryInfo queryInfo);
        void ApDungThoiGianDienBienChiDinhDuocPham(List<long> chiDinhDuocPhamIds, DateTime? thoiGianDienBien);
        Task<List<KhoLookupItemVo>> GetKhoCurrentUser(DropDownListRequestModel queryInfo);
        GetDuocPhamTonKhoGridVoItem GetDuocPhamInfoById(ThongTinThuocDieuTriVo thongTinThuocVo);
        Task ThemThuoc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task CapNhatThuoc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        //Task SapXepThuoc(SapXepThuoc sapXepThuoc, YeuCauTiepNhan yeuCauTiepNhan);
        Task SapXepThuoc(SapXepThuoc sapXepThuoc);

        Task<string> CapNhatThuocChoTuTruc(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task XoaThuoc(long noiTruChiDinhDuocPhamId, YeuCauTiepNhan yeuCauTiepNhan);
        //Task<string> XuLySoThuTu(ThuocBenhVienVo donThuocChiTiet, YeuCauTiepNhan yeuCauTiepNhan);
        Task<string> TangHoacGiamSTTDonThuocChiTiet(ThuocBenhVienTangGiamSTTVo donThuocChiTietn);

        string GetResourceValueByResourceName(string ten);
        string InPhieuCongKhaiThuocVatTu(InPhieuCongKhaiThuocVatTuReOrder inToaThuoc);
        string InPhieuThucHienThuocVatTu(InPhieuThucHienThuocVatTu inToaThuoc);

        Task<string> HoanTraDuocPhamTuBenhNhan(YeuCauTraDuocPhamTuBenhNhanChiTietVo yeuCauTraDuocPham);
        //Task<ThongTinHoanTraThuocVo> GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo);
        ThongTinHoanTraThuocVo GetThongTinHoanTraThuocVo(HoanTraThuocVo hoanTraThuocVo);

        Task<GridDataSource> GetDataForGridDanhSachThuocHoanTra(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDanhSachThuocHoanTra(QueryInfo queryInfo);
        Task<bool> KiemTraNgayTra(DateTime? ngayTra);
        Task<bool> CheckSoLuongTra(double soLuong, double soLuongDaTra, double? soLuongTra);
        Task ThemThuocNgoaiBenhVien(ThuocBenhVienVo donThuocChiTiet);
        Task CapNhatThuocNgoaiBenhVien(ThuocBenhVienVo donThuocChiTiet);
        Task XoaThuocNgoaiBenhVien(long noiTruPhieuDieuTriTuVanThuocId);
        GridDataSource GetDataForGridDanhSachThuocNgoai(QueryInfo queryInfo);
        GridDataSource GetDataForGridDanhSachDichTruyenNgoai(QueryInfo queryInfo);
        string InNoiTruPhieuDieuTriTuVanThuoc(InNoiTruPhieuDieuTriTuVanThuoc inNoiTruPhieuDieuTriTuVanThuoc);
        Task CapNhatKhongTinhPhi(CapNhatKhongTinhPhi capNhatKhongTinhPhi, YeuCauTiepNhan yeuCauTiepNhan);
        Task XuLyXoaYLenhKhiXoaDichVuNoiTruAsync(EnumNhomGoiDichVu loaiDichVu, long yeuCauDichVuId);
        Task CreateNewDateDieuTri(YeuCauTiepNhan yeuCauTiepNhan, long? khoaId, List<DateTime> dates);
        Task<KetQuaApDungNoiTruDonThuocTongHopVo> ApDungDonThuocChoCacNgayDieuTriAsync(NoiTruDonThuocTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan);
        Task<string> ApDungDonThuocChoCacNgayDieuTriConfirmAsync(NoiTruDonThuocTongHopVo model, YeuCauTiepNhan yeuCauTiepNhan);

        Task<DateTime?> GetNgayNhapVien(long yeuCauTiepNhanId);
        Task<long> GetNoiTruKhoaChuyenDen(long yeuCauTiepNhanId);

    }
}
