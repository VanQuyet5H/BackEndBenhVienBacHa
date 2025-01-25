using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan
{
    public class DanhSachLichSuThuNganGridVo : GridItem
    {
        public string SoBLHD { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr => NgayThu.ApplyFormatDateTime();
        public string NgayThuDisplay => NgayThu.ApplyFormatDateTimeSACH();
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string HoTenRemoveDiacritics => HoTen.RemoveDiacritics();
        public int? NamSinh { get; set; }
        public string GioiTinhStr { get; set; }
        public string DiaChi { get; set; }
        public string DiaChiRemoveDiacritics => DiaChi.RemoveDiacritics();
        public string DienThoaiStr { get; set; }
        public string NguoiThu { get; set; }
        public string NguoiThuRemoveDiacritics => NguoiThu.RemoveDiacritics();
        public string NoiThucHien { get; set; }
        public string ThuChiTienBenhNhanStr { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public int? LoaiPhieu { get; set; }
        public string LyDoHuy { get; set; }
        public decimal? SoTienThu { get; set; }

        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }

        public decimal? SoTienMiemGiam { get; set; }
        public decimal? SoTienCongNo { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
}