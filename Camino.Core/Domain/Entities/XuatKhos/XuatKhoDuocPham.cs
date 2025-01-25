using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;

namespace Camino.Core.Domain.Entities.XuatKhos
{
    public class XuatKhoDuocPham : BaseEntity
    {
        //public long KhoDuocPhamXuatId { get; set; }
        public long KhoXuatId { get; set; }
        //public long? KhoDuocPhamNhapId { get; set; }
        public long? KhoNhapId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public Enums.XuatKhoDuocPham LoaiXuatKho { get; set; }
        public string LyDoXuatKho { get; set; }
        public string TenNguoiNhan { get; set; }
        public long? NguoiNhanId { get; set; }
        public long NguoiXuatId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiNhan { get; set; }
        public DateTime NgayXuat { get; set; }

        public long? YeuCauLinhDuocPhamId { get; set; }
        public virtual YeuCauLinhDuocPham YeuCauLinhDuocPham { get; set; }

        public bool? TraNCC { get; set; }

        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public virtual NhaThau NhaThau { get; set; }

        private ICollection<XuatKhoDuocPhamChiTiet> _xuatKhoDuocPhamChiTiets;
        public virtual ICollection<XuatKhoDuocPhamChiTiet> XuatKhoDuocPhamChiTiets
        {
            get => _xuatKhoDuocPhamChiTiets ?? (_xuatKhoDuocPhamChiTiets = new List<XuatKhoDuocPhamChiTiet>());
            protected set => _xuatKhoDuocPhamChiTiets = value;
        }
        private ICollection<NhapKhoDuocPham> _nhapKhoDuocPham;
        public virtual ICollection<NhapKhoDuocPham> NhapKhoDuocPhams
        {
            get => _nhapKhoDuocPham ?? (_nhapKhoDuocPham = new List<NhapKhoDuocPham>());
            protected set => _nhapKhoDuocPham = value;
        }

        public virtual Kho KhoDuocPhamXuat { get; set; }
        public virtual Kho KhoDuocPhamNhap { get; set; }

        public virtual NhanVien NguoiNhan { get; set; }
        public virtual NhanVien NguoiXuat { get; set; }

        #region clone
        public XuatKhoDuocPham Clone()
        {
            return (XuatKhoDuocPham)this.MemberwiseClone();
        }
        #endregion
    }
}