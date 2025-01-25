using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.LinhVatTu
{
    public class DSLinhVatTuGridVo : GridItem
    {
        public string MaPL { get; set; }
        public string Loai { get; set; }
        public string NguoiYeuCau { get; set; }
        public string LinhTuKho { get; set; }
        public string LinhVeKho { get; set; }
        public long LinhTuKhoId { get; set; }
        public long LinhVeKhoId { get; set; }
        public string NgayYeuCauHienThi => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string NgayYeuCauString => NgayYeuCau.ApplyFormatDateTimeSACH();
        public DateTime NgayYeuCau { get; set; }
        public string TinhTrang { get; set; }
        public string Nguoiduyet { get; set; }
        public string NgayDuyetHienThi { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetString => NgayDuyet != null ? NgayDuyet.Value.ApplyFormatDateTimeSACH() : "";
        public bool? DuocDuyet { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public List<YeuCauLinhVatTuBuGridVo> ListChildLinhBu { get; set; }
        public List<DSLinhVatChildTuGridVo> ListChildLinhDuTru { get; set; }
        public List<DSLinhVatChildTuGridVo> ListChildLinhBenhNhan { get; set; }
        public bool? DaGui { get; set; }
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
    public class XacNhanInLinhVatTu
    {
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public bool TrangThaiIn { get; set; }
        public string Hosting { get; set; }
    }
    public class XacNhanInLinhVatTuXemTruoc
    {
        public long KhoLinhId { get; set; }
        public string Hosting { get; set; }
        public List<long> YeuCauVatTuBenhVienIds { get; set; }
        public DateTime? ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemLinhTongHopDenNgay { get; set; }
    }
    public class DSLinhVatChildTuGridVo : GridItem
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
        public long VatTuBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public long LinhveKhoId { get; set; }
        ////

        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public double SoLuong { get; set; }
        public long BenhNhanId { get; set; }
        //

        public string KeyId { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongYeuCau { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public bool? DuocDuyet { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public List<DSLinhVatChildTuGridVo> ListChildChildLinhBenhNhan { get; set; }
        public DateTime? NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri != null ? NgayDieuTri.Value.ApplyFormatDateTimeSACH() :"";
    }
    public class YeuCauLinhVatTuBuGridVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
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
        public string YeuCauLinhVatTuIdstring { get; set; }
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public double SoLuongYeuCau { get; set; }
        public bool? KhongLinhBu { get; set; }
        public long LinhveKhoId { get; set; }
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public double SLDaLinh { get; set; }
        public List<VatTuLinhBuCuaBNGridVos> ListChildChildLinhBu { get; set; }
        public bool? DaDuyet { get; set; }
        public double SoLuongYeuCauDaDuyet { get; set; }
    }
    public class VatTuLinhBuCuaBNGridVos : GridItem
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
    public class InXemTruocLinhTTGridVo
    {
        public string NgayKe { get; set; }
        public bool DuocDuyet { get; set; }
        public string BacSyKeToa { get; set; }
        public string HoatChat { get; set; }
        public string DonViTinh { get; set; }
        public string HangSX { get; set; }
        public string NuocSanXuat { get; set; }
        public double SLYeuCau { get; set; }
        public string LoaiVatTu { get; set; }
        public long VatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string GhiChu { get; set; }
        public string LoaiPhieuLinh { get; set; }
        public string TenVatTu { get; set; }
    }
    public class VatTuGridVo
    {
        public double SoLuong { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public bool LaVatTuBHYT { get; set; }
    }
    public class VatTuDaTaoGridVo
    {
        public bool? DuocDuyet { get; set; }
        public string TenLoaiLinh { get; set; }
        public double SoLuong { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuongCoTheXuat { get; set; }
        public string GhiChu { get; set; }
        public double YeuCau { get; set; }
        public string MaTN { get; set; }
        public bool LaVatTuBHYT { get; set; }
    }
    public class OBJListVatTu
    {
        public string html { get; set; }
        public int Index { get; set; }
    }
    public class LinhVatTuTrucTiepQueryInfo
    {
        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public long idKhoLinh { get; set; }
        public long phongDangNhapId { get; set; }
        public string dateSearchStart { get; set; }
        public string dateSearchEnd { get; set; }
    }
}

