using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoBienBanKiemKeKTGridVo : GridItem
    {
        public long KhoId { get; set; }
        public string Kho { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public double SoLuongNhap { get; set; }
        public double SoLuongXuat { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGiaBan { get; set; }
        public bool CoBHYT { get; set; }
        public string SoLo { get; set; }

        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string DonVi { get; set; }
        //public string SoKiemSoat { get; set; } -> SoLo
        public string NuocSX { get; set; }
        public DateTime HanDung { get; set; }
        public string HanDungDisplay => HanDung.ApplyFormatDate();
        public double SLSoSach { get; set; }
        public int SLThucTe { get; set; }
        public string TinhTrangHuTon { get; set; }
        public string GhiChu { get; set; }

        //public long? NhomVTId { get; set; }
        public string NhomVatTuName { get; set; }
    }

    public class BaoCaoBienBanKiemKeKTXuatVTDbQuery
    {
        public long KhoId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public double SoLuongXuat { get; set; }
        public decimal DonGia { get; set; }
        public string SoLo { get; set; }
        public DateTime HanDung { get; set; }
    }
    public class BaoCaoBienBanKiemKeKTXuatDPDbQuery
    {
        public long KhoId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuongXuat { get; set; }
        public decimal DonGia { get; set; }
        public string SoLo { get; set; }
        public DateTime HanDung { get; set; }
    }
}