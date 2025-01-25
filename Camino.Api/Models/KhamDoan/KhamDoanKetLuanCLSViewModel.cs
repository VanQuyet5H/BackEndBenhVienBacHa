using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamDoan
{
    public class KhamDoanKetLuanCLSViewModel :BaseViewModel
    {
        public string KSKKetQuaCanLamSang { get; set; }
        public string KSKDanhGiaCanLamSang { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
    }
}
