using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class BienBanCamKetPhauThuatViewModel
    {
        public BienBanCamKetPhauThuatViewModel()
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
            ThongTinNguoiBenhs = new List<ThongTinNguoiBenhViewModel>();
            FileChuKy = new List<FileChuKyViewModel>();
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
        public bool? CheckCreateOrUpdate { get; set; }

        public List<ThongTinNguoiBenhViewModel> ThongTinNguoiBenhs { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
    }

    public class ThongTinNguoiBenhViewModel : BaseViewModel
    {
        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string Cmnd { get; set; }

        public long? QuanHe { get; set; }

        public string DiaChi { get; set; }
    }
}
