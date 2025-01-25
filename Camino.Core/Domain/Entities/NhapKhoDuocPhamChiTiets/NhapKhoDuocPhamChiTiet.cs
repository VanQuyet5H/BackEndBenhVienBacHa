using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhamViTris;
using Camino.Core.Domain.Entities.XuatKhos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets
{
    public class NhapKhoDuocPhamChiTiet : BaseEntity
    {
        public long NhapKhoDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long HopDongThauDuocPhamId { get; set; }
        public string Solo { get; set; }
        //public bool DatChatLuong { get; set; }
        public DateTime HanSuDung { get; set; }
        public double SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; set; }
        public int VAT { get; set; }
        //public int? ChietKhau { get; set; }
        public string MaVach { get; set; }
        //public long? KhoDuocPhamViTriId { get; set; }
        public long? KhoViTriId { get; set; }
        public double SoLuongDaXuat { get; set; }

        public bool LaDuocPhamBHYT { get; set; }
        public int? TiLeBHYTThanhToan { get; set; }
        public DateTime NgayNhapVaoBenhVien { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public Enums.PhuongPhapTinhGiaTriTonKho PhuongPhapTinhGiaTriTonKho { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaTonKho { get; set; }
        public string DanhSachMayXetNghiemId { get; set; }

        public virtual DuocPhamBenhVienPhanNhom DuocPhamBenhVienPhanNhom { get; set; }

        public virtual NhapKhoDuocPhams.NhapKhoDuocPham NhapKhoDuocPhams { get; set; }
        public virtual HopDongThauDuocPhams.HopDongThauDuocPham HopDongThauDuocPhams { get; set; }
        public virtual DuocPhamBenhViens.DuocPhamBenhVien DuocPhamBenhViens { get; set; }
        public DateTime NgayNhap { get; set; }
        public string MaRef { get; set; }
        public long? KhoNhapSauKhiDuyetId { get; set; }
        public long? NguoiNhapSauKhiDuyetId { get; set; }
        public virtual KhoViTri KhoDuocPhamViTri { get; set; }
        public virtual Kho KhoNhapSauKhiDuyet { get; set; }
        public virtual User NguoiNhapSauKhiDuyet { get; set; }

        private ICollection<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTris;

        public virtual ICollection<XuatKhoDuocPhamChiTietViTri> XuatKhoDuocPhamChiTietViTris
        {
            get => _xuatKhoDuocPhamChiTietViTris ?? (_xuatKhoDuocPhamChiTietViTris = new List<XuatKhoDuocPhamChiTietViTri>());
            protected set => _xuatKhoDuocPhamChiTietViTris = value;
        }

        
    }
}
