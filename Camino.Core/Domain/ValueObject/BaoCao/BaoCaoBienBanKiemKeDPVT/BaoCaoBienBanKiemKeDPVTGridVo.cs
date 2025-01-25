using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT
{
    public class BaoCaoBienBanKiemKeDPVTDbQuery
    {
        public long DuocPhamBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public double SoLuongNhap { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanDung { get; set; }
    }
    public class BaoCaoBienBanKiemKeXuatVTDbQuery
    {
        public long VatTuBenhVienId { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanDung { get; set; }
    }
    public class BaoCaoBienBanKiemKeXuatDPDbQuery
    {
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanDung { get; set; }
    }
    public class BaoCaoBienBanKiemKeDPVTGridVo:GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string MaDuoc { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public double SoLuongNhap { get; set; }
        public double SoLuongXuat { get; set; }
        public double SoLuongHienCo { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanDung { get; set; }
        public string HanDungStr => HanDung.HasValue ? HanDung.Value.ApplyFormatDate() : "";
        public string GhiChu { get; set; }
    }
}
