using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class DichVuKyThuatThongTinGiaGridVo: GridItem
    {
        public Enums.HangBenhVien HangBenhVien { get; set; }
        public string TenHangBenhVien { get; set; }
        public decimal Gia { get; set; }
        public string GiaFormat { get; set; }
       public DateTime? TuNgay { get; set; }
        public string TuNgayFormat { get; set; }
        public DateTime? DenNgay { get; set; }
        public string DenNgayFormat { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public string TenHieuLuc { get ; set; }
    }
}
