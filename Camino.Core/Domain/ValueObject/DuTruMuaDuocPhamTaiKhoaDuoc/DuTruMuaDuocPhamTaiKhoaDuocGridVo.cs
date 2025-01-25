using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc
{
    public class ThongTinLyDoHuyDuyetTaiKhoaDuoc
    {
        public long Id { get; set; }
        public string LyDoHuy { get; set; }
    }

    public class DuTruMuaDuocPhamTaiKhoaDuocGridVo : GridItem
    {
        public int DuTruMuaId { get; set; }
        public string LoaiNhom { get; set; }
        public int TrangThaiDuTru { get; set; }
        public string TrangThai { get; set; }
        public string SoPhieu { get; set; }
        public string KhoaKhoa { get; set; }
        public long KhoaIdKhoId { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public long KhoId { get; set; }
        public string NguoiYeuCau { get; set; }
        public string KyDuTru { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayKhoaDuocDuyet { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocId { get; set; }

        public string TuNgayDisplay => TuNgay.ApplyFormatDateTimeSACH();
        public string DenNgayDisplay => DenNgay.ApplyFormatDateTimeSACH();
        public string NgayYeuCauDisplay => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NgayKhoaDuocDuyetDisplay => NgayKhoaDuocDuyet?.ApplyFormatDateTimeSACH();
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocDaXuLy : GridItem
    {
        public string SoPhieu { get; set; }
        public string DuTruTheo { get; set; }
        public string NguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public string NgayYeuCauHienThi => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NgayGiamDocDuyetHienThi => NgayGiamDocDuyet != null ? NgayGiamDocDuyet.Value.ApplyFormatDateTimeSACH(): "";
        public EnumTrangThaiDuTruKhoaDuoc TinhTrang {get;set;}
        public string TinhTrangHienThi => TinhTrang.GetDescription();
        public string GhiChu { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocSearch
    {
        public bool ChoDuyet { get; set; }
        public bool ChoGoi { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocTuChoiSearch
    {
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocSearchDaXuLy
    {
        public bool DaGoiVaDangChoDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }
    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocChildGridVo : GridItem
    {
        public DuTruMuaDuocPhamTaiKhoaDuocChildGridVo()
        {
            TongTonList = new List<KhoTongTonDuocPham>();
            HopDongChuahapList = new List<HopDongChuaNhapDuoc>();
        }
        public long KhoId {get;set;}
        public bool LoaiKhoHayKhoa { get; set; }
        public string Loai { get; set; }
        public EnumTrangThaiLoaiDuTru TrangThai { get; set; }
        public bool LoaiDuocPham { get; set; }
        public string DuocPham { get; set; }
        public long DuocPhamId { get; set; }
        public string HoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string SDK { get; set; }
        public string DVT { get; set; }
        public string DD { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string NhomDieuTriDuPhong { get; set; }
        public int SLDuTru { get; set; }
        public double KhoDuTruTon { get; set; }
        public double KhoTongTon { get; set; }
        public double HDChuaNhap { get; set; }
        public double SLDuKienSuDungTrongKy { get; set; }
        public int? SLDuTruTKhoaDuyet { get; set; }
        public int? SLDuTruKDuocDuyet { get; set; }
        public long KhoaPhongId { get; set; }
        public List<KhoTongTonDuocPham> TongTonList { get; set; }
        public List<HopDongChuaNhapDuoc> HopDongChuahapList { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocChildChildGridVo : GridItem
    {
        public string Nhom { get; set; }
        public string Kho { get; set; }
        public string KyDuTru { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienSuDungTrongKy { get; set; }
        public string NhomDieuTriDuPhong { get; set; }

    }
    public class DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien : GridItem
    {
        public DuTruMuaDuocPhamTaiKhoaDuocChildNhaThuocBenhVien()
        {
            TongTonList = new List<KhoTongTonDuocPham>();
            HopDongChuahapList = new List<HopDongChuaNhapDuoc>();
        }
        public long KhoaPhongId { get; set; }
        public long KhoId { get; set; }
        public string Loai { get; set; }
        public bool LoaiKhoHayKhoa { get; set; }
        public EnumTrangThaiLoaiDuTru TrangThai { get; set; }
        public bool LoaiDuocPham { get; set; }
        public string DuocPham { get; set; }
        public long DuocPhamId { get; set; }
        public string HoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string SDK { get; set; }
        public string DVT { get; set; }
        public string DD { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public EnumNhomDieuTriDuPhong? NhomDieuTri { get; set; }
        public string NhomDieuTriDuPhong => NhomDieuTri != null ? NhomDieuTri.GetDescription() : "";
        public int SLDuTru { get; set; }
        public double KhoDuTruTon { get; set; }
        public double KhoTongTon { get; set; }
        public int SLDuKienSuDungTrongKy { get; set; }
        public int? SLDuTruTKhoaDuyet { get; set; }
        public double HDChuaNhap { get; set; }
        public List<KhoTongTonDuocPham> TongTonList { get; set; }
        public List<HopDongChuaNhapDuoc> HopDongChuahapList { get; set; }
    }
    public class KhoTongTonDuocPham
    {
        public string TenKhoTong { get; set; }
        public double TongTon { get; set; }
    }
    public class HopDongChuaNhapDuoc
    {
        public string TenHopDong { get; set; }
        public double TongTon { get; set; }
    }
    #region GET LIST DATA
    public class DuTruMuaDuocPhamChiTietViewGridVo : GridItem
    {
        public string TenLoaiDuTru { get; set; }
        public long LoaiDuTruId { get; set; }
        public string SoPhieu { get; set; }
        public string TenKhoa { get; set; }
        public string TenKho{ get; set; }
        public long KhoaId { get; set; }
        public long KhoId { get; set; }
        public long NguoiYeuCauId { get; set; }
        public string TenNguoiYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public EnumTrangThaiDuTruKhoaDuoc TrangThai { get; set; }
        public string TrangThaiHienThi { get; set; }
        public string LyDoTuChoi { get; set; }
        public string KyDuTru { get; set; }
   
    public string GhiChu { get; set; }
        public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocList { get; set; }
    }
    public class ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList : GridItem
    {
        public ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamList()
        {
            TongTonList = new List<KhoTongTonDuocPham>();
            HopDongChuahapList = new List<HopDongChuaNhapDuoc>();
        }
        public long? KhoaId { get; set; }
        public bool Loai { get; set; }
        public string TenLoai { get; set; }
        public long DuocPhamId { get; set; }
        public string TenDuocPham { get; set; }
        public string HoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string SDK { get; set; }
        public string DVT { get; set; }
        public string DD { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string NhomDieuTriDuPhong { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienSuDungTrongKho { get; set; }
        public double KhoDuTruTon { get; set; }
        public double HDChuaNhap { get; set; }
        public int? SLDuTruTKhoDuyet { get; set; }
        public int? SLDuTruKhoDuocDuyet { get; set; }
        public double KhoTongTon { get; set; }
        public long DuocPhamDuTruTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuocPhamDuTruId { get; set; }
        public List<KhoTongTonDuocPham> TongTonList { get; set; }
        public List<HopDongChuaNhapDuoc> HopDongChuahapList { get; set; }
        public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild { get; set; }
    }
    public class ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild : GridItem
    {
        public string Nhom { get; set; }
        public string Kho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienTrongKy { get; set; }
    }
    #endregion
    #region gridVo tu chối
    public class DuTruMuaDuocPhamTuChoiGridVo : GridItem
    {
        public string SoPhieu { get; set; }
        public string KhoaKho { get; set; }
        public string KyDuTru { get; set; }
        public string NguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauHienThi => NgayYeuCau.ApplyFormatDateTimeSACH();
        public DateTime? NgayTuChoi { get; set; }
        public string NgayTuChoiHienThi => NgayTuChoi != null ? NgayTuChoi.Value.ApplyFormatDateTimeSACH() :"";
        public string NguoiTuChoi { get; set; }
        public string LyDo { get; set; }
        public long? NguoiTuChoiId { get; set; }
        public long NguoiYeuCauId { get; set; }
        public long KhoaId { get; set; }
        public long KhoId { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public EnumTrangThaiLoaiDuTruDaXuLy TinhTrang { get; set; }
        public string TinhTrangHienThi => TinhTrang.GetDescription();
    }
    #endregion
    #region GET LIST DATA gởi
    public class DuTruMuaDuocPhamChiTietGoiViewGridVo : GridItem
    {
        //public string TenLoaiDuTru { get; set; }
        //public long LoaiDuTruId { get; set; }
        //public string SoPhieu { get; set; }
        //public string TenKhoa { get; set; }
        //public string TenKho { get; set; }
        //public long KhoaId { get; set; }
        //public long KhoId { get; set; }
        public long NguoiYeuCauId { get; set; }
        public string TenNguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public EnumTrangThaiDuTruKhoaDuoc TrangThai { get; set; }
        public string TrangThaiHienThi { get; set; }
        public string KyDuTru { get; set; }
        public long KyDuTruId { get; set; }
        public string GhiChu { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long DuTruDuocPhamId { get; set; }
        public long DuTruDuocPhamTheoKhoaId { get; set; }
        public List<long> ListDuTruDuocPhamId { get; set; }
        public List<long> ListDuTruDuocPhamTheoKhoaId { get; set; }
        public string LyDoTuChoi { get; set; }
        public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList { get; set; }
    }
    public class ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList : GridItem
    {
        public ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList()
        {
            thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild = new List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild>();
        }
        public long DuTruMuaDuocPhamKhoDuocId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long DuTruMuaDuocPhamChiTietId { get; set; }
        public string Khoa { get; set; }
        public bool Loai { get; set; }
        public long DuocPhamId { get; set; }
        public string TenDuocPham { get; set; }
        public string HoatChat { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string SDK { get; set; }
        public string DVT { get; set; }
        public string DD { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string NhomDieuTriDuPhong { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienSuDungTrongKho { get; set; }
        public int? SLDuTruTKhoDuyet { get; set; }
        public int? SLDuTruKhoDuocDuyet { get; set; }
        public long DuocPhamDuTruTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamDuTruId { get; set; }

        // 
        public long DuTruMuaDuocPhamKhoaDuId { get; set; }
        public long DuTruMuaDuocPhamKhoId { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }

        public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild { get; set; }
    }
    public class ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocListGoiChild : GridItem
    {
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuTruMuaDuocPhamChiTietId { get; set; }
        public string Nhom { get; set; }
        public string Khoa { get; set; }
        public string Kho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienTrongKy { get; set; }
        public string NhomDieuTriDuPhong { get; set; }
    }


    
    #endregion
    #region child da xu ly
    public class DuTruMuaDuocPhamKhoaDuocChild : GridItem
    {
        public string SoPhieu { get; set; }
        public string KhoaKhoaString  { get; set; }
        public long KhoId { get; set; }
        public long KhoaId { get; set; }
        public string KyDuTruTheo { get; set; }
        public long KyDuTruTheoId { get; set; }
        public string NguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauHienThi => NgayYeuCau.ApplyFormatDateTimeSACH();
        public DateTime? NgayKhoaDuocDuyet { get; set; }
        public long TinhTrang { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
        public long DuTruMuaDuocPhamKhoDuocId { get; set; }
        public string NgayKhoaDuocDuyetHienThi => NgayKhoaDuocDuyet != null ?NgayKhoaDuocDuyet.Value.ApplyFormatDateTimeSACH() :"";
    }
    public class DuTruMuaDuocPhamKhoaDuocChildChild : GridItem
    {
        public string Kho { get; set; }
        public string Nhom { get; set; }
        public string NhomDieuTri { get; set; }
        public string Khoa { get; set; }
        public string KyDuTru { get; set; }
        public int SLDuKienTrongKy { get; set; }
        public int SLDuTru { get; set; }
    }
    #endregion
}
