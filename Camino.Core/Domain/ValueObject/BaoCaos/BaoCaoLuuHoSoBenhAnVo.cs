using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoLuuHoSoBenhAnVo
    {
        public long KhoaId { get; set; }
        public DateTime? NgayVaoVien { get; set; }
        public DateTime? NgayRaVien { get; set; }
        public string Hosting { get; set; }
    }
    public class BaoCaoLuuHoSoBenhAnGridVo :GridItem
    {
        public string ThuTuSap { get; set; }
        public string SoLuuTru { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string Tuoi { get; set; }
        public DateTime? ThoiGianVaoVien { get; set; }
        public DateTime? ThoiGianRaVien { get; set; }
        public string ThoiGianVaoVienString => ThoiGianVaoVien != null ? ThoiGianVaoVien.GetValueOrDefault().ApplyFormatDateTimeSACH() :"";
        public string ThoiGianRaVienString => ThoiGianRaVien != null ? ThoiGianRaVien.GetValueOrDefault().ApplyFormatDateTimeSACH() :"";
        public string ChanDoan { get; set; }
        public string ICD { get; set; }
        public string Khoa { get; set; }
        public string ThongTinRaVien { get; set; }
}
    public class ChuanDoanVaICD 
    {
        public string GhiChuChuanDoanRaVien { get; set; }
        public string TenChuanDoanRaVien { get; set; }
      
    }
}
