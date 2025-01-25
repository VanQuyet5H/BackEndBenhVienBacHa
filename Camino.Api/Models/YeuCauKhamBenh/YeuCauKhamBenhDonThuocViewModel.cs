using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenh
{
    public class YeuCauKhamBenhDonThuocViewModel : BaseViewModel
    {
        public long? YeuCauKhamBenhId { get; set; }
        public long? ToaThuocMauId { get; set; }
        public int? LoaiDonThuoc { get; set; }
        public int? TrangThai { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public long? BacSiKeDonId { get; set; }
        public long? NoiKeDonId { get; set; }
        public DateTime? ThoiDiemKeDon { get; set; }
        public long? NoiCapThuocId { get; set; }
        public long? NhanVienCapThuocId { get; set; }
        public DateTime? ThoiDiemCapThuoc { get; set; }
        public string GhiChu { get; set; }
        public YeuCauKhamBenhDonThuocChiTietViewModel YeuCauKhamBenhDonThuocChiTiet { get; set; }
    }
}
