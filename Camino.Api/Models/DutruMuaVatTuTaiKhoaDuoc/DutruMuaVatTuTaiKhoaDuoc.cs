using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DutruMuaVatTuTaiKhoaDuoc
{
    public class DutruMuaVatTuTaiKhoaDuoc : BaseViewModel
    {
        public class DuTruMuaVatTuTaiKhoaDuocViewModel : BaseViewModel
        {
            public DuTruMuaVatTuTaiKhoaDuocViewModel()
            {
                thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList = new List<DuTruMuaVatTuKhoDuocChiTietVM>();
            }
            public string SoPhieu { get; set; }
            public long NhanVienYeuCauId { get; set; }
            public DateTime NgayYeuCau { get; set; }
            public long KyDuTruMuaVatTuVatTuId { get; set; }
            public DateTime TuNgay { get; set; }
            public DateTime DenNgay { get; set; }
            public string GhiChu { get; set; }
            public bool? GiamDocDuyet { get; set; }
            public long? GiamDocId { get; set; }
            public DateTime? NgayGiamDocDuyet { get; set; }
            public string LyDoGiamDocTuChoi { get; set; }
            public List<long> ListDuTruVatTuId { get; set; }
            public List<long> ListDuTruVatTuTheoKhoaId { get; set; }
            public List<DuTruMuaVatTuKhoDuocChiTietVM> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocGoiList { get; set; }
        }
        public class DuTruMuaVatTuKhoDuocChiTietVM : BaseViewModel
        {
            public DuTruMuaVatTuKhoDuocChiTietVM()
            {
                thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild = new List<ThongTinChiTietTongHopDuTruTuaTaiKhoaVatTuListChild2>();
            }
            public long DuTruMuaVatTuKhoDuocId { get; set; }
            public long DuTruMuaVatTuTheoKhoaChiTietId { get; set; }
            public long DuTruMuaVatTuChiTietId { get; set; }
            public string Khoa { get; set; }
            public bool Loai { get; set; }
            public long VatTuId { get; set; }
            public string TenVatTu { get; set; }
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
            public long VatTuDuTruTheoKhoaId { get; set; }
            public long DuTruMuaVatTuTheoKhoaId { get; set; }
            public long DuTruMuaVatTuId { get; set; }
            public long VatTuDuTruId { get; set; }

            // 
            public long DuTruMuaVatTuKhoaDuId { get; set; }
            public long DuTruMuaVatTuKhoId { get; set; }
            public long KyDuTruMuaVatTuVatTuId { get; set; }
            public List<ThongTinChiTietTongHopDuTruTuaTaiKhoaVatTuListChild2> thongTinChiTietTongHopDuTruTuaTaiKhoaDuocListChild { get; set; }
        }
        public class ThongTinChiTietTongHopDuTruTuaTaiKhoaVatTuListChild2 : BaseViewModel
        {
            public long DuTruMuaVatTuTheoKhoaId { get; set; }
            public long DuTruMuaVatTuTheoKhoaChiTietId { get; set; }
            public long DuTruMuaVatTuId { get; set; }
            public long DuTruMuaVatTuChiTietId { get; set; }
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
        public class DuTruMuaVatTuTaiKhoaDuocDuyet : BaseViewModel
        {
            public DuTruMuaVatTuTaiKhoaDuocDuyet()
            {
                ListDuTruMuaVatTuKhoDuocChiTiet = new List<DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet>();
            }
            public bool LoaiDuyet { get; set; }
            public string LyDoTruongKhoaTuChoi { get; set; }
            public List<DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet> ListDuTruMuaVatTuKhoDuocChiTiet { get; set; }
        }
        public class DuTruMuaVatTuTaiKhoaDuocDuyetChiTiet : BaseViewModel
        {
            public long? DuTruMuaVatTuId { get; set; }
            public long VatTuId { get; set; }
            public bool LaVatTuBHYT { get; set; }
            public int SoLuongDuTru { get; set; }
            public int SoLuongDuKienSuDung { get; set; }
            public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
            //public EnumNhomDieuTriDuPhong? NhomDieuTriDuPhong { get; set; }
            public long? DuTruMuaVatTuTheoKhoaChiTietId { get; set; }
            public long? DuTruMuaVatTuKhoDuocChiTietId { get; set; }
            public long? DuTruMuaVatTuTheoKhoaId { get; set; }
            public int? SoLuongDuTruKhoDuocDuyet { get; set; }
        }
    }
}
