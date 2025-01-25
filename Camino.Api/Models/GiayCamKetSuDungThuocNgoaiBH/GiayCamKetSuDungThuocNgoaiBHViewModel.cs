using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayCamKetSuDungThuocNgoaiBH
{
    public class GiayCamKetSuDungThuocNgoaiBHViewModel
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }


        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienString { get; set; }



        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }
        public long YeuCauTiepNhanId { get; set; }

    }
}
