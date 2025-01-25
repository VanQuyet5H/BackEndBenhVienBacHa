using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.CauHinhThuePhong
{
    public class CauHinhThuePhongViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public long? LoaiThuePhongPhauThuatId { get; set; }
        public string TenLoaiThuePhongPhauThuat { get; set; }
        public long? LoaiThuePhongNoiThucHienId { get; set; }
        public string TenLoaiThuePhongNoiThucHien { get; set; }
        public int? BlockThoiGianTheoPhut { get; set; }
        public decimal? GiaThue { get; set; }
        public int? PhanTramNgoaiGio { get; set; }
        public int? PhanTramLeTet { get; set; }
        public decimal? GiaThuePhatSinh { get; set; }
        public int? PhanTramPhatSinhNgoaiGio { get; set; }
        public int? PhanTramPhatSinhLeTet { get; set; }
        public bool? HieuLuc { get; set; }
    }
}
