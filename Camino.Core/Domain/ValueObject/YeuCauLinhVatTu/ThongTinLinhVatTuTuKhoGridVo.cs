using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhVatTu
{
    public class ThongTinLinhVatTuTuKhoGridVo : GridItem
    {
        public ThongTinLinhVatTuTuKhoGridVo()
        {
            YeuCauVatTuBenhViens = new List<ListIdYeuCauVatTuBenhVien>();
            ListYeuCauVatTuBenhViens = new List<ThongTinLanKhamKho>();
            NgayDieuTris = new List<DateTime>();
        }
        public int STT { get; set; }
        public string LinhVePhong { get; set; }
        public string NguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string TenVatTu { get; set; }
        public long VatTuId { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSX { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLYeuCau { get; set; }
        public string LoaiThuoc { get; set; }
        public bool LoaiVatTu { get; set; }
        public bool DuocPhamDaDuocKe { get; set; }
        public long KhoLinhId { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string DVKham { get; set; }
        public List<ListIdYeuCauVatTuBenhVien> YeuCauVatTuBenhViens { get; set; }
        public List<ThongTinLanKhamKho> ListYeuCauVatTuBenhViens { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
        public bool IsCheckRowItem { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public double SoLuongTon { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string NgayKe { get; set; }
        public string NgayDieuTriTuChoi { get; set; }
        public List<DateTime> NgayDieuTris { get; set; }
    }
    public class ThongTinLinhTuKho : GridItem
    {
        public long LinhTuKhoId { get; set; }
        public long NoiChiDinhId { get; set; }
        public long LinhVePhongId { get; set; }
        public string LinhVePhong { get; set; }
        public string LinhVeKhoa { get; set; }
        public string NguoiYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string TenKho { get; set; }
    }
    public class ThongTinLanKhamKho : GridItem
    {
        public DateTime NgayYeuCau { get; set; }
        public int STT { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DVKham { get; set; }
        public string BacSyKeToa { get; set; }
        public string NgayKe { get; set; }
        public int SLKe { get; set; }
        public bool? DuocDuyet { get; set; }
        public string TenVatTu { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string HangSX { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLYeuCau { get; set; }
        public string LoaiThuoc { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
        public bool IsCheckRowItem { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public double SoLuongTon { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string HighLightClass => SoLuongTon < SLKe ? "bg-row-lightRed" : "";
    }
    public class DaDuyetVatTu
    {
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
    }
}
public class ListIdYeuCauVatTuBenhVien
{
    public long Id { get; set; }
    public string TenThuoc { get; set; }
    public long YeuCauTiepNhanId { get; set; }
}
public class LinhTrucTiepVatTuChiTietGridVo:GridItem
{
    public string Nhom { get; set; }
    public long? YeuCauLinhVatTuId { get; set; }
    public long? VatTuBenhVienId { get; set; }
    public bool? LaVatTuBHYT { get; set; }
    public double? SoLuong { get; set; }
    public double? SoLuongCoTheXuat { get; set; }
    public double? SLTon { get; set; }
    public List<long> YeuCauVatTuBenhVienIds { get; set; }
}
public class YeuCauVatTuBenhVienTT
{
    public long Id { get; set; }
    public long VatTuId { get; set; }
    public string TenThuoc { get; set; }
    public long YeuCauTiepNhanId { get; set; }
    public bool LaVatTuBHYT { get; set; }
    public double? SLTon { get; set; }
    public double? SoLuong { get; set; }
}
public class VoQueryDSVTChoGoi
{
    public long KhoLinhId { get; set; }
    public long YeuCauLinhVatTuId { get; set; }
    public string TuNgay { get; set; }
    public string DenNgay { get; set; }

}
