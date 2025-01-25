using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class BangTheoDoiHoiTinhViewModel
    {
        public BangTheoDoiHoiTinhViewModel()
        {
            ListPhieu = new List<PhieuHoiTinhViewModel>();
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public string CachMo { get; set; }

        public string CachVoCam { get; set; }

        public string PhongMo { get; set; }

        public string DdNhan { get; set; }

        public DateTime? GioNhan { get; set; }

        public DateTime? GioChuyen { get; set; }

        public string VePhong { get; set; }

        public string PhauThuatVien { get; set; }

        public string BsGmhs { get; set; }

        public string YlenhBs { get; set; }

        public string PhongHoiTinhSo { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public string ThongTinHoSo { get; set; }

        public List<PhieuHoiTinhViewModel> ListPhieu { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
        public bool IsSave { get; set; }
    }

    public class PhieuHoiTinhViewModel : BaseViewModel
    {
        public string NgayThucHienDisplay { get; set; }

        public string CachMo { get; set; }

        public string DdNhan { get; set; }
    }
}
