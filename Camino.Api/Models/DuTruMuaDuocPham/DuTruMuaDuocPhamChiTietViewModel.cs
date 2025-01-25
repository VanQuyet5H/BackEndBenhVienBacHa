using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DuTruMuaDuocPham
{
    public class DuTruMuaDuocPhamChiTietViewModel : BaseViewModel
    {
        public EnumNhomDieuTriDuPhong? NhomDieuTriDuPhong { get; set; }
        public string NhomDieuTriDuTru { get; set; }
        public long? DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public string Ten { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int? SoLuongDuTru { get; set; }
        public int? SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string TenDuongDung { get; set; }
        public long? DonViTinhId { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string GhiChu { get; set; }
        public string TenLoaiDuocPham { get; set; }

    }

    public class DuocPhamDuTruGridViewModel : BaseViewModel
    {
        public DuocPhamDuTruGridViewModel()
        {
            YeuCauMuaThuocChiTietValidators = new List<DuocPhamDuTruViewModelValidator>();
        }
        public int LoaiDuocPham { get; set; }
        public string TenLoaiDuocPham { get; set; }
        public long? NhomDieuTriDuPhong { get; set; }
        public string NhomDieuTriDuTru { get; set; }
        public long? DuocPhamId { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string SoDangKy { get; set; }
        public string TenDuongDung { get; set; }
        public long? DuongDungId { get; set; }
        public long? DonViTinhId { get; set; }
        public string DVT { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string GhiChu { get; set; }
        public double? SoLuongTonDuTru { get; set; }
        public double? SoLuongDuTru { get; set; }
        public double? SoLuongDuKienSuDung { get; set; }
        public List<DuocPhamDuTruViewModelValidator> YeuCauMuaThuocChiTietValidators { get; set; }
        public bool IsThemDuocPham { get; set; }
        public string MaHoatChat { get; set; }
        public LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
    }


}
