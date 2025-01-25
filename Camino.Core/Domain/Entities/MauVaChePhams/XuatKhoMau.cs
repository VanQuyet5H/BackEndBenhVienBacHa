using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class XuatKhoMau : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long NguoiXuatId { get; set; }
        public long NguoiNhanId { get; set; }
        public DateTime NgayXuat { get; set; }
        public string GhiChu { get; set; }

        public virtual NhanVien NguoiXuat { get; set; }
        public virtual NhanVien NguoiNhan { get; set; }

        private ICollection<XuatKhoMauChiTiet> _xuatKhoMauChiTiets;
        public virtual ICollection<XuatKhoMauChiTiet> XuatKhoMauChiTiets
        {
            get => _xuatKhoMauChiTiets ?? (_xuatKhoMauChiTiets = new List<XuatKhoMauChiTiet>());
            protected set => _xuatKhoMauChiTiets = value;
        }
    }
}
