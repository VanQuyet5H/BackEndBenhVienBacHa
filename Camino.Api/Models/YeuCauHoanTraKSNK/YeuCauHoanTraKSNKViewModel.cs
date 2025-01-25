using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.YeuCauHoanTraKSNK
{
    public class YeuCauHoanTraKSNKViewModel : BaseViewModel
    {
        public YeuCauHoanTraKSNKViewModel()
        {
            YeuCauTraKSNKChiTiets = new List<YeuCauTraKSNKChiTietViewModel>();
        }

        public string SoPhieu { get; set; }

        public long? KhoXuatId { get; set; }

        public long? KhoNhapId { get; set; }

        public string KhoXuat { get; set; }

        public string KhoNhap { get; set; }

        public long? NhanVienYeuCauId { get; set; }

        public long? NhanVienDuyetId { get; set; }

        public string NhanVienYeuCau { get; set; }

        public string NhanVienDuyet { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string GhiChu { get; set; }

        public string LyDoKhongDuyet { get; set; }

        public bool? DuocDuyet { get; set; }

        public string DuocDuyetDisplay => GetTinhTrang(DuocDuyet);

        public List<YeuCauTraKSNKChiTietViewModel> YeuCauTraKSNKChiTiets { get; set; }

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }
    }

    public class YeuCauTraKSNKChiTietViewModel : BaseViewModel
    {
        public new string Id { get; set; }

        public string Nhom { get; set; }

        public long? VatTuBenhVienId { get; set; }

        public string DVT { get; set; }

        public string HopDong { get; set; }

        public long? HopDongThauVatTuId { get; set; }

        public string VatTu { get; set; }

        public string MaVatTu { get; set; }

        public bool LaVatTuBHYT { get; set; }

        public string Loai => GetLoaiBhyt(LaVatTuBHYT);

        public string SoLo { get; set; }

        public DateTime? HanSuDung { get; set; }

        public DateTime? NgayNhap { get; set; }

        public string HanSuDungDisplay => HanSuDung != null ? HanSuDung.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public string NgayNhapDisplay => NgayNhap != null ? NgayNhap.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public double? SoLuongTra { get; set; }

        public long? YeuCauTraVatTuId { get; set; }

        public int? Vat { get; set; }

        public int? TiLeThapGia { get; set; }

        private string GetLoaiBhyt(bool laBhyt)
        {
            return laBhyt ? "BHYT" : "Không BHYT";
        }

        public double SoLuongTon { get; set; }

        public double? SoLuongXuat { get; set; }
    }

    public class YeuCauHoanTraKSNKViewModelIgnoreValidate : BaseViewModel
    {
        public YeuCauHoanTraKSNKViewModelIgnoreValidate()
        {
            YeuCauTraKSNKChiTiets = new List<YeuCauTraKSNKChiTietViewModel>();
        }

        public string SoPhieu { get; set; }

        public long? KhoXuatId { get; set; }

        public long? KhoNhapId { get; set; }

        public string KhoXuat { get; set; }

        public string KhoNhap { get; set; }

        public long? NhanVienYeuCauId { get; set; }

        public long? NhanVienDuyetId { get; set; }

        public string NhanVienYeuCau { get; set; }

        public string NhanVienDuyet { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string GhiChu { get; set; }

        public string LyDoKhongDuyet { get; set; }

        public bool? DuocDuyet { get; set; }

        public string DuocDuyetDisplay => GetTinhTrang(DuocDuyet);

        public List<YeuCauTraKSNKChiTietViewModel> YeuCauTraKSNKChiTiets { get; set; }

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }
    }

    public class YeuCauHoanTraKSNKTuTrucViewModel : BaseViewModel
    {
        public YeuCauHoanTraKSNKTuTrucViewModel()
        {
            YeuCauHoanTraKSNKChiTiets = new List<YeuCauTraKSNKTuTrucChiTietVo>();
            YeuCauHoanTraKSNKChiTietHienThis = new List<YeuCauTraKSNKGridVo>();
        }

        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
        public long? KhoNhapId { get; set; }
        public string TenKhoNhap { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public string GhiChu { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public Enums.LoaiDuocPhamVatTu? LoaiDuocPhamVatTu { get; set; }
        public List<YeuCauTraKSNKTuTrucChiTietVo> YeuCauHoanTraKSNKChiTiets { get; set; }
        public List<YeuCauTraKSNKGridVo> YeuCauHoanTraKSNKChiTietHienThis { get; set; }
    }
}