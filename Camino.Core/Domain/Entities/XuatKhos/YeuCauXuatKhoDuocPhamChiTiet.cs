using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.MayXetNghiems;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.XuatKhos
{
    public class YeuCauXuatKhoDuocPhamChiTiet : BaseEntity
    {
        public long YeuCauXuatKhoDuocPhamId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public double SoLuongXuat { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        //public PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public decimal? DonGiaTonKho { get; set; }
        public string MaVach { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public string MaRef { get; set; }
        public long? MayXetNghiemId { get; set; }

        public virtual YeuCauXuatKhoDuocPham YeuCauXuatKhoDuocPham { get; set; }
        public virtual XuatKhoDuocPhamChiTietViTri XuatKhoDuocPhamChiTietViTri { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual MayXetNghiem MayXetNghiem { get; set; }
    }
}
