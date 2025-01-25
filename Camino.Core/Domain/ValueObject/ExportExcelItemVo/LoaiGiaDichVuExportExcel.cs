using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LoaiGiaDichVuExportExcel
    {
        [Width(30)]
        public string TenNhom { get; set; }
        [Width(40)]
        public string Ten { get; set; }
    }
}
