using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCaoHoatDongKhoaKhamBenhs
{
    public class BaoCaoHoatDongKhoaKhamBenhVo : GridItem
    {
        public string DichVu { get; set; }

        public int TongSo => Bhyt.GetValueOrDefault() +
                             VienPhi.GetValueOrDefault() +
                             KskDoan.GetValueOrDefault() +
                             Ksk.GetValueOrDefault() +
                             Goi.GetValueOrDefault();

        public int? Bhyt { get; set; }

        public int? VienPhi { get; set; }

        public int? KskDoan { get; set; }

        public int? Ksk { get; set; }

        public int? Goi { get; set; }

        public int? TreEm { get; set; }

        public int? SoLanCapCuu { get; set; }

        public int? SoNguoiBenhVaoVien { get; set; }

        public int? SoNguoiBenhChuyenVien { get; set; }

        public int? SoNguoiBenhDieuTriNgoaiTru { get; set; }

        public int? SoNgayDieuTriNgoaiTru { get; set; }
    }
}
