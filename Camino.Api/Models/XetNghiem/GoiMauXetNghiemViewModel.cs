using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.XetNghiem
{
    public class PhieuGoiMauXetNghiemViewModel : BaseViewModel
    {
        //public PhieuGoiMauXetNghiemViewModel()
        //{
        //    lstThongTinMauGoi = new List<ThongTinMauGoiViewModel>();
        //}

        public string SoPhieu { get; set; }
        public long NguoiGoiMauId { get; set; }
        public string NguoiGoiMauDisplay { get; set; }
        public DateTime NgayGoiMau { get; set; }
        public string NgayGoiMauDisplay => NgayGoiMau.ApplyFormatDateTimeSACH();
        public long NoiTiepNhanId { get; set; }
        public string NoiTiepNhanDisplay { get; set; }
        public long? NguoiNhanMauId { get; set; }
        public string NguoiNhanMauDisplay { get; set; }
        public DateTime? NgayNhanMau { get; set; }
        public string NgayNhanMauDisplay => NgayNhanMau == null ? "" : NgayNhanMau.Value.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang == true ? "Đã nhận mẫu" : "Chờ nhận mẫu";
        //public List<ThongTinMauGoiViewModel> lstThongTinMauGoi { get; set; }
    }

    public class ThongTinMauGoiViewModel : BaseViewModel
    {
        public long NhomXetNghiemId { get; set; }
        public string Barcode { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int NamSinh { get; set; }
        public bool GioiTinh { get; set; }
        public int TinhTrang { get; set; }
        public bool TuChoi { get; set; }
    }
}
