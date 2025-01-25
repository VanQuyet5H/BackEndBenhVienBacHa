using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ExportMauVaChePhamExportExcel
    {
        [Width(30)]
        public string Ma { get; set; }
        [Width(100)]
        public string Ten { get; set; }
        [Width(30)]
        public string TheTichs { get; set; }
        [Width(30)]
        public string GiaTriToiDas { get; set; }

    }
}
