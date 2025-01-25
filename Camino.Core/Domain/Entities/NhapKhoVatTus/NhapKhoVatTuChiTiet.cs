using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhamViTris;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.NhapKhoVatTus
{
    public class NhapKhoVatTuChiTiet : BaseEntity
    {
        public long NhapKhoVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long HopDongThauVatTuId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public double SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; set; }
        public string MaVach { get; set; }
        public long? KhoViTriId { get; set; }
        public double SoLuongDaXuat { get; set; }
        public DateTime NgayNhap { get; set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaTonKho { get; set; }

        public virtual NhapKhoVatTu NhapKhoVatTu { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }
        public virtual KhoViTri KhoViTri { get; set; }

        public virtual Kho KhoNhapSauKhiDuyets { get; set; }
        public virtual User NguoiNhapSauKhiDuyet { get; set; }

        private ICollection<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTris;
        public virtual ICollection<XuatKhoVatTuChiTietViTri> XuatKhoVatTuChiTietViTris
        {
            get => _xuatKhoVatTuChiTietViTris ?? (_xuatKhoVatTuChiTietViTris = new List<XuatKhoVatTuChiTietViTri>());
            protected set => _xuatKhoVatTuChiTietViTris = value;

        }
    }
}
