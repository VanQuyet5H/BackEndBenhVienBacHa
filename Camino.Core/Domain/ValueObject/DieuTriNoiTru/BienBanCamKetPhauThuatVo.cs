using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class BienBanCamKetPhauThuatVo
    {
        public BienBanCamKetPhauThuatVo()
        {
            BsGiaiThich = string.Empty;
            NgayHoiChan = null;
            NgayThucHien = null;
            ChanDoan = string.Empty;
            PhuongPhapPttt = string.Empty;
            BacSiThucHien = null;
            NguoiThucHienReadonly = string.Empty;
            NgayThucHienReadonly = string.Empty;
            IdNoiTruHoSo = null;
            ThongTinNguoiBenhs = new List<ThongTinNguoiBenhVo>();
        }

        public string BsGiaiThich { get; set; }

        public string ChanDoan { get; set; }

        public string PhuongPhapPttt { get; set; }

        public DateTime? NgayHoiChan { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public long? BacSiThucHien { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public List<ThongTinNguoiBenhVo> ThongTinNguoiBenhs { get; set; }
    }

    public class ThongTinNguoiBenhVo : GridItem
    {
        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string Cmnd { get; set; }

        public long? QuanHe { get; set; }

        public string DiaChi { get; set; }
    }
}
