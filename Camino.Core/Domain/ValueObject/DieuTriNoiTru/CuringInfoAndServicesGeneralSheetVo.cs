using System;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class CuringInfoAndServicesGeneralSheetVo
    {
        public CuringInfoAndServicesGeneralSheetVo()
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
    }
}
