using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoBangKePhieuXuatKhoQueryData
    {
        public string KhoXuat { get; set; }
        public long PhieuXuatId { get; set; }
        public string KhoNhap { get; set; }
        public bool? TraNCC { get; set; }
        public bool XuatChoBenhNhan { get; set; }
        public DateTime NgayXuat { get; set; }
        public string SoPhieu { get; set; }
        public List<BaoCaoBangKePhieuXuatKhoGridVo> BaoCaoBangKePhieuXuatKhoGridVos { get; set; }
    }
    public class BaoCaoBangKePhieuXuatKhoGridVo : GridItem
    {
        public long DuocPhamId { get; set; }
        public string MaDuoc { get; set; }
        public string TenDuoc { get; set; }
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuong * DonGia;
        //public long? KhoVatTuId { get; set; } -> PhieuXuatId
        public long PhieuXuatId { get; set; }
        public string TenPhieu { get; set; }
        //public string MaKho { get; set; } -> TenPhieu
        //public string TenKhoVTYT { get; set; } -> TenPhieu
    }
    public class BaoCaoBangKePhieuXuatKhoGroupVo
    {
        public long PhieuXuatId { get; set; }
    }
    public class BaoCaoBangKePhieuXuatKhoVo
    {
        public BaoCaoBangKePhieuXuatKhoVo()
        {
            Data = new List<BaoCaoBangKePhieuXuatKhoGridVo>();
            ListGroupTheoFileExecls = new List<BaoCaoBangKePhieuXuatKhoGroupVo>();
        }
        public int TotalRowCount { get; set; }
        public List<BaoCaoBangKePhieuXuatKhoGridVo> Data { get; set; }
        public List<BaoCaoBangKePhieuXuatKhoGroupVo> ListGroupTheoFileExecls { get; set; }
    }
}