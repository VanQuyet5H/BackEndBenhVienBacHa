using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.LinhDuocPham
{
    public class DsLinhDuocPhamGridVo : GridItem
    {
        public string MaPL { get; set; }
        public string Loai { get; set; }
        public string NguoiYeuCau { get; set; }
        public string LinhTuKho { get; set; }
        public string LinhVeKho { get; set; }
        public string NgayYeuCauHienThi => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NgayYeuCauString => NgayYeuCau.ApplyFormatDateTimeSACH();
        public DateTime NgayYeuCau { get; set; }
        public string TinhTrang { get; set; }
        public string Nguoiduyet { get; set; }
        public string NgayDuyetString => NgayDuyet != null ? NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";
        public string NgayDuyetHienThi { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public long LinhVeKhoId { get; set; }
        public bool? DaGui { get; set; }

        public List<YeuCauLinhDuocPhamBuGridVo> ListChildLinhBu { get; set; }

        public List<DSLinhDuocPhamChildTuGridVo> ListChildLinhDuTru { get; set; }
        public List<DSLinhDuocPhamChildTuGridVo> ListChildLinhBenhNhan { get; set; }

    }
    public class SeachNgay
    {
        public string NgayYeuCauRangDateStartDate { get; set; }
        public string NgayYeuCauRangDateStartEnd { get; set; }
        public string NgayDuyetRangDateStartDate { get; set; }
        public string NgayDuyetRangDateStartEnd { get; set; }
        public bool? DangChoGoi { get; set; }
        public bool? DangChoDuyet { get; set; }
        public bool? TuChoiDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public string Searching { get; set; }
    }
    public class XacNhanInLinhDuocPham
    {
        public long YeuCauLinhDuocPhamId { get; set; }
        public long LoaiPhieuLinh { get; set; }

        public string Hosting { get; set; }
        public bool TrangThaiIn { get; set; }
    }
    public class XacNhanInLinhDuocPhamXemTruoc
    {
        public long KhoLinhId { get; set; }
        public string Hosting { get; set; }
        public List<long> YeuCauDuocPhamBenhVienIds { get; set; }
        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }
    }
    public class YeuCauDuocPhamBenhViensGridVo
    {
        public long Id { get; set; }
        public string TenThuoc { get; set; }
    }
    public class DSLinhDuocPhamChildTuGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DVKham { get; set; }
        public string BacSyKeToa { get; set; }
        public DateTime NgayKe { get; set; }
        public string NgayKetString { get; set; }
        public double SL { get; set; }
        public string BSKeToa { get; set; }
        public string Ma { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongYc { get; set; }
        public string NongDoVaHamLuong { get; set; }
        public string Nhom { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public double SLTon { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public long LinhVeKhoId { get; set; }
        //

        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public double SoLuong { get; set; }
        public long BenhNhanId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        //

        public string KeyId { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public string TenDuocPham { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongYeuCau { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public bool? DuocDuyet { get; set; }
        public List<DSLinhDuocPhamChildTuGridVo> ListChildChildLinhBenhNhan { get; set; }

        public DateTime? NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri != null ? NgayDieuTri.Value.ApplyFormatDateTimeSACH():"";
        public string HighLightClass => SoLuongTon < SoLuongYeuCau && DuocDuyet != true ? "" : "";
    }
    public class YeuCauLinhDuocPhamBuGridVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenVatTu { get; set; }
        public string Nhom { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongCanBu { get; set; }
        public string SoLuongCanBuString => SoLuongCanBu.MathRoundNumber(2).ToString();
        public string YeuCauLinhVatTuIdstring { get; set; }
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public double SoLuongYeuCau { get; set; }
        public bool? KhongLinhBu { get; set; }
        public long LinhVeKhoId { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public double SLDaLinh { get; set; }
        public List<DuocPhamLinhBuCuaBNGridVos> ListChildChildLinhBu { get; set; }
        public bool? DaDuyet { get; set; }
        public double SoLuongYeuCauDaDuyet { get; set; }
    }
    public class DuocPhamLinhBuCuaBNGridVos : GridItem
    {
        public int? STT { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DVKham { get; set; }
        public long? BSKeToaId { get; set; }
        public string BSKeToa { get; set; }
        public string NgayKe { get; set; }
        public double? SL { get; set; }
        public double? SLDaLinh { get; set; }
        public double? SLDanhSachDuyet { get; set; }
        public double? SLCanBu { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
    }
    public class DuocPhamLoaiQuanLyLinhTTGridVo : GridItem
    {
        public string MaDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public bool? DuocDuyet { get; set; }
        public double SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
       
    }
    public class OBJList
    {
        public int  Index { get; set; }
        public string html { get; set; }
    }
    public class DuocPhamVatTuLinhTTGridVo : GridItem
    {
        public string MaDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public string TenLoaiLinh { get; set; }
        public bool? DuocDuyet { get; set; }
        public double SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string GhiChu { get; set; }
        public double YeuCau { get; set; }
        public string MaTN { get; set; }
    }
}
