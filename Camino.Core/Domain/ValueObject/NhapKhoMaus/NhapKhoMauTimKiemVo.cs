using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NhapKhoMaus
{
    public class NhapKhoMauTimKiemVo
    {
        public string SearchString { get; set; }
        public NhapKhoMauTimKiemTrangThaiVo TrangThai { get; set; }
        public NhapKhoMauTimKiemDateRangeVo TuNgayDenNgay { get; set; }

        //BVHD-3926
        public NhapKhoMauTimKiemDateRangeVo TuNgayDenNgayHoaDon { get; set; }
    }

    public class NhapKhoMauTimKiemTrangThaiVo
    {
        public bool ChoNhapGia { get; set; }
        public bool DaNhapGia { get; set; }
    }

    public class NhapKhoMauTimKiemDateRangeVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
