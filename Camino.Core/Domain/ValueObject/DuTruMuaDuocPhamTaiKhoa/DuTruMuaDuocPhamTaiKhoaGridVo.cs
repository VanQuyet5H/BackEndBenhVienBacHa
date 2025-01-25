using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoa
{
    public class ThongTinLyDoHuyDuyetTaiKhoa
    {
        public long Id { get; set; }
        public string LyDoHuy { get; set; }
    }

    public class DuTruMuaDuocPhamTaiKhoaGridVo : GridItem
    {
        public string LoaiNhom { get; set; }
        public string TrangThai { get; set; }
        public string SoPhieu { get; set; }
        public string TenKho { get; set; }
        public string TenKhoa { get; set; }

        public string NguoiYeuCau { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");

        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayTruongKhoaDuyet { get; set; }

        public DateTime? NgayTuChoi { get; set; }
        public string NgayTuChoiDisplay => NgayTuChoi?.ApplyFormatDateTimeSACH();
        public string NguoiTuChoi { get; set; }
        public string LyDoTuChoi { get; set; }
        public string TuNgayDisplay => TuNgay.ApplyFormatDateTimeSACH();
        public string DenNgayDisplay => DenNgay.ApplyFormatDateTimeSACH();
        public string NgayYeuCauDisplay => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NgayTruongKhoaDuyetDisplay => NgayTruongKhoaDuyet?.ApplyFormatDateTimeSACH();
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public RangeDates RangeFromDateDaXuLy { get; set; }
        public RangeDates RangeFromDateTuChoi { get; set; }

        public bool? DaGuiChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? DaDuyetDaXuLy { get; set; }
        public bool? TuChoiDuyet { get; set; }
        public string SearchString { get; set; }
        public string SearchStringDaXuLy { get; set; }
        public string SearchStringTuChoi { get; set; }
        public int? TinhTrang { get; set; }
        public DateTime? NgayKhoDuocDuyet { get; set; }
        public string NgayKhoDuocDuyetDisplay => NgayKhoDuocDuyet?.ApplyFormatDateTimeSACH();

        public DateTime? NgayGiamDocDuyet { get; set; }
        public string NgayGiamDocDuyetDisplay => NgayGiamDocDuyet?.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
    }


    public class GetThongTinGoiTaiKhoa
    {
        public long KhoaPhongId { get; set; }
        public string TenKhoaPhong { get; set; }
        public long NguoiyeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
    }

    public class DuTruMuaDuocPhamTaiKhoaChiTietVo : GridItem
    {
        public long DuocPhamId { get; set; }
        public string Nhom
        {
            get { return LaBHYT ? "BHYT" : "Không BHYT"; }
        }
        public int STT { get; set; }
        public string TenNhomDieuTriDuPhong { get; set; }
        public string TenKho { get; set; }
        public string TenDuocPham { get; set; }
        public string KyDuTruDisplay { get; set; }
        public bool LaBHYT { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongDuTru { get; set; }
        public string SoLuongDuTruDisplay => SoLuongDuTru.ApplyNumber();

        public double SoLuongDuKienSuDung { get; set; }
        public string SoLuongDuKienSuDungDisplay => SoLuongDuKienSuDung.ApplyNumber();
        public string LoaiThuoc { get; set; }
        public string NhomDuPhong { get; set; }
        public double KhoDuTruTon { get; set; }
        public double KhoTongTon { get; set; }
        public double HDChuaNhap { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HamLuong => NongDoHamLuong;
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public string SoDangKy { get; set; }

        public long DuTruMuaDuocPhamId { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long KhoId { get; set; }
        public List<long> DuTruMuaDuocPhamIds { get; set; }
        public long DuTruMuaDuocPhamChiTietId { get; set; }
        public List<long> DuTruMuaDuocPhamChiTietIds { get; set; }

        public List<ThongTinChiTietTonKho> ThongTinChiTietTonKhoTongs { get; set; }
        public List<ThongTinChiTietTonKho> ThongTinChiTietTonHDTs { get; set; }

        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");
    }

    public class DuyetDuTruMuaDuocPhamViewModel
    {
        public long DuTruMuaDuocPhamId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KhoaPhongId { get; set; }
        public string GhiChu { get; set; }
        public List<DuTruMuaDuocPhamTaiKhoaChiTietVo> DuTruMuaDuocPhamTaiKhoaChiTietVos { get; set; }
    }

    public class DuTruMuaDuocPhamViewModel
    {
        public int TinhTrang { get; set; }

        public string SoPhieu { get; set; }
        public string TenLoaiDuTru { get; set; }
        public long KhoNhapId { get; set; }
        public string TenKho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public EnumNhomDuocPhamDuTru LoaiDuTru { get; set; }
        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");
        public string TenNhanVienYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }


        public string TenNhanVienKhoDuocDuyet { get; set; }
        public DateTime? NgayKhoDuocDuyet { get; set; }
        public string NgayKhoDuocDuyetDisplay => NgayKhoDuocDuyet?.ApplyFormatDateTimeSACH();

        public string TenGiamDocDuyet { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public string NgayGiamDocDuyetDisplay => NgayGiamDocDuyet?.ApplyFormatDateTimeSACH();
        public string LyDoGiamDocTuChoi { get; set; }

    }

    public class DuTruMuaDuocPhamTaiKhoaSearch
    {
        public bool ChoDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public DateTime? NgayYeuCauTuFormat { get; set; }
        public DateTime? NgayYeuCauDenFormat { get; set; }
    }

    public class ThongTinDuTruMuaChiTietTaiKhoa : GridItem
    {
        public string LoaiNhom { get; set; }
        public string TenKho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");
        public double SoLuongDuTru { get; set; }
        public double SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
    }

    public class PhieuInDuTruMuaTaiKhoa
    {      
        public long? DuTruMuaVatTuTheoKhoaId { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }

        public string HostingName { get; set; }
        public bool Header { get; set; }
    }

    public class DuTruMuaDuocPhamTaiKhoaGridVoDetail : GridItem
    {
        public string SoPhieu { get; set; }
        public string LoaiNhom { get; set; }
        public string TenKho { get; set; }
        public string KyDuTru => TuNgay.ApplyFormatDate() + " - " + DenNgay.ApplyFormatDate();
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NguoiYeuCau { get; set; }
        public DateTime? NgayTruongKhoaDuyet { get; set; }
        public string NgayTruongKhoaDuyetDisplay => NgayTruongKhoaDuyet?.ApplyFormatDateTimeSACH();
        public int? TinhTrang { get; set; }

    }

    public class ChiTietDaXuLyChild
    {
        public long DuTruMuaDuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }
}

