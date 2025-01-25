using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoChiTietThuPhiVienPhiBenhNhanViewModel
    {

        public int STT { get; set; }
        public string MaTiepNhan { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr { get; set; }      

        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string SoBenhAn { get; set; }
        public string TenDichVu { get; set; }
        public string BacSiChiDinhThucHien { get; set; }

        public decimal? DoanhThu { get; set; }
        public decimal? BHYTChiTra { get; set; }
        public decimal? BHYTTuNhanChiTra { get; set; }
        public decimal? MiemGiam { get; set; }
        public decimal? Voucher { get; set; }
        public decimal? ThuTuBenhNhan { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }

        public string NguoiThu { get; set; }


        public string Group => NgayThuStr + MaBenhNhan + TenBenhNhan;
    }
}
