using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.CauHinhThuePhong
{
    public class CauHinhThuePhongExportExcel
    {
        [Width(50)]
        public string Ten { get; set; }
        [Width(40)]
        public string LoaiThuePhongPhauThuat { get; set; }
        [Width(40)]
        public string LoaiThuePhongNoiThucHien { get; set; }
        [Width(20)]
        public string ThoiGianThueDisplay { get; set; }
        [Width(20)]
        public string GiaThueDisplay { get; set; }
        [Width(20)]
        public int PhanTramNgoaiGio { get; set; }
        [Width(20)]
        public int PhanTramLeTet { get; set; }
        [Width(20)]
        public string GiaThuePhatSinhDisplay { get; set; }
        [Width(20)]
        public int PhanTramPhatSinhNgoaiGio { get; set; }
        [Width(20)]
        public int PhanTramPhatSinhLeTet { get; set; }
        [Width(20)]
        public string HieuLuc { get; set; }
    }
}
