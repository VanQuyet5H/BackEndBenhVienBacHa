using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhamViTris;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams
{
    public class YeuCauNhapKhoDuocPhamChiTiet : BaseEntity
    {
        public long YeuCauNhapKhoDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
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
        public DateTime NgayNhap { get; set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public decimal ThanhTienTruocVat { get; set; }
        public decimal ThanhTienSauVat { get; set; }
        public decimal? ThueVatLamTron { get; set; }
        public string GhiChu { get; set; }

        public virtual YeuCauNhapKhoDuocPham YeuCauNhapKhoDuocPham { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual DuocPhamBenhVienPhanNhom DuocPhamBenhVienPhanNhom { get; set; }
        public virtual KhoViTri KhoViTri { get; set; }
        public virtual Kho KhoNhapSauKhiDuyet { get; set; }
        public virtual User NguoiNhapSauKhiDuyet { get; set; }
    }
}
