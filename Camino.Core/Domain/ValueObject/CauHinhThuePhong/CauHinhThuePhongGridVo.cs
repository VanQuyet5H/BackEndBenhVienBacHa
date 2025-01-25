using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.CauHinhThuePhong
{
    public class CauHinhThuePhongGridVo : GridItem
    {
        public string Ten { get; set; }
        public string LoaiThuePhongPhauThuat { get; set; }
        public string LoaiThuePhongNoiThucHien { get; set; }
        public int BlockThoiGianTheoPhut { get; set; }
        public decimal GiaThue { get; set; }
        public int PhanTramNgoaiGio { get; set; }
        public int PhanTramLeTet { get; set; }
        public decimal GiaThuePhatSinh { get; set; }
        public int PhanTramPhatSinhNgoaiGio { get; set; }
        public int PhanTramPhatSinhLeTet { get; set; }
        public bool HieuLuc { get; set; }

        public string ThoiGianThueDisplay => $"<={BlockThoiGianTheoPhut}p";
        public string GiaThueDisplay => GiaThue.ApplyNumber();
        public string GiaThuePhatSinhDisplay => $"{GiaThuePhatSinh.ApplyNumber()} đ/60'";
    }

    public class CauHinhThuePhongTimKiemVo
    {
        public string SearchString { get; set; }
        public long? LoaiPhauThuatId { get; set; }
        public long? NoiThucHienId { get; set; }
    }
}
