using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauTraVatTus
{
    public class YeuCauTraVatTu : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }

        public virtual Kho KhoXuat { get; set; }
        public virtual Kho KhoNhap { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTiets;
        public virtual ICollection<YeuCauTraVatTuChiTiet> YeuCauTraVatTuChiTiets
        {
            get => _yeuCauTraVatTuChiTiets ?? (_yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuChiTiet>());
            protected set => _yeuCauTraVatTuChiTiets = value;

        }
    }
}
