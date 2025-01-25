using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuTruMuaKSNKTaiKhoa
{
    public class DuTruMuaKSNKTaiKhoaGridVo : GridItem
    {
        public string TrangThai { get; set; }
        public string SoPhieu { get; set; }
        public string TenKho { get; set; }
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
        public string LoaiNhom { get; set; }
        public string TenKhoa { get; set; }
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

    public class DuTruMuaKSNKTaiKhoaSearch
    {
        public bool ChoDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }

        public RangeDate RangeYeuCau { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }
    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class DuTruMuaKSNKViewModel
    {
        public string SoPhieu { get; set; }
        public long KhoNhapId { get; set; }
        public string TenKho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");
        public string TenNhanVienYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }

        public string TenNhanVienKhoDuocDuyet { get; set; }
        public DateTime? NgayKhoDuocDuyet { get; set; }
        public string NgayKhoDuocDuyetDisplay => NgayKhoDuocDuyet?.ApplyFormatDateTimeSACH();
        public int TinhTrang { get; set; }
        public string TenGiamDocDuyet { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public string NgayGiamDocDuyetDisplay => NgayGiamDocDuyet?.ApplyFormatDateTimeSACH();
        public string LyDoGiamDocTuChoi { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }
    }

    public class DuTruMuaKSNKTaiKhoaChiTietVo : GridItem
    {
        public DuTruMuaKSNKTaiKhoaChiTietVo()
        {
            ThongTinChiTietTonKhoTongs = new List<Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho>();
            ThongTinChiTietTonHDTs = new List<Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho>();
        }
        public long VatTuId { get; set; }
        public string Nhom => LaBHYT ? "BHYT" : "Không BHYT";
        public int STT { get; set; }
        public string KeyId { get; set; }
        public string VatTu { get; set; }
       
        public string TenVatTu { get; set; }
        public string HangSanXuat { get; set; }
        public bool LaBHYT { get; set; }
        public string TenKho { get; set; }
        public string KyDuTruDisplay { get; set; }
        public string DonViTinh { get; set; }
        public string QuyCach { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongDuTru { get; set; }
        public double SoLuongDuKienSuDung { get; set; }
        public double KhoDuTruTon { get; set; }
        public double KhoTongTon { get; set; }
        public double HDChuaNhap { get; set; }
        public long DuTruMuaVatTuTheoKhoaId { get; set; }

        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public long DuTruMuaVatTuId { get; set; }
        public List<long> DuTruMuaVatTuIds { get; set; }
        public long DuTruMuaVatTuChiTietId { get; set; }
        public List<long> DuTruMuaVatTuChiTietIds { get; set; }

        public List<Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho> ThongTinChiTietTonKhoTongs { get; set; }
        public List<Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham.ThongTinChiTietTonKho> ThongTinChiTietTonHDTs { get; set; }
    }

    public class ThongTinChiTietTonKho
    {
        public string Ten { get; set; }
        public double? SLTon { get; set; }
        public string SoLuongTon => SLTon.GetValueOrDefault().ApplyNumber();
    }

    public class ThongTinDuTruMuaKSNKChiTietTaiKhoa : GridItem
    {
        public string TenKho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru => TuNgay.ToString("dd/MM/yyyy") + " - " + DenNgay.ToString("dd/MM/yyyy");
        public double SoLuongDuTru { get; set; }
        public double SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
    }

    public class DuyetDuTruMuaKSNKViewModel
    {
        public long DuTruMuaVatTuId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KhoaPhongId { get; set; }
        public string GhiChu { get; set; }
        public List<DuTruMuaKSNKTaiKhoaChiTietVo> DuTruMuaVatTuTaiKhoaChiTietVos { get; set; }
    }

    public class PhieuInDuTruMuaKSNKTaiKhoa
    {
        public long? DuTruMuaVatTuTheoKhoaId { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }
        
        public string HostingName { get; set; }
        public bool Header { get; set; }
    }

    public class ThongTinLyDoHuyDuyetKSNKTaiKhoa
    {
        public long Id { get; set; }
        public string LyDoHuy { get; set; }
    }

    public class GetThongTinGoiKSNKTaiKhoa
    {
        public long KhoaPhongId { get; set; }
        public string TenKhoaPhong { get; set; }
        public long NguoiyeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
    }

    public class DuTruMuaKSNKTaiKhoaGridVoDetail : GridItem
    {
        public string SoPhieu { get; set; }
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
}
