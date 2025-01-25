using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauKSNK
{
    public class LinhThuongKSNKVo
    {
    }
    public class KSNKLookupVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public bool IsDisabled { get; set; }
        public bool? LaKSNKBHYT { get; set; }
        public string HanSuDung { get; set; }
        public double SLTon { get; set; }
        public string SLTonFormat => SLTon.MathRoundNumber().ToString();
        public int? Rank { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string HamLuong { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
    }

    public class KSNKBenhVienJsonVo
    {
        public long? LinhTuKhoId { get; set; }
        public long? LinhVeKhoId { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public bool IsCreate { get; set; }
        public bool? TrangThai { get; set; }
        public string ThoiDiemChiDinhTu { get; set; }
        public string ThoiDiemChiDinhDen { get; set; }
    }

    public class KSNKJsonVo
    {
        public long? KhoId { get; set; }
        public int? LaKSNKBHYT { get; set; }
        public bool? LoaiDuocPhamHayVatTu { get; set; }
    }

    public class KSNKGridViewModelValidators
    {
        public long? KSNKBenhVienId { get; set; }
        public bool? LaKSNKBHYT { get; set; }
    }

    public class LinhThuongKSNKGridVo
    {
        public LinhThuongKSNKGridVo()
        {
            KSNKBenhViens = new List<long>();
        }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }

        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLTon { get; set; }
        public long KhoXuatId { get; set; }
        public int LoaiKSNK { get; set; }
        public List<long> KSNKBenhViens { get; set; }
        
        public long KhoLinhId { get; set; } // lĩnh từ kho id
        public string TenKhoLinh { get; set; } // lĩnh từ kho id
        public bool LoaiDuocPhamHayVatTu { get; set; } 
    }

    public class PhieuLinhThuongKSNK
    {
        public long YeuCauLinhKSNKId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public int? LoaiPhieuLinh { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class ThongTinLinhKSNKChiTiet
    {
        public string Ma { get; set; }
        public string TenThuocHoacKSNK { get; set; }
        public string DVT { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLThucPhat { get; set; }
        public string GhiChu { get; set; }
        public bool LaKSNKBHYT { get; set; }
        public long? KSNKBenhVienId { get; set; }
    }

    public class YeuCauLinhKSNKBuGridParentVo : GridItem
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
        public double? SoLuongCanBu { get; set; }
        public string YeuCauLinhVatTuIdstring { get; set; }
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public bool? KhongLinhBu { get; set; }
        public double? SoLuongDaBu { get; set; }
        public double? SoLuongDuocDuyet { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public double? SLYeuCauLinhThucTeMax => (SoLuongCanBu.GetValueOrDefault() - SoLuongDaBu.GetValueOrDefault()).MathCelling();
        public bool CheckBox { get; set; }

        public bool LoaiDuocPhamHayVatTu { get; set; }

    }
    //public class KSNKLinhBuCuaBNGridVos : GridItem
    //{
    //    public int? STT { get; set; }
    //    public string MaTN { get; set; }
    //    public string MaBN { get; set; }
    //    public string HoTen { get; set; }
    //    public string DVKham { get; set; }
    //    public long? BSKeToaId { get; set; }
    //    public string BSKeToa { get; set; }
    //    public string NgayKe { get; set; }
    //    public double? SL { get; set; }
    //    public double? SLDaBu { get; set; }
    //    public double? SLYeuCau { get; set; }
    //    public DateTime NgayDieuTri { get; set; }
    //    public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();

    //}

    public class ThongTinKSNKTietItem
    {
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public long? KhoLinhTuId { get; set; }
        public long? KhoLinhVeId { get; set; }
        public double SoLuongCanBu { get; set; }
        public double SoLuongTon { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public double? SoLuongDaBu { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public DateTime? ThoiDiemChiDinhTu { get; set; }
        public DateTime? ThoiDiemChiDinhDen { get; set; }
    }
    public class PhieuLinhThuongKSNKXemTruoc
    {
        public PhieuLinhThuongKSNKXemTruoc()
        {
            YeuCauKSNKBenhViens = new List<YeuCauKSNKBenhViens>();
            YeuCauKSNKBenhVienIds = new List<long>();
        }
        public long KhoLinhId { get; set; }
        public long KhoLinhBuId { get; set; }
        public string HostingName { get; set; }
        public List<YeuCauKSNKBenhViens> YeuCauKSNKBenhViens { get; set; }
        public List<long> YeuCauKSNKBenhVienIds { get; set; }
        public DateTime ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime ThoiDiemLinhTongHopDenNgay { get; set; }
    }
    public class YeuCauKSNKBenhViens
    {
        public long KhoLinhTuId { get; set; }
        public long KhoLinhVeId { get; set; }
        public long KSNKBenhVienId { get; set; }
    }
    public class TrangThaiDuyetVos
    {
        public bool? TrangThai { get; set; }
        public EnumTrangThaiPhieuLinh? EnumTrangThaiPhieuLinh { get; set; }
        public string Ten { get; set; }
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
    public class PhieuLinhThuongKSNKData
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
    public class PhieuLinhThuongKSNKModel
    {
        public long YeuCauLinhVatTuId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public int? LoaiPhieuLinh { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class PhieuLinhThuongDPVTModel
    {
        public PhieuLinhThuongDPVTModel()
        {
            YeuCauLinhVatTuIds = new List<InFoPhieuLinh>();
        }
        public List<InFoPhieuLinh> YeuCauLinhVatTuIds { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public int? LoaiPhieuLinh { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class InFoPhieuLinh
    {
        public long YeuCauLinhVatTuId { get; set; } // hoặc dược phẩm
        public bool LoaiDuocPhamHayVatTu { get; set; }

    }
    public class OBJKSNKList
    {
        public int Index { get; set; }
        public string html { get; set; }
    }
}

