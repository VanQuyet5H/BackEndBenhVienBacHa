using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        //bool IsDaChiDinhBacSiVaGiuong(long yeuCauTiepNhanId);
        bool IsDaChiDinhBacSi(long yeuCauTiepNhanId);
        bool IsDaChiDinhGiuong(long yeuCauTiepNhanId);
        bool IsDichVuGiuongAvailable(long dichVuGiuongId, DateTime thoiGianNhan);
        bool IsGiuongAvailable(long giuongId, long? yeuCauDichVuGiuongBenhVienId, DateTime? thoiGianNhan, DateTime? thoiGianTra);
        DateTime GetThoiDiemNhapVien(long yeuCauTiepNhanId);
        Task<List<ChanDoanICD>> GetChanDoanICD(DropDownListRequestModel model);

        Task<long> GetFirstEkipDieuTriId(long noiTruBenhAnId);
        Task<GridDataSource> GetDanhSachEkipDieuTriForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachEkipDieuTriForGrid(QueryInfo queryInfo);
        bool KiemTraTonTaiLichBacSiDieuTriTuNgay(DateTime? tuNgay, long noiTruBenhAnId, long noiTruEkipDieuTriId);
        //bool KiemTraTonTaiLichBacSiDieuTriDenNgay(DateTime? denNgay, long noiTruBenhAnId, long noiTruEkipDieuTriId);
        bool KiemTraTuNgayVoiNgayNhapVien(DateTime? tuNgay, long noiTruBenhAnId);
        List<NoiTruEkipDieuTri> XuLyThemBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri);
        List<NoiTruEkipDieuTri> XuLySuaBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri);
        List<NoiTruEkipDieuTri> XuLyXoaBacSiDieuTri(NoiTruEkipDieuTri noiTruEkipDieuTri);

        Task<List<ChiPhiKhamChuaBenhNoiTruVo>> GetChiPhiGiuongNoiTrus(long yeuCauTiepNhanId);
        Task<long> GetFirstYeuCauDichVuGiuongBenhVienId(long yeuCauTiepNhanId);
        Task<GridDataSource> GetDanhSachCapGiuongForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachCapGiuongForGrid(QueryInfo queryInfo);
        ChiTietSuDungGiuongTheoNgayVo GetDanhSachChiTietSuDungGiuongTheoNgayForGrid(long yeuCauTiepNhanId);
        Task<List<LookupItemVo>> GetListDichVuGiuongBenhVien(DropDownListRequestModel queryInfo);
        LookupItemVo GetLoaiGiuong(EnumLoaiGiuong loaiGiuong);
        List<LookupItemVo> GetListGiuongChoChiTietSuDungTheoNgay(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetListDoiTuongSuDung(DropDownListRequestModel queryInfo);
        Task SuaDanhSachChiTietSuDungGiuongTheoNgay(YeuCauTiepNhan yeuCauTiepNhan, ChiTietSuDungGiuongTheoNgayVo chiTietSuDungGiuongTheoNgayVo);
        Task XuLyThemCapGiuong(YeuCauTiepNhan yeuCauTiepNhanChiTiet, CapGiuongVo capGiuongVo);
        //Task XuLySuaCapGiuong(YeuCauTiepNhan yeuCauTiepNhan, CapGiuongVo capGiuongVo);
        Task XuLySuaHoatDongGiuong(YeuCauTiepNhan yeuCauTiepNhan, CapGiuongVo capGiuongVo);
        List<YeuCauDichVuGiuongBenhVien> XuLySuaCapGiuong(CapGiuongVo capGiuongVo, DoiTuongSuDung? oldDoiTuongSuDung);
        List<YeuCauDichVuGiuongBenhVien> XuLyXoaCapGiuong(YeuCauDichVuGiuongBenhVien yeuCauDichVuGiuongBenhVien);
        void XuLyThoiGianTraGiuong(YeuCauTiepNhan yeuCauTiepNhan, DateTime thoiDiemBatDauSuDung, DateTime? thoiDiemKetThucSuDung, DoiTuongSuDung? doiTuongSuDung, long currentYeuCauDichVuGiuongBenhVienId);
        //bool KiemTraTonTaiLichCapGiuongThoiGianNhan(DateTime? thoiGianNhan, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId);
        //bool KiemTraTonTaiLichCapGiuongThoiGianTra(DateTime? thoiGianTra, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId);
        //bool KiemTraTonTaiBenhNhanTrongLichChuyenPhong(DateTime? thoiGianNhan, DateTime? thoiGianTra, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId);
        bool KiemTraThoiGianNhanTonTaiBenhNhanTrongLichChuyenPhong(DateTime? thoiGianNhan, DoiTuongSuDung? doiTuongSuDung, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId);
        //bool KiemTraThoiGianTraTonTaiBenhNhanTrongLichChuyenPhong(DateTime? thoiGianTra, DoiTuongSuDung? doiTuongSuDung, long yeuCauTiepNhanId, long yeuCauDichVuGiuongBenhVienId);
        bool KiemTraThoiGianNhanGiuongVoiNgayNhapVien(DateTime? thoiGianNhan, long noiTruBenhAnId);
        ThoiDiemNhanGiuongVo GetThoiDiemChiDinhGiuong(long yeuCauTiepNhanId);
        ThongTinGiaDichVuGiuongVo GetDonGiaDichVuGiuong(long yeuCauTiepNhanId, long dichVuGiuongId, DateTime ngayPhatSinh, bool? baoPhong);
        bool KiemTraThoiGianNhanGiuongVoiThoiDiemChiDinhGiuong(DateTime? thoiGianNhan, long noiTruBenhAnId);

        Task<long> GetFirstKhoaPhongDieuTriId(long noiTruBenhAnId);
        Task<NoiTruKhoaPhongDieuTri> GetLastKhoaPhongDieuTri(long noiTruBenhAnId);
        Task<GridDataSource> GetDanhSachChuyenKhoaForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachChuyenKhoaForGrid(QueryInfo queryInfo);
        bool KiemTraTonTaiLichChuyenPhong(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId, long noiTruKhoaPhongDieuTriId);
        bool KiemTraThoiDiemVaoKhoaVoiNgayNhapVien(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId);
        bool KiemTraKhoaChuyenDenKhacKhoaChuyenDi(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId, long? khoaChuyenDenId, long id);
        bool KiemTraTonTaiLichDieuTri(DateTime? thoiDiemVaoKhoa, long noiTruBenhAnId);
        bool KiemTraSuaTrongPhamViBanDau(NoiTruKhoaPhongDieuTri newNoiTruKhoaPhongDieuTri);
        List<NoiTruKhoaPhongDieuTri> XuLyThemKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri);
        List<NoiTruKhoaPhongDieuTri> XuLySuaKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri);
        List<NoiTruKhoaPhongDieuTri> XuLyXoaKhoaPhongDieuTri(NoiTruKhoaPhongDieuTri noiTruKhoaPhongDieuTri);
        Task<NoiTruKhoaPhongDieuTri> GetCurrentNoiTruKhoaPhongDieuTri(long noiTruBenhAnId);
        Task<KhoaPhongChuyenDen> GetCurrentKhoaHienTaiBenhNhanChuyenDen(long noiTruBenhAnId);
        Task<List<LookupItemVo>> GetDanhSachKhoaChuyenDen(DropDownListRequestModel model);

        bool CompareTuNgayDenNgay(DateTime? tuNgay, DateTime? denNgay);
        bool KiemTraDichVuCoTrongGoi(long benhNhanId, long? dichVuGiuongBenhVienId);
    }
}