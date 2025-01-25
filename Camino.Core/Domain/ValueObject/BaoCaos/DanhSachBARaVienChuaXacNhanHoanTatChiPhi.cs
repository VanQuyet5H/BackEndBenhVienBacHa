using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{

    public class DanhSachBARaVienChuaXacNhanHoanTatChiPhiQueryInfoVo : QueryInfo
    {
        public string TimKiem { get; set; }
        public long KhoaId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    public class DanhSachBARaVienChuaXacNhanHoanTatChiPhi : GridItem
    {
        public string MaNB { get; set; }
        public string MaTN { get; set; }
        public string SoBenhAn { get; set; }
        public string TenBenhNhan { get; set; }
        public string SoTheBHYT { get; set; }  

        public DateTime? NgayVaoVien { get; set; }
        public string NgayVaoVienDisplay => NgayVaoVien?.ApplyFormatDateTimeSACH();

        public DateTime? NgayRaVien { get; set; }
        public string NgayRaVienDisplay => NgayRaVien?.ApplyFormatDateTimeSACH();

        public bool NhapVien { get; set; }
        public string KhoaRaVien { get; set; }

        public decimal TamUng { get; set; }
        public decimal HoanUng { get; set; }
        public decimal DaThanhToan { get; set; }

    }
}
