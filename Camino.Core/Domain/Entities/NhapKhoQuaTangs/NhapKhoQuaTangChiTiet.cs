using Camino.Core.Domain.Entities.QuaTangs;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NhapKhoQuaTangs
{
    public class NhapKhoQuaTangChiTiet : BaseEntity
    {
        public long NhapKhoQuaTangId { get; set; }
        public long QuaTangId { get; set; }
        public string NhaCungCap { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int SoLuongDaXuat { get; set; }

        public virtual NhapKhoQuaTang NhapKhoQuaTang { get; set; }

        public virtual QuaTang QuaTang { get; set; }

        private ICollection<XuatKhoQuaTangChiTiet> _xuatKhoQuaTangChiTiets;
        public virtual ICollection<XuatKhoQuaTangChiTiet> XuatKhoQuaTangChiTiet
        {
            get => _xuatKhoQuaTangChiTiets ?? (_xuatKhoQuaTangChiTiets = new List<XuatKhoQuaTangChiTiet>());
            protected set => _xuatKhoQuaTangChiTiets = value;
        }

    }
}
