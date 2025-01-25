using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class GiayCamKetGayMeGiamDauTrongDeSauPhauThuatViewModel
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienString { get; set; }

        public string CMND { get; set; }
        public string CoQuanCapCMND { get; set; }
        public string SDT { get; set; }
        public long? BSGMHSId { get; set; }
        public string BSGMHSText { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }
        public long YeuCauTiepNhanId { get; set; }

    }
}
