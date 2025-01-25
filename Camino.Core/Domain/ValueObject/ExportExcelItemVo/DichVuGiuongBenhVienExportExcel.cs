using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuGiuongBenhVienExportExcel : GridItem
    {
        public DichVuGiuongBenhVienExportExcel()
        {
            DichVuGiuongBenhVienExportExcelChild = new List<DichVuGiuongBenhVienExportExcelChild>();
        }

        [Width(30)]
        public string Ma { get; set; }

        [Width(23)]
        public string MaTT37 { get; set; }

        [Width(54)]
        public string Ten { get; set; }

        [Width(110)]
        public string TenNoiThucHien { get; set; }

        [Width(20)]
        public string HangBenhVienDisplay { get; set; }

        [Width(20)]
        public string LoaiGiuongDisplay { get; set; }

        [Width(50)]
        public string MoTa { get; set; }

        [Width(20)]
        public string HieuLucHienThi { get; set; }

        public List<DichVuGiuongBenhVienExportExcelChild> DichVuGiuongBenhVienExportExcelChild { get; set; }
    }
}
