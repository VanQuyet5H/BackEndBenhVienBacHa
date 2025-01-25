using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ExportNhaSanXuatExcel
    {
        [Width(30)]
        public string Ma { get; set; }

        [Width(100)]
        public string Ten { get; set; }
    }
}
