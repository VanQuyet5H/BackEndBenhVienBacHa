using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class ThanhToanHelper
    {
        public static bool SoTienTuongDuong(this decimal value1, decimal value2)
        {
            return Decimal.Compare(Math.Round(value1, MidpointRounding.AwayFromZero), Math.Round(value2, MidpointRounding.AwayFromZero)) == 0;
        }
    }
}
