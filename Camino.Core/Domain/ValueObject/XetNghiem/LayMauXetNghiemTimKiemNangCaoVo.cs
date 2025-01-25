using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class LayMauXetNghiemTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
        public LayMauXetNghiemTrangThaiTimKiemNangCapVo TrangThai { get; set; }
        public TuNgayDenNgayVo TuNgayDenNgay { get; set; }
        public bool? IsGridChuaCapCode { get; set; }

    }

    public class LayMauXetNghiemTrangThaiTimKiemNangCapVo
    {
        public bool ChoLayMau { get; set; }
        public bool ChoGuiMau { get; set; }
        public bool ChoKetQua { get; set; }
        public bool DaCoKetQua { get; set; }
        public bool DaLayMau { get; set; }
    }

    public class TuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
