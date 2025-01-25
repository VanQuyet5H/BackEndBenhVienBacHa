using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.QuayThuoc
{
    public class HinhThucThanhToanViewModel : BaseViewModel
    {
        public long HinhThucThanhToanId { get; set; }
        public string HinhThucThanhToanText { get; set; }
        public int SoTien { get; set; }
    }
}
