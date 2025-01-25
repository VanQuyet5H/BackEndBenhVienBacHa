using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Camino.Api.Models.YeuCauHoanTraDuocPham
{
    public class YeuCauHoanTraDuocPhamViewModel : BaseViewModel
    {
        public YeuCauHoanTraDuocPhamViewModel()
        {
            DuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTietViewModel>();
            YeuCauTraDuocPhamChiTiets = new List<DuocPhamHoanTraChiTiet>();
        }

        public string SoPhieu { get; set; }

        public long? KhoXuatId { get; set; }
        public string TenKhoCanHoanTra { get; set; }
        public long? KhoNhapId { get; set; }
        public string TenKhoNhanHoanTra { get; set; }

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

        public List<YeuCauTraDuocPhamChiTietViewModel> DuocPhamChiTiets { get; set; }

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }

        public List<DuocPhamHoanTraChiTiet> YeuCauTraDuocPhamChiTiets { get; set; }
    }

    public class YeuCauTraDuocPhamChiTietViewModel : BaseViewModel
    {
        public string Nhom { get; set; }

        public long? DuocPhamBenhVienId { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }

        public string HopDong { get; set; }

        public long? HopDongThauDuocPhamId { get; set; }

        public string DuocPham { get; set; }

        public bool LaDuocPhamBHYT { get; set; }

        public string Loai => GetLoaiBhyt(LaDuocPhamBHYT);

        public string SoLo { get; set; }

        public DateTime? HanSuDung { get; set; }

        public double? SoLuongTra { get; set; }

        public long? YeuCauTraDuocPhamId { get; set; }

        public int? Vat { get; set; }

        public int? TiLeThapGia { get; set; }

        private string GetLoaiBhyt(bool laBhyt)
        {
            return laBhyt ? "BHYT" : "Không BHYT";
        }
    }

    public class DuocPhamHoanTraChiTiet : BaseViewModel
    {
        //public DuocPhamHoanTraChiTiet()
        //{
        //    XuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTriViewModel>();
        //}

        public string Id { get; set; }
        public int STT { get; set; }
        public string TenDuocPham { get; set; }
        public string MaDuocPham { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ToString(CultureInfo.CurrentCulture);
        public double? SoLuongXuat { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }
        public string SoLo { get; set; }

        public string SoDangKy { get; set; }

        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();

        public DateTime? NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap != null ? NgayNhap.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        //public List<XuatKhoDuocPhamChiTietViTriViewModel> XuatKhoDuocPhamChiTietViTris { get; set; }
    }

    public class YeuCauHoanTraDuocPhamViewModelIgnoreValidate : BaseViewModel
    {
        public YeuCauHoanTraDuocPhamViewModelIgnoreValidate()
        {
            DuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTietViewModel>();
            YeuCauTraDuocPhamChiTiets = new List<DuocPhamHoanTraChiTiet>();
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

        public List<YeuCauTraDuocPhamChiTietViewModel> DuocPhamChiTiets { get; set; }

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }

        public List<DuocPhamHoanTraChiTiet> YeuCauTraDuocPhamChiTiets { get; set; }
    }

    //Update 31/12/2021

    public class YeuCauHoanTraDuocPhamTuTrucViewModel : BaseViewModel
    {
        public YeuCauHoanTraDuocPhamTuTrucViewModel()
        {
            YeuCauHoanTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamTuTrucChiTietVo>();
            YeuCauHoanTraDuocPhamChiTietHienThis = new List<YeuCauTraDuocPhamGridVo>();
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
        public List<YeuCauTraDuocPhamTuTrucChiTietVo> YeuCauHoanTraDuocPhamChiTiets { get; set; }
        public List<YeuCauTraDuocPhamGridVo> YeuCauHoanTraDuocPhamChiTietHienThis { get; set; }
    }
}
