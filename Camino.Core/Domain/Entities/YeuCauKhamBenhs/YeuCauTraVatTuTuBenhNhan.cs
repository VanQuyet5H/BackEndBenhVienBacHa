using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauTraVatTuTuBenhNhan : BaseEntity
    {
        public long KhoTraId { get; set; }
        public long KhoaHoanTraId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public long? NhanVienDuyetId { get; set; }
        public string LyDoKhongDuyet { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public DateTime? ThoiDiemHoanTraTongHopTuNgay { get; set; }
        public DateTime? ThoiDiemHoanTraTongHopDenNgay { get; set; }
        public virtual Kho KhoTra { get; set; }
        public virtual KhoaPhong KhoaHoanTra { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }
        public virtual NhanVien NhanVienDuyet { get; set; }

        private ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> YeuCauTraVatTuTuBenhNhanChiTiets
        {
            get => _yeuCauTraVatTuTuBenhNhanChiTiets ?? (_yeuCauTraVatTuTuBenhNhanChiTiets = new List<YeuCauTraVatTuTuBenhNhanChiTiet>());
            protected set => _yeuCauTraVatTuTuBenhNhanChiTiets = value;
        }
    }
}
