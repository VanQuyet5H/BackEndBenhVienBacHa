using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauTraDuocPhams
{
    public class YeuCauTraDuocPham : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        //public long XuatKhoVatTuChiTietId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }

        public virtual Kho KhoXuat { get; set; }
        public virtual Kho KhoNhap { get; set; }
        //public virtual XuatKhoVatTuChiTiet XuatKhoVatTuChiTiet { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiets { get; set; }
        public virtual ICollection<YeuCauTraDuocPhamChiTiet> YeuCauTraDuocPhamChiTiets
        {
            get => _yeuCauTraDuocPhamChiTiets ?? (_yeuCauTraDuocPhamChiTiets = new List<YeuCauTraDuocPhamChiTiet>());
            protected set => _yeuCauTraDuocPhamChiTiets = value;
        }
    }
}
