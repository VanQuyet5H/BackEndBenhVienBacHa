using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.XetNghiems
{
    public class PhieuGoiMauXetNghiem : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long NhanVienGoiMauId { get; set; }
        public DateTime ThoiDiemGoiMau { get; set; }
        public long PhongNhanMauId { get; set; }
        public long? NhanVienNhanMauId { get; set; }
        public DateTime? ThoiDiemNhanMau { get; set; }
        public bool? DaNhanMau { get; set; }
        public string GhiChu { get; set; }

        public virtual NhanVien NhanVienGoiMau { get; set; }
        public virtual NhanVien NhanVienNhanMau { get; set; }
        public virtual PhongBenhVien PhongNhanMau { get; set; }

        private ICollection<MauXetNghiem> _mauXetNghiems;
        public virtual ICollection<MauXetNghiem> MauXetNghiems
        {
            get => _mauXetNghiems ?? (_mauXetNghiems = new List<MauXetNghiem>());
            protected set => _mauXetNghiems = value;
        }
    }
}
