using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams
{
    public class YeuCauNhapKhoDuocPham : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }

        public long KhoId { get; set; }
        public string SoChungTu { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public DateTime NgayNhap { get; set; }
        public long NguoiNhapId { get; set; }
        public bool? DuocKeToanDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string KyHieuHoaDon { get; set; }
        public bool? DaXuatExcel { get; set; }
        public virtual Kho Kho { get; set; }
        public virtual NhanVien NguoiGiao { get; set; }
        public virtual NhanVien NguoiNhap { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<NhapKhoDuocPham> _nhapKhoDuocPhams;
        public virtual ICollection<NhapKhoDuocPham> NhapKhoDuocPhams
        {
            get => _nhapKhoDuocPhams ?? (_nhapKhoDuocPhams = new List<NhapKhoDuocPham>());
            protected set => _nhapKhoDuocPhams = value;
        }
    }
}
