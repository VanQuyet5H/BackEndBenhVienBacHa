using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Helpers
{
    public static class AddressHelper
    {
        public static string ApplyFormatAddress(string tinhThanh, string quanHuyen, string phuongXa, string diaChi)
        {
            var diaChiFull = diaChi??string.Empty;
            diaChiFull += string.IsNullOrEmpty(phuongXa) ? string.Empty : (diaChiFull == string.Empty ? "" : ", ") + phuongXa;
            diaChiFull += string.IsNullOrEmpty(quanHuyen) ? string.Empty : (diaChiFull == string.Empty ? "" : ", ") + quanHuyen;
            diaChiFull += string.IsNullOrEmpty(tinhThanh) ? string.Empty : (diaChiFull == string.Empty ? "" : ", ") + tinhThanh;
            return diaChiFull;
        }
    }
}
