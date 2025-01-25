using System;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class CuringInfoAndServicesGeneralSheetViewModel
    {
        public CuringInfoAndServicesGeneralSheetViewModel()
        {
            ChanDoan = string.Empty;
            MethodCuringPlan = string.Empty;
            TienLuong = string.Empty;
            LuuY = string.Empty;
            SuDungBhyt = string.Empty;
            ServicesPriceInfo = string.Empty;
            NgayThucHien = null;
            BsDieuTri = null;
        }

        public string ChanDoan { get; set; }

        public string MethodCuringPlan { get; set; }

        public string TienLuong { get; set; }

        public string LuuY { get; set; }

        public string SuDungBhyt { get; set; }

        public string ServicesPriceInfo { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public long? BsDieuTri { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }
    }
}
