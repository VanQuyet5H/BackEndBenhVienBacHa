using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DoiTuongUuDaiDichVuKyThuatExportExcel
    {
        public DoiTuongUuDaiDichVuKyThuatExportExcel()
        {
            DoiTuongUuDaiDichVuKyThuatExportExcelChild = new List<DoiTuongUuDaiDichVuKyThuatExportExcelChild>();
        }
       
        [Width(30)]
        public string Ma { get; set; }
        [Width(30)]
        public string Ma4350 { get; set; }
        [Width(150)]
        public string Ten { get; set; }
        public long DichVuKyThuatId { get; set; }
        public List<DoiTuongUuDaiDichVuKyThuatExportExcelChild> DoiTuongUuDaiDichVuKyThuatExportExcelChild { get;set; }
    }

    public class DoiTuongUuDaiDichVuKyThuatExportExcelChild
    {
        [TitleGridChild("Đối tượng")]
        public string DoiTuong { get; set; }
        [TitleGridChild("Tỉ lệ ưu đãi")]
        public string TiLeUuDai { get; set; }
    }
}
