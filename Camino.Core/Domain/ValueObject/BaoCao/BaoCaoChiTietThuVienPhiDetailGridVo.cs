using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoChiTietThuVienPhiDetailGridVo : GridItem
    {
        public int STT { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTimeSACH();
        public string SoBLHD { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string SoBenhAn { get; set; }
        public bool GoiDichVu { get; set; }
        public decimal? SoTienThu { get; set; }
        public decimal? HuyThu { get; set; }
        public decimal? ThucThu => SoTienThu.GetValueOrDefault() + HuyThu.GetValueOrDefault();
        public decimal? CongNo { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
        public decimal? Voucher { get; set; }
        public string LyDo { get; set; }
        public string NguoiThu { get; set; }
    }
}
