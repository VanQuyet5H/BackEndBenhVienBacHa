using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.GachNo
{
    public class GachNoBenhNhanViewModel : BaseViewModel
    {
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNamSinh
        {
            get
            {
                if (NgaySinh != null || ThangSinh != null || NamSinh != null)
                {
                    if (NgaySinh == null || ThangSinh == null)
                    {
                        return $"{NamSinh ?? 0}";
                    }
                    return $"{NgaySinh ?? 0}/{ThangSinh ?? 0}/{NamSinh ?? 0}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh?.GetDescription();
        public string SoChungMinhThu { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string DiaChiDayDu { get; set; }
    }
}
