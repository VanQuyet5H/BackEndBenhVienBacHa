using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhVatTu
{
    public class LinhThuongVatTuVo
    {
    }
    public class VatTuLookupVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public bool IsDisabled { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public string HanSuDung { get; set; }
        public double SLTon { get; set; }
        public string SLTonFormat => SLTon.MathRoundNumber().ToString();
        public int? Rank { get; set; }
    }

    public class VatTuBenhVienJsonVo
    {
        public long? LinhTuKhoId { get; set; }
        public long? LinhVeKhoId { get; set; }
        public long? YeuCauLinhVatTuId { get; set; }
        public bool IsCreate { get; set; }
        public bool? TrangThai { get; set; }
        public string ThoiDiemChiDinhTu { get; set; }
        public string ThoiDiemChiDinhDen { get; set; }
    }

    public class VatTuJsonVo
    {
        public long? KhoId { get; set; }
        public int? LaVatTuBHYT { get; set; }
    }

    public class VatTuGridViewModelValidator
    {
        public long? VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
    }

    public class LinhThuongVatTuGridVo
    {
        public LinhThuongVatTuGridVo()
        {
            VatTuBenhViens = new List<long>();
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
        public int LoaiVatTu { get; set; }
        public List<long> VatTuBenhViens { get; set; }
    }

    public class PhieuLinhThuongVatTu
    {
        public long YeuCauLinhVatTuId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public int? LoaiPhieuLinh { get; set; }
        public bool? TrangThai { get; set; }
    }

    public class ThongTinLinhVatTuChiTiet
    {
        public string Ma { get; set; }
        public string TenThuocHoacVatTu { get; set; }
        public string DVT { get; set; }
        public double? SLYeuCau { get; set; }
        public double? SLThucPhat { get; set; }
        public string GhiChu { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public long? VatTuBenhVienId { get; set; }
    }

    public class YeuCauLinhVatTuBuGridParentVo : GridItem
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
        public double? SLDaBu { get; set; }
        public double? SLYeuCau { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();
    }

    public class ThongTinVatTuTietItem
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
    public class PhieuLinhThuongVatTuXemTruoc
    {
        public PhieuLinhThuongVatTuXemTruoc()
        {
            YeuCauVatTuBenhViens = new List<YeuCauVatTuBenhViens>();
            YeuCauVatTuBenhVienIds = new List<long>();
        }
        public long KhoLinhId { get; set; }
        public long KhoLinhBuId { get; set; }
        public string HostingName { get; set; }
        public List<YeuCauVatTuBenhViens> YeuCauVatTuBenhViens { get; set; }
        public List<long> YeuCauVatTuBenhVienIds { get; set; }
        public DateTime ThoiDiemLinhTongHopTuNgay { get; set; }
        public DateTime ThoiDiemLinhTongHopDenNgay { get; set; }
    }
    public class YeuCauVatTuBenhViens
    {
        public long KhoLinhTuId { get; set; }
        public long KhoLinhVeId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
    }
}
