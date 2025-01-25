using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTraCuuDuLieuQueryInfo : QueryInfo
    {
        public string TimKiem { get; set; }
        public List<long> BaoCaoTraCuuDuLieuIds { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }

    public class BaoCaoTraCuuDuLieu : GridItem
    {
        public string HienThiTrenSo { get; set; }

        public DateTime NgayChungTu { get; set; }
        public string NgayChungTuDisplay => NgayChungTu.ApplyFormatDate();

        public DateTime NgayHachToan { get; set; }
        public string NgayHachToanDisplay => NgayHachToan.ApplyFormatDate();

        public string SoChungTu { get; set; }
        public string DienGiai { get; set; }
        public string HanThanhToan { get; set; }
        public string DienGiaiHachToan { get; set; }
        public string TaiKhoanNo { get; set; }
        public string TaiKhoanCo { get; set; }
        public decimal SoTien { get; set; }
        public string DoiTuongNo { get; set; }
        public string DoiTuongCo { get; set; }
        public string TaiKhoanNganHang { get; set; }
        public string KhoanMucCP { get; set; }
        public string DonVi { get; set; }
        public string DoiTuongTHCP { get; set; }
        public string CongTrinh { get; set; }
        public string HopDongBan { get; set; }
        public string CPKhongHopLy { get; set; }
        public string MaThongKe { get; set; }
        public string DienGiaiThue { get; set; }
        public string TKThueGTGT { get; set; }

        public decimal? TienThueGTGT { get; set; }
        public int? PhanTramThueGTGT { get; set; }
        public decimal? GiaTriHHDVChuaThue { get; set; }
        public string MauSoHopDong { get; set; }

        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDate();

        public string KyHieuHopDong { get; set; }
        public string SoHoaDon { get; set; }
        public int NhomHHDVMuaVao { get; set; }
        public string MaDoiTuongThue { get; set; }
        public string TenDoiTuongThue { get; set; }
        public string MaSoThueDoiTuongThue { get; set; }

        //BVHD-3957       
        public bool DaXuatExcel { get; set; }
        public string HighLightClass => DaXuatExcel ? "bg-row-lightblue" : string.Empty;
    }

    public class BaoCaoTraCuuDuLieuQueryData
    {
        //BVHD-3957
        public long Id { get; set; }
        public bool? DaXuatExcel { get; set; }
        public DateTime NgayNhap { get; set; }
        //public string SoChungTu { get; set; }
        public string SoPhieu { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string SoHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu DuocPhamHoacVatTu { get; set; }
        public List<BaoCaoTraCuuDuLieuQueryDataDetail> BaoCaoTraCuuDuLieuQueryDataDetails { get; set; }
    }
    public class BaoCaoTraCuuDuLieuQueryDataDetail
    {
        //BVHD-3957
        public long Id { get; set; }

        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long NhaThauId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public decimal DonGiaNhap { get; set; }
        public double SoLuongNhap { get; set; }
        public string Nhom { get; set; }
        public int VAT { get; set; }
        public decimal ThanhTienTruocVat { get; set; }
        public decimal ThanhTienSauVat { get; set; }
        public decimal ThueVatLamTron { get; set; }
    }
}
