using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoChiTietThuVienPhiMasterGridVo : GridItem
    {
        public long? NhanVienId { get; set; }
        public string HoTenNhanVien { get; set; }
        public decimal? TotalTamUng { get; set; }
        public decimal? TotalHoanUng { get; set; }
        public decimal? TotalSoTienThu { get; set; }
        public decimal? TotalHuyThu { get; set; }
        public decimal? TotalThucThu => TotalTamUng.GetValueOrDefault() - TotalHoanUng.GetValueOrDefault() + TotalSoTienThu.GetValueOrDefault() - TotalHuyThu.GetValueOrDefault() + TotalThuNoTienMat.GetValueOrDefault() + TotalThuNoChuyenKhoan.GetValueOrDefault() + TotalThuNoPos.GetValueOrDefault();
        public decimal? TotalCongNo { get; set; }
        public decimal? TotalTienMat { get; set; }
        public decimal? TotalChuyenKhoan { get; set; }
        public decimal? TotalPos { get; set; }
        public decimal? TotalVoucher { get; set; }
        public decimal? TotalThuNoTienMat { get; set; }
        public decimal? TotalThuNoChuyenKhoan { get; set; }
        public decimal? TotalThuNoPos { get; set; }
    }
}
