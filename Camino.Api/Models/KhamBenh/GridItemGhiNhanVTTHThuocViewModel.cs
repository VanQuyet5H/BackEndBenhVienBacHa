using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class GridItemGhiNhanVTTHThuocViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public string YeuCauGhiNhanVTTHThuocId { get; set; }
        public bool IsCapNhatSoLuong { get; set; }
        public double? SoLuong { get; set; }
        public bool IsCapNhatTinhPhi { get; set; }
        public bool? TinhPhi { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
    }

    public class UpdateSoLuongItemGhiNhanVTTHThuocViewModel
    {
        public double? SoLuong { get; set; }
        public double? SoLuongBanDau { get; set; }
        public bool? LaDuocPham { get; set; }
        public long? VatTuThuocBenhVienId { get; set; }
        public bool? LaBHYT { get; set; }
        public long? KhoId { get; set; }
    }

    public class VTTHThuocCanKiemTraTrungKhiThemViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public string DichVuChiDinhId { get; set; }
        public string DichVuGhiNhanId { get; set; }
    }
}
