using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DoiTuongUuDaiDichVuKhamBenhExportExcel
    {
        public DoiTuongUuDaiDichVuKhamBenhExportExcel()
        {
            DoiTuongUuDaiDichVuKhamBenhExportExcelChild = new List<DoiTuongUuDaiDichVuKhamBenhExportExcelChild>();
        }
        [Width(30)]
        public string Ma { get; set; }
        [Width(30)]
        public string Ma4350 { get; set; }
        [Width(150)]
        public string Ten { get; set; }
        public long DichVuKyThuatId { get; set; }
        public List<DoiTuongUuDaiDichVuKhamBenhExportExcelChild> DoiTuongUuDaiDichVuKhamBenhExportExcelChild { get;set; }
    }

    public class DoiTuongUuDaiDichVuKhamBenhExportExcelChild
    {
        [TitleGridChild("Đối tượng")]
        public string DoiTuong { get; set; }
        [TitleGridChild("Tỉ lệ ưu đãi")]
        public string TiLeUuDai { get; set; }
    }
}
