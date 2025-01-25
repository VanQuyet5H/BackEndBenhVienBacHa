using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinhThuePhong : BaseEntity
    {
        public string Ten { get; set; }
        public long LoaiThuePhongPhauThuatId { get; set; }
        public long LoaiThuePhongNoiThucHienId { get; set; }
        public int BlockThoiGianTheoPhut { get; set; }
        public decimal GiaThue { get; set; }
        public int PhanTramNgoaiGio { get; set; }
        public int PhanTramLeTet { get; set; }
        public decimal GiaThuePhatSinh { get; set; }
        public int PhanTramPhatSinhNgoaiGio { get; set; }
        public int PhanTramPhatSinhLeTet { get; set; }
        public bool HieuLuc { get; set; }

        public virtual LoaiThuePhongPhauThuat LoaiThuePhongPhauThuat { get; set; }
        public virtual LoaiThuePhongNoiThucHien LoaiThuePhongNoiThucHien { get; set; }

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }
    }
}
