using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoThuPhiVienPhiViewModel
    {
        public int STT { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr { get; set; }

        public string SoBLHD { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string SoBenhAn { get; set; }
        public bool GoiDichVu { get; set; }  

        public decimal? SoTienThu { get; set; }
        public decimal? HuyThu { get; set; }
        public decimal? ThucThu { get; set; }

        public decimal? CongNo { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
        public string NguoiThu { get; set; }        
    }
}
