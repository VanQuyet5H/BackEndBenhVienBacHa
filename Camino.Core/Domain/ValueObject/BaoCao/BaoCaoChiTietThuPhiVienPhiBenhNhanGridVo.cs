using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace Camino.Core.Domain.ValueObject
{
    public class BaoCaoChiTietThuPhiVienPhiBenhNhanGridVo : GridItem
    {
        public int STT { get; set; }
        public string MaTiepNhan { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
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
        public string NguoiThu => DanhSachNguoiThu == null ? "" : string.Join(", ", DanhSachNguoiThu.Distinct().ToArray());
        public List<string> DanhSachNguoiThu { get; set; }
    }
}
