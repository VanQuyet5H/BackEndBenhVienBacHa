using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.GoiDichVu
{
    public class ChuongTrinhGoiDvMarketingViewModel : BaseViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string GoiDv { get; set; }

        public long GoiDvId { get; set; }

        public long GoiDvIdCu { get; set; }

        public string GoiDichVu { get; set; }

        public string MoTaGoiDichVu { get; set; }

        public decimal GiaTruocChietKhau { get; set; }

        public decimal? GiaSauChietKhau { get; set; }

        //public double TiLeChietKhau { get; set; }

        public DateTime TuNgay { get; set; }

        public string TuNgayDisplay => TuNgay.ApplyFormatDate();

        public DateTime? DenNgay { get; set; }

        public string DenNgayDisplay => DenNgay != null ? DenNgay.GetValueOrDefault().ApplyFormatDate() : string.Empty;

        public bool? TamNgung { get; set; }

        public bool CoYeuCauSuDung { get; set; }

        public bool? GoiSoSinh { get; set; }

        public long? LoaiGoiDichVuId { get; set; }
        public string TenLoaiGoiDichVu { get; set; }


        public long? CongTyBaoHiemTuNhanId { get; set; }

        public List<QuaTangKemViewModel> QuaTangKems { get; set; }
        public List<KhuyenMaiKemViewModel> KhuyenMaiKems { get; set; }
        public List<KhuyenMaiKemViewModel> KhuyenMaiKemsGiuong { get; set; }
        public List<KhuyenMaiKemViewModel> KhuyenMaiKemsKhamBenh { get; set; }
        public List<KhuyenMaiKemViewModel> KhuyenMaiKemsKyThuat { get; set; }
        public List<YeuCauSuDungChuongTrinhViewModel> YeuCauSuDungChuongTrinhs { get; set; }
        public List<NhomDichVuViewModel> NhomDichVus { get; set; }
    }

    public class QuaTangKemViewModel
    {
        public string GhiChu { get; set; }

        public long IdSys { get; set; }

        public long QuaTangId { get; set; }

        public string QuaTang { get; set; }

        public double SoLuong { get; set; }

        public long GoiDvChuongTrinhMarketingId { get; set; }
    }

    public class KhuyenMaiKemViewModel
    {
        public long DvId { get; set; }

        public long IdDatabase { get; set; }

        public long GoiDichVuId { get; set; }

        public string MaDv { get; set; }

        public string TenDv { get; set; }

        public long LoaiGia { get; set; }

        public int SoLuong { get; set; }

        public string GhiChu { get; set; }

        public string LoaiGiaDisplay { get; set; }

        public Enums.EnumDichVuTongHop Nhom { get; set; }

        public string NhomDisplay => Nhom.GetDescription();

        public decimal DonGia { get; set; }

        public decimal DonGiaKhuyenMai { get; set; }

        public decimal ThanhTien => DonGiaKhuyenMai * SoLuong;
        public int SoNgaySuDung { get; set; }
    }

    public class NhomDichVuViewModel
    {
        public long Id { get; set; }
        public long DvId { get; set; }

        public string MaDv { get; set; }

        public string TenDv { get; set; }

        public Enums.EnumDichVuTongHop Nhom { get; set; }

        public string NhomDisplay => Nhom.GetDescription();
        public long LoaiGia { get; set; }

        public string LoaiGiaDisplay { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGiaBenhVien { get; set; }

        public decimal DonGiaTruocChietKhau { get; set; }

        public decimal DonGiaSauChietKhau { get; set; }
        public decimal ThanhTienBenhVien => DonGiaBenhVien * SoLuong;
        public decimal ThanhTienTruocChietKhau => DonGiaTruocChietKhau * SoLuong;
        public decimal ThanhTienSauChietKhau => DonGiaSauChietKhau * SoLuong;
    }
    public class YeuCauSuDungChuongTrinhViewModel : BaseViewModel
    {
        public string MaBn { get; set; }

        public string TenBn { get; set; }

        public string DiaChi { get; set; }

        public DateTime NgayDangKy { get; set; }

        public string NgayDangKyDisplay => NgayDangKy.ApplyFormatDate();
    }

    public class LoaiGoiDichVuModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool IsDefault { get; set; }
    }
}
