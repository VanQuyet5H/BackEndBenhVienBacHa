using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacBieuDoChuyenDaVo
    {
        public DateTime? NgayGhiBieuDo { get; set; }
        public long? NguoiGhiBieuDoId { get; set; }
        public string NguoiGhiBieuDoDisplay { get; set; }
        public int? TienThaiPara1 { get; set; }
        public int? TienThaiPara2 { get; set; }
        public int? TienThaiPara3 { get; set; }
        public int? TienThaiPara4 { get; set; }
    }

    public class HoSoKhacGiayInBieuDoChuyenDa
    {
        public string HoTen { get; set; }
        public string Para { get; set; }
        public string SoNhapVien { get; set; }
        public DateTime? NgayVaoVienValue { get; set; }
        public string NgayVaoVien => NgayVaoVienValue?.ApplyFormatDateTimeSACH();
        public DateTime? NgayGhiBieuDoValue { get; set; }
        public string NgayGhiBieuDo => NgayGhiBieuDoValue?.ApplyFormatDate();
        public string OiDaVo { get; set; }
    }
}