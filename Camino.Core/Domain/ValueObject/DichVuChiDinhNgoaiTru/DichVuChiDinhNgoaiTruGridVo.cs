using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DichVuChiDinhNgoaiTru
{
    public class NhomDichVuKyThuatChiDinhNgoaiTruGridVo: GridItem
    {
        public string TenDichVu { get; set; }
        public string TenNhom => (ChiDinhSauNhapVien ? "Chỉ định sau nhập viện" : "Chỉ định trước nhập viện") 
                                 + (!string.IsNullOrEmpty(TenDichVu) ? $" - {TenDichVu}" : string.Empty);
        public bool ChiDinhSauNhapVien { get; set; }
        public string HighLightClass => "bg-row-lightblue";
    }

    public class ThongTinDichVuChiDinhNgoaiTruVo
    {
        public long Id { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long? DichVuKhamId { get; set; }
        public string TenDichVuKham { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public DateTime? ThoiDiemNhapVien { get; set; }
        public bool ChiDinhSauNhapVien => ThoiDiemNhapVien != null && ThoiDiemChiDinh > ThoiDiemNhapVien;
    }

    public class DichVuNgoaiTruTimKiemVo
    {
        public long TiepNhanNgoaiTruId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public bool ChiDinhSauNhapVien { get; set; }
    }
}
