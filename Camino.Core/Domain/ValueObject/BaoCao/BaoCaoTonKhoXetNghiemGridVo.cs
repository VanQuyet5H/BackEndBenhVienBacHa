using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTonKhoXetNghiemGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string MaHoaChat { get; set; }
        public string TenHoaChat { get; set; }
        public string DVT { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDateTimeSACH();
        public double? TonDau { get; set; }
        public double? Nhap { get; set; }
        public double? TongSo => ((TonDau ?? 0) + (Nhap ?? 0)).MathRoundNumber(2);
        public double? Xuat { get; set; }
        public double? TonCuoi => ((TongSo ?? 0) - (Xuat ?? 0)).MathRoundNumber(2);
        //public long? NhomId { get; set; }
        public string Nhom { get; set; }
        public string Loai { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
    }
}
