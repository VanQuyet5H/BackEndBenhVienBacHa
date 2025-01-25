using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTheKhoSoChiTietVatTuHangHoaGridVo: GridItem
    {
        public DateTime NgayXuatNhap { get; set; }
        public string NgayXuatNhapStr => NgayXuatNhap.ApplyFormatDateTimeSACH();
        public string SoChungTu { get; set; }
        public DateTime? NgayChungTu { get; set; }
        public string NgayChungTuStr => NgayChungTu != null ? NgayChungTu.Value.ApplyFormatDateTimeSACH() : string.Empty;
        public string DienGiai { get; set; }
        public decimal DonGia { get; set; }
        public double SoLuongNhap { get; set; }
        public decimal ThanhTienNhap => DonGia * (decimal)SoLuongNhap;
        public double SoLuongXuat { get; set; }
        public decimal ThanhTienXuat => DonGia * (decimal)SoLuongXuat;
        public double SoLuongTon { get; set; }
        public decimal ThanhTienTon { get; set; }
    }
}
