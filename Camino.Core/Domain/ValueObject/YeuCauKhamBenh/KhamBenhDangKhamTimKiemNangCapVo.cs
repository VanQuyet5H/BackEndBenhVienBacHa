using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class KhamBenhDangKhamTimKiemNangCapVo
    {
        public long? PhongBenhVienId { get; set; }
        public long? KhoaPhongId { get; set; }
        public KhamBenhDangKhamTrangThaiTimKiemNangCapVo TrangThai { get; set; }
        public TuNgayDenNgayVo TuNgayDenNgay { get; set; }

    }

    public class KhamBenhDangKhamTrangThaiTimKiemNangCapVo
    {
        public bool ChuaKham { get; set; }
        public bool DangKham { get; set; }
        public bool DangLamChiDinh { get; set; }
        public bool DangDoiKetLuan { get; set; }
    }

    public class TuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
