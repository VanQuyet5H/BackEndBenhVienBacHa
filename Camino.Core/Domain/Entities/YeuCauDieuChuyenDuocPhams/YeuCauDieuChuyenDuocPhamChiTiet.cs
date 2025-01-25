using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.XuatKhos;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams
{
    public class YeuCauDieuChuyenDuocPhamChiTiet : BaseEntity   
    {
        public long YeuCauDieuChuyenDuocPhamId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public double SoLuongDieuChuyen { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? DonGiaTonKho { get; set; }
        public string MaVach { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public string MaRef { get; set; }
        public virtual YeuCauDieuChuyenDuocPham YeuCauDieuChuyenDuocPham { get; set; }
        public virtual XuatKhoDuocPhamChiTietViTri XuatKhoDuocPhamChiTietViTri { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
    }
}
