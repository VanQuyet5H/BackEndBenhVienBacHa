using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuQuaTangs;
using Camino.Core.Domain.Entities.NhapKhoQuaTangs;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.QuaTangs
{
    public class QuaTang : BaseEntity
    {
        public string Ten { get; set; }
        public string DonViTinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }

        private ICollection<NhapKhoQuaTangChiTiet> _nhapKhoQuaTangChiTiets { get; set; }
        public virtual ICollection<NhapKhoQuaTangChiTiet> NhapKhoQuaTangChiTiets
        {
            get => _nhapKhoQuaTangChiTiets ?? (_nhapKhoQuaTangChiTiets = new List<NhapKhoQuaTangChiTiet>());
            protected set => _nhapKhoQuaTangChiTiets = value;
        }

        private ICollection<XuatKhoQuaTangChiTiet> _xuatKhoQuaTangChiTiets;
        public virtual ICollection<XuatKhoQuaTangChiTiet> XuatKhoQuaTangChiTiet
        {
            get => _xuatKhoQuaTangChiTiets ?? (_xuatKhoQuaTangChiTiets = new List<XuatKhoQuaTangChiTiet>());
            protected set => _xuatKhoQuaTangChiTiets = value;
        }
        private ICollection<ChuongTrinhGoiDichVuQuaTang> _chuongTrinhGoiDichVuQuaTangs { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuQuaTang> ChuongTrinhGoiDichVuQuaTangs
        {
            get => _chuongTrinhGoiDichVuQuaTangs ?? (_chuongTrinhGoiDichVuQuaTangs = new List<ChuongTrinhGoiDichVuQuaTang>());
            protected set => _chuongTrinhGoiDichVuQuaTangs = value;
        }
    }
}
