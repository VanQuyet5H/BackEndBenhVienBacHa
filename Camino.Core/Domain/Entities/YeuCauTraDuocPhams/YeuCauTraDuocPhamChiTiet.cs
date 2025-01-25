using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauTraDuocPhams
{
    public class YeuCauTraDuocPhamChiTiet : BaseEntity
    {
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuongTra { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public string Solo { get; set; }
        public DateTime HanSuDung { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public string MaVach { get; set; }
        public string MaRef { get; set; }
        public long? KhoViTriId { get; set; }


        public long YeuCauTraDuocPhamId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }

        public virtual YeuCauTraDuocPham YeuCauTraDuocPham { get; set; }
        public virtual XuatKhoDuocPhamChiTietViTri XuatKhoDuocPhamChiTietViTri { get; set; }

        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual DuocPhamBenhVienPhanNhom DuocPhamBenhVienPhanNhom { get; set; }
        public virtual Kho KhoViTri { get; set; }
    }
}
