using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using System;

namespace Camino.Core.Domain.Entities.XuatKhoVatTus
{
    public class YeuCauXuatKhoVatTuChiTiet : BaseEntity
    {
        public long YeuCauXuatKhoVatTuId { get; set; }
        public long? XuatKhoVatTuChiTietViTriId { get; set; }
        public double SoLuongXuat { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long HopDongThauVatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public string MaVach { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public string MaRef { get; set; }
        public virtual YeuCauXuatKhoVatTu YeuCauXuatKhoVatTu { get; set; }
        public virtual XuatKhoVatTuChiTietViTri XuatKhoVatTuChiTietViTri { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }
    }
}
