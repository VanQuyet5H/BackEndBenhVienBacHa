using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Camino.Core.Domain.Entities.XuatKhoQuaTangs
{
    public class XuatKhoQuaTang : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string SoPhieu { get; set; }
        public long NguoiXuatId { get; set; }
        public long BenhNhanId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public DateTime NgayXuat { get; set; }
        public string GhiChu { get; set; }

        public virtual NhanVien NguoiXuat { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }

        private ICollection<XuatKhoQuaTangChiTiet> _xuatKhoQuaTangChiTiets;
        public virtual ICollection<XuatKhoQuaTangChiTiet> XuatKhoQuaTangChiTiet
        {
            get => _xuatKhoQuaTangChiTiets ?? (_xuatKhoQuaTangChiTiets = new List<XuatKhoQuaTangChiTiet>());
            protected set => _xuatKhoQuaTangChiTiets = value;
        }
    }
}
