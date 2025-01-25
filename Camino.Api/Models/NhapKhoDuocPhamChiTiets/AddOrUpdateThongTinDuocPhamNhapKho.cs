using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhapKhoDuocPhamChiTiets
{
    public class AddOrUpdateThongTinDuocPhamNhapKho : BaseViewModel
    {
        public long? HopDongThauDuocPhamId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string Solo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string MaVach { get; set; }
        public int? SoLuongNhap { get; set; }
        public double? DonGiaNhap { get; set; }
        public int? VAT { get; set; }
        public long? KhoViTriId { get; set; }
    }
}
