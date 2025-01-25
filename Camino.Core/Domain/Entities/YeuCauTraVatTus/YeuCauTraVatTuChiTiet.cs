using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauTraVatTus
{
    public class YeuCauTraVatTuChiTiet : BaseEntity
    {
        public long YeuCauTraVatTuId { get; set; }
        public long? XuatKhoVatTuChiTietViTriId { get; set; }

        public long VatTuBenhVienId { get; set; }
        public double SoLuongTra { get; set; }
        public long HopDongThauVatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal DonGiaNhap { get; set; }

        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }

        public string MaVach { get; set; }
        public string MaRef { get; set; }
        public long? KhoViTriId { get; set; }

        public virtual YeuCauTraVatTu YeuCauTraVatTu { get; set; }
        public virtual XuatKhoVatTuChiTietViTri XuatKhoVatTuChiTietViTri { get; set; }

        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }
        public virtual Kho KhoViTri { get; set; }
    }
}
