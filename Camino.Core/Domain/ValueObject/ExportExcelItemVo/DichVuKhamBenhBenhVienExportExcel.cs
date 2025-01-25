using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuKhamBenhBenhVienExportExcel
    {
        [Width(30)]
        public string Ma { get; set; }

        [Width(23)]
        public string MaTT37 { get; set; }

        [Width(54)]
        public string Ten { get; set; }

        [Width(20)]
        public string HangBenhVien { get; set; }

        [Width(110)]
        public string TenNoiThucHien { get; set; }

        [Width(50)]
        public string MoTa { get; set; }

        [Width(20)]
        public string HieuLucHienThi { get; set; }
    }
}
