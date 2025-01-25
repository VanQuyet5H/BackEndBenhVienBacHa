using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.XuatKhos;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;

namespace Camino.Core.Domain.Entities.NhapKhoDuocPhams
{
    public class NhapKhoDuocPham : BaseEntity
    {
        public long? XuatKhoDuocPhamId { get; set; }
        //public long KhoDuocPhamId { get; set; }
        public long KhoId { get; set; }
        public string SoChungTu { get; set; }
        //public Enums.EnumLoaiNhapKho LoaiNhapKho { get; set; }
        public string TenNguoiGiao { get; set; }
        public long? NguoiGiaoId { get; set; }
        public long NguoiNhapId { get; set; }
        public bool? DaHet { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public DateTime? NgayHoaDon { get; set; }

        public long? YeuCauNhapKhoDuocPhamId { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }
        public virtual YeuCauNhapKhoDuocPham YeuCauNhapKhoDuocPham { get; set; }
        public virtual YeuCauLinhDuocPham YeuCauLinhDuocPham { get; set; }

        public virtual KhoDuocPhams.Kho KhoDuocPhams { get; set; }
        public virtual XuatKhoDuocPham XuatKhoDuocPham { get; set; }
        public virtual NhanVien NhanVienNhap { get; set; }
        public DateTime NgayNhap { get; set; }
        private ICollection<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTiets;
        public virtual ICollection<NhapKhoDuocPhamChiTiet> NhapKhoDuocPhamChiTiets
        {
            get => _nhapKhoDuocPhamChiTiets ?? (_nhapKhoDuocPhamChiTiets = new List<NhapKhoDuocPhamChiTiet>());
            protected set => _nhapKhoDuocPhamChiTiets = value;
        }
    
    }
}
