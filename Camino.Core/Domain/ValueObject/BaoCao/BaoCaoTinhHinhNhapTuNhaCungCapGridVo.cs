using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTinhHinhNhapTuNhaCungCapQueryData
    {
        public DateTime NgayNhap { get; set; }
        //public string SoChungTu { get; set; }
        public string SoPhieu { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string SoHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public List<BaoCaoTinhHinhNhapTuNhaCungCapChiTiet> BaoCaoTinhHinhNhapTuNhaCungCapChiTiets { get; set; }
        //public decimal Thuoc { get; set; }
        //public decimal VTYT { get; set; }
        //public decimal HoaChat { get; set; }
        //public decimal ThueVAT { get; set; }
        //public decimal ThanhTien => Thuoc + VTYT + HoaChat - ThueVAT;//ex
        //public long? NhaCungCapId { get; set; }
        //public string TenNhaCungCap { get; set; }
    }
    public class BaoCaoTinhHinhNhapTuNhaCungCapChiTiet
    {
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long NhaThauId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public decimal DonGiaNhap { get; set; }
        public double SoLuongNhap { get; set; }
        public int Nhom { get; set; }
        public int VAT { get; set; }
        public decimal ThanhTienTruocVat { get; set; }
        public decimal ThanhTienSauVat { get; set; }
        public decimal ThueVatLamTron { get; set; }
        public string GhiChu { get; set; }
    }
    public class BaoCaoTinhHinhNhapTuNhaCungCapGridVo : GridItem
    {
        public DateTime NgayChungTu { get; set; }
        public string NgayChungTuStr => NgayChungTu.ApplyFormatDate();
        public string SoChungTu { get; set; }
        public DateTime NgayHoaDon { get; set; }
        public string NgayHoaDonStr => NgayHoaDon.ApplyFormatDate();
        public string SoHoaDon { get; set; }
        public decimal Thuoc { get; set; }
        public decimal VTYT { get; set; }
        public decimal HoaChat { get; set; }
        public decimal ThueVAT { get; set; }
        public decimal ThanhTien { get; set; }
        public long? NhaCungCapId { get; set; }
        public string TenNhaCungCap { get; set; }
        public string KhoNhap { get; set; }
        public string GhiChu { get; set; }
    }
}