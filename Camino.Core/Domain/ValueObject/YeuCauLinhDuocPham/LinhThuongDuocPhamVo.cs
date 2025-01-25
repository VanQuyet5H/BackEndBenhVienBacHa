using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
    public class LinhThuongDuocPhamVo
    {
        public LinhThuongDuocPhamVo()
        {
            DuocPhamBenhViens = new List<long>();
        }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public string Ten { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public long? DVTId { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLYeuCau { get; set; }
        public double SLTon { get; set; }
        public long KhoXuatId { get; set; }
        public int LoaiDuocPham { get; set; }
        public List<long> DuocPhamBenhViens { get; set; }
    }

    public class DuocPhamLookupVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public long? DVTId { get; set; }
        public string DVT { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public double SLTon { get; set; }
        public string SLTonFormat => SLTon.MathRoundNumber(2).ToString();
        public string HanSuDung { get; set; }
        public DateTime? NgayNhapVaoBenhVien { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string HamLuong { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        
        public List<string> MayXetNghiemTenVaIds { get; set; }

    }
    public class DuocPhamJsonVo
    {
        public long? KhoId { get; set; }
        public int? LaDuocPhamBHYT { get; set; }
    }

    public class DuocPhamBenhVienJsonVo
    {
        public long? LinhTuKhoId { get; set; }
        public long? LinhVeKhoId { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public bool IsCreate { get; set; }
        public bool? TrangThai { get; set; }
        public string ThoiDiemChiDinhTu { get; set; }
        public string ThoiDiemChiDinhDen { get; set; }
    }

    public class NhanVienYeuCauVo
    {
        public long? NhanVienYeuCauId { get; set; }
        public string HoTen { get; set; }
    }

    public class NhanVienDuyetVo
    {
        public long? NhanVienDuyetId { get; set; }
        public string HoTenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
    }

    public class ThongTinDuocPhamChiTietItem
    {
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public long? KhoLinhTuId { get; set; }
        public long? KhoLinhVeId { get; set; }
        public double SoLuongCanBu { get; set; }
        public double SoLuongTon { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public double? SoLuongDaBu { get; set; }
        public DateTime? ThoiDiemChiDinhTu { get; set; }
        public DateTime? ThoiDiemChiDinhDen { get; set; }
    }

    public class PhieuLinhThuongDuocPhamData
    {
        public string HeaderPhieuLinhThuoc { get; set; }
        public string LogoUrl { get; set; }
        public string MaVachPhieuLinh { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string NoiGiao { get; set; }
        public string DienGiai { get; set; }
        public string CongKhoan { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string ThuocHoacVatTu { get; set; }
        public string NhapVeKho { get; set; }

    }

    public class PhieuLinhThuongDuocPham
    {
        public long YeuCauLinhDuocPhamId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public int? LoaiPhieuLinh { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class ThongTinLinhDuocPhamChiTiet
    {
        public string Ma { get; set; }
        public string TenThuocHoacVatTu { get; set; }
        public string DVT { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLThucPhat { get; set; }
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
    }

    public class DuocPhamLinhBuGridVo : GridItem
    {
        public DuocPhamLinhBuGridVo()
        {
            YeuCauDuocPhamBenhVienIds = new List<long>();
        }
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string TenDuocPham { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double? SoLuongCanBu { get; set; }
        public string Nhom { get; set; }
        public double? SoLuongTon { get; set; }
        public double? SoLuongDaBu { get; set; }
        public bool LaBHYT { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public double? SoLuongDuocDuyet { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        //public double? SLYeuCauLinhThucTeMax => SoLuongCanBu.MathCelling();
        public double? SLYeuCauLinhThucTeMax => (SoLuongCanBu.GetValueOrDefault() - SoLuongDaBu.GetValueOrDefault()).MathCelling();
        public List<long> YeuCauDuocPhamBenhVienIds { get; set; }
    }

    public class DuocPhamLinhBuCuaBNGridVo : GridItem
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
        public double? SLDaBu { get; set; }
        public double? SLYeuCau { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();

    }

    public class TrangThaiDuyetDuTruMuaVo
    {
        public bool? IsKhoaDuyet { get; set; }
        public bool? IsKhoDuocDuyet { get; set; }
        public bool? IsGiamDocDuyet { get; set; }
        public bool? TrangThai { get; set; }
        public string Ten { get; set; }
    }
    public class TrangThaiDuyetVo
    {
        public bool? TrangThai { get; set; }
        public EnumTrangThaiPhieuLinh? EnumTrangThaiPhieuLinh { get; set; }
        public string Ten { get; set; }
    }

    public class DuocPhamGridViewModelValidator
    {
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
    }

    public class YeuCauDuocPhamChiTietsGridVo
    {
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public double? SLYeuCau { get; set; }
    }

    public class LinhBuDuocPhamChiTietVo
    {
        public LinhBuDuocPhamChiTietVo()
        {
            YeuCauDuocPhamBenhVienIds = new List<long>();
        }
        public string Nhom { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public double? SoLuong { get; set; }
        public string Ten { get; set; }
        public bool? DuocDuyet { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLCanBu { get; set; }
        public double? SLTon { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuongDungId { get; set; }
        public string DuongDung { get; set; }
        public long? DVTId { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public bool LaBHYT { get; set; }
        public List<long> YeuCauDuocPhamBenhVienIds { get; set; }
    }
    public class XemPhieuLinhBuDuocPham
    {
        public XemPhieuLinhBuDuocPham()
        {
            YeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhViens>();
            YeuCauDuocPhamBenhVienIds = new List<long>();
        }
        public long KhoLinhId { get; set; }
        public long KhoLinhBuId { get; set; }
        public string HostingName { get; set; }
        public List<YeuCauDuocPhamBenhViens> YeuCauDuocPhamBenhViens { get; set; }
        public List<long> YeuCauDuocPhamBenhVienIds { get; set; }
        public DateTime ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime ThoiDiemLinhTongHopDenNgay { get; set; }
    }
    public class YeuCauDuocPhamBenhViens
    {
        public long KhoLinhTuId { get; set; }
        public long KhoLinhVeId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
    }

    public class DuocPhamBenhVienMayXetNghiemJson
    {
        public long? DuocPhamBenhVienId { get; set; }
    }
}
