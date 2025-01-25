using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
    public class ThongTinLinhTuKhoGridVo : GridItem
    {
        public ThongTinLinhTuKhoGridVo()
        {
            YeuCauDuocPhamBenhViens = new List<ListIdYeuCauDuocPhamBenhVien>();
            ListYeuCauDuocPhamBenhViens = new List<ThongTinLanKhamKho>();
            NgayDieuTris = new List<DateTime>();
        }
        public int STT { get; set; }
        public string LinhVePhong { get; set; }
        public string NguoiYeuCau { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public string TenDuocPham { get; set; }
        public bool LoaiDuocPham { get; set; }
        public long DuocPhamId { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSX { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLYeuCau { get; set; }
        public string LoaiThuoc { get; set; }
        public bool DuocPhamDaDuocKe { get; set; }
        public bool LaDuocPhamBHYT { get; set; } 
        public long KhoLinhId { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string SLKe { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public double SoLuongTon { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public List<ListIdYeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens { get; set; }
        public List<ThongTinLanKhamKho> ListYeuCauDuocPhamBenhViens { get; set; }
        //update 
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
        public bool IsCheckRowItem { get; set; }
        public List<long>ListCheckCus { get; set; }
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
        public int STT { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DVKham { get; set; }
        public string BacSyKeToa { get; set; }
        public string NgayKe { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public int SLKe { get; set; }
        public bool? DuocDuyet { get; set; }
        public string TenDuocPham { get; set; }
        public long DuocPhamId { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSX { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLYeuCau { get; set; }
        public string LoaiThuoc { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public double SoLuongTon { get; set; }

        public string GhiChu { get; set; }
        public string LoaiPhieuLinh { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
        // update tạo check grid child
        public bool? IsCheckRowItem { get; set; }
        public long YeuCauTiepNhanId  {get; set; }
        public string HighLightClass => SoLuongTon < SLKe ? "bg-row-lightRed" : "";
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
    }
    public class DaDuyet
    {
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
    }
}
public class ListIdYeuCauDuocPhamBenhVien 
{
    public long Id  { get; set; }
    public string TenThuoc { get; set; }
    public long YeuCauTiepNhanId { get; set; }
}
public class LinhTrucTiepDuocPhamChiTietGridVo :GridItem
{
    public string Nhom { get; set; }
    public long? YeuCauLinhDuocPhamId { get; set; }
    public long? DuocPhamBenhVienId { get; set; }
    public bool? LaDuocPhamBHYT { get; set; }
    public double? SoLuong { get; set; }
    public double? SoLuongCoTheXuat { get; set; }
    public double? SLTon { get; set; }
    public List<long> YeuCauDuocPhamBenhVienIds { get; set; }
}
    public class YeuCauDuocPhamBenhVienTT
{
    public long Id { get; set; }
    public long DuocPhamId { get; set; }
    public string TenThuoc { get; set; }
    public long YeuCauTiepNhanId { get; set; }
    public bool LaDuocPhamBHYT { get; set; }
    public double? SLTon { get; set; }
    public double? SoLuong { get; set; }
}

public class LinhTrucTiepVo{
    public long? YeuCauTiepNhanId { get; set; }
    public bool? CheckItem { get; set; }
    public long? YeuCauLinhDuocPhamId { get; set; }
    public int? PhongLamViecId { get; set; }
    public long? KhoLinhId { get; set; }
    public string DateBatDau { get; set; }
    public string DateKetThuc { get; set; }
  
}
public class VoQueryDSChoGoi
{
    public long KhoLinhId { get; set; }
    public long YeuCauLinhDuocPhamId { get; set; }
    public string TuNgay { get; set; }
    public string DenNgay { get; set; }

}


