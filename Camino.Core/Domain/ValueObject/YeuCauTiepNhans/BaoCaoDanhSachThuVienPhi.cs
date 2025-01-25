using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class BaoCaoDanhSachThuVienPhi
    {
        public long BenhNhanId { get; set; }
        public string HoTenBenhNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string NguoiThu { get; set; }
        public DateTime NgayThu { get; set; }
        public string NgayThuStr { get; set; }
        public string SoBienLai { get; set; }
        public decimal? TamUng { get; set; }
        public decimal? ThuVienPhi { get; set; }
        public decimal? HoanTien { get; set; }
        public decimal? CongNo { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }
    }
}
    