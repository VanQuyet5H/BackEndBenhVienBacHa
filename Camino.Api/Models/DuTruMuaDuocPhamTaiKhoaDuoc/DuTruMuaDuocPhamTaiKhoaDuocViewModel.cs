using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DuTruMuaDuocPhamTaiKhoaDuoc
{
    public class DuTruMuaDuocPhamTaiKhoaDuocViewModel : BaseViewModel
    {
        public DuTruMuaDuocPhamTaiKhoaDuocViewModel()
        {
            thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = new List<DuTruMuaDuocPhamKhoDuocChiTietVM>();
        }
        public string SoPhieu { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? GiamDocDuyet { get; set; }
        public long? GiamDocId { get; set; }
        public DateTime? NgayGiamDocDuyet { get; set; }
        public string LyDoGiamDocTuChoi { get; set; }
        public List<long> ListDuTruDuocPhamId { get; set; }
        public List<long> ListDuTruDuocPhamTheoKhoaId { get; set; }
        public List<DuTruMuaDuocPhamKhoDuocChiTietVM> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList { get; set; }
}
    public class DuTruMuaDuocPhamKhoDuocChiTietVM : BaseViewModel
    {
        public DuTruMuaDuocPhamKhoDuocChiTietVM()
        {
            thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild = new List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamListChild>();
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
        public int SLDuTruTKhoDuyet { get; set; }
        public int SLDuTruKhoDuocDuyet { get; set; }
        public long DuocPhamDuTruTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamDuTruId { get; set; }

        // 
        public long DuTruMuaDuocPhamKhoaDuId { get; set; }
        public long DuTruMuaDuocPhamKhoId { get; set; }
        public long KyDuTruMuaDuocPhamVatTuId { get; set; }
        public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamListChild> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild { get; set; }
    }
    public class ThongTinChiTietTongHopDuTruTuaTaiKhoaDuocPhamListChild : BaseViewModel
    {
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuTruMuaDuocPhamChiTietId { get; set; }
        public string Nhom { get; set; }
        public string Kho { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string KyDuTru { get; set; }
        public int SLDuTru { get; set; }
        public int SLDuKienTrongKy { get; set; }
    }
    public class HuyDuyetQueryInfo
    {
        public long Id { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
    }
    public class TuChoiQueryInfo
    {
        public long Id { get; set; }
        public string LyDoTuChoi { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocDuyet : BaseViewModel
    {
        public DuTruMuaDuocPhamTaiKhoaDuocDuyet()
        {
            ListDuTruMuaDuocPhamKhoDuocChiTiet = new List<DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet>();
        }
        public bool LoaiDuyet { get; set; }
        public string LyDoTruongKhoaTuChoi { get; set; }
        public List<DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet> ListDuTruMuaDuocPhamKhoDuocChiTiet { get; set; }
    }
    public class DuTruMuaDuocPhamTaiKhoaDuocDuyetChiTiet  : BaseViewModel
    {
        public long? DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        //public EnumNhomDieuTriDuPhong? NhomDieuTriDuPhong { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocChiTietId { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public int? SoLuongDuTruKhoDuocDuyet { get; set; }
    }
}
