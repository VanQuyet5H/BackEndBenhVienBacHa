using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class XacNhanCapCodeTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? PhienXetNghiemId { get; set; }
        public XacNhanCapCodeTrangThaiTimKiemNangCapVo TrangThai { get; set; }
        public XacNhanCapCodeTuNgayDenNgayVo TuNgayDenNgay { get; set; }
    }

    public class XacNhanCapCodeTrangThaiTimKiemNangCapVo
    {
        public bool ChuaCapCode { get; set; }
        public bool DaCapCode { get; set; }
        public bool ChuaNhanMau { get; set; }
        public bool DaNhanMau { get; set; }
    }

    public class XacNhanCapCodeTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class BenhNhanXetNghiemGridVo : GridItem
    {
        public BenhNhanXetNghiemGridVo()
        {
            BenhNhanXetNghiemPhienChiTiets = new List<BenhNhanXetNghiemPhienChiTietGridVo>();
        }
        public long? PhienXetNghiemId { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        //public string NamSinhDisplay =>
        //    (NgaySinh != null && NgaySinh != 0 && ThangSinh != null && ThangSinh != 0 && NamSinh != null && NamSinh != 0)
        //        ? (new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value)).ToString("dd/MM/yyyy")
        //        : NamSinh?.ToString();
        public string NamSinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string Barcode { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? ThoiDiemBatDau { get; set; }
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }

        public List<BenhNhanXetNghiemPhienChiTietGridVo> BenhNhanXetNghiemPhienChiTiets { get; set; }
        public string NhanVienLayMauIdDisplay { get; set; }
        public string ThoiGianLayMauDisplay { get; set; }
    }

    public class BenhNhanXetNghiemQueryVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? PhienXetNghiemId { get; set; }
    }

    public class ThongTinBenhNhanXetNghiemVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NamSinhDisplay =>
            (NgaySinh != null && NgaySinh != 0 && ThangSinh != null && ThangSinh != 0 && NamSinh != null && NamSinh != 0)
                ? (new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value)).ToString("dd/MM/yyyy")
                : NamSinh?.ToString();
        public int Tuoi => NamSinh == null ? 0 : DateTime.Now.Year - NamSinh.Value;
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh.GetDescription();
        public string Tuyen { get; set; }
        public int? MucHuong { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string SoDienThoai { get; set; }
        public string SoTheBHYT { get; set; }
        public bool IsCoDuKetQua { get; set; }
        public bool IsTraKetQua { get; set; }
        public bool IsAutoBarcode { get; set; }
        public string TraKetQuaChoBenhNhan => IsTraKetQua ? "Đã trả" : "Chưa trả";
        public bool IsCoPhienChiTietCoKetQua { get; set; }
        public bool IsNhanVienKhoaXetNghiem { get; set; }

        //BVHD-3364
        public string TenCongTy { get; set; }

        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public bool? CoBHYT { get; set; }
        public string DoiTuong
        {
            get
            {
                var doiTuong = string.Empty;
                if (LoaiYeuCauTiepNhan == Enums.EnumLoaiYeuCauTiepNhan.KhamSucKhoe)
                {
                    doiTuong = "Khám sức khỏe";
                }
                else
                {
                    if (MucHuong != null && MucHuong != 0 && CoBHYT == true)
                    {
                        doiTuong = $"BHYT ({MucHuong}%)";
                    }
                    else
                    {
                        doiTuong = "Viện phí";
                    }
                }
                return doiTuong;
            }
        }

        public int? SoLuongThem { get; set; }
        public int? BarcodeNumber { get; set; }
        public string Barcode { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }


    public class BenhNhanXetNghiemChuaCapCodeExcel
    {
        [Width(30)]
        public string TenCongTy { get; set; }
        [Width(30)]
        public string SoHopDong { get; set; }
        [Width(30)]
        public string MaBenhNhan { get; set; }
        [Width(30)]
        public string MaTiepNhan { get; set; }
        [Width(30)]
        public string Barcode { get; set; }
        [Width(30)]
        public string HoTen { get; set; }
        [Width(30)]
        public string GioiTinhDisplay { get; set; }
        [Width(30)]
        public string NamSinhDisplay { get; set; }
        [Width(30)]
        public string NhanVienLayMauIdDisplay { get; set; }
        [Width(30)]
        public string ThoiGianLayMauDisplay { get; set; }
    }
    public class BenhNhanXetNghiemChuaCapBarcodeImport
    {
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
    }

    public class BenhNhanChuaCapBarcode
    {
        public string TenCongTy { get; set; }
        public string SoHopDong { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string BarcodeNumberInput { get; set; }
        public int? BarcodeNumber { get; set; }
        public string HoTen { get; set; }
        public string GioiTinhDisplay { get; set; }
        public string NamSinhDisplay { get; set; }
        public string Error { get; set; }
        public string BarcodeId { get; set; }
        public bool IsError { get; set; }
        // BVHD-3836
        public long? NhanVienLayMauId { get; set; }
        public DateTime? ThoiGianLayMau { get; set; }
        public string NhanVienLayMauIdDisplay { get; set; }
        public string ThoiGianLayMauDisplay { get; set; }


    }

    public class BenhNhanChuaCapBarcodeLookupVo
    {
        public int BarCodeNumber { get; set; }
        public string BarCodeId { get; set; }
        public string MaTN { get; set; }
    }

    public class BenhNhanXetNghiemPhienChiTietGridVo
    {
        public DateTime? ThoiDiemLayMau { get; set; }
        public DateTime? ThoiDiemNhanMau { get; set; }
        public long? KhoaPhongChiDinhId { get; set; }
        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThai { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
    }

    public class InBarcodeDaCapCodeBenhNhan
    {
        public string HostingName { get; set; }
        public string SearchString { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long? HopDongKhamSucKhoeId { get; set; }
        public int? SoLuong { get; set; }
    }


    public class InBarcodeBenhNhansVo
    {
        public long PhienXetNghiemId { get; set; }
        public string BarcodeId { get; set; }
        public string BarcodeIdPrint { get; set; }
        public int BarcodeNumber { get; set; }
        public string TenBenhNhan { get; set; }
        public string BarcodeByBarcodeId { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhFotmat => GioiTinh.GetDescription();
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhFotmat => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string GioCapCode { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public int SLNhomDichVuBenhVien { get; set; }
    }
}
