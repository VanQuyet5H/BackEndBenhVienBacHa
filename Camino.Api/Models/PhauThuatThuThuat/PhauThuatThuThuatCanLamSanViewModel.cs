using Camino.Core.Domain.ValueObject.PhauThuatThuThuat;
using System.Collections.Generic;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class DichVuKyThuatChuaHoanThanhTrenHoanThanh
    {
        public int DichVuKyThuatDaHoanThanh { get; set; }
        public int TongDichVuKyThuat { get; set; }
    }

    public class InPhieuChiDinhCLSPTTT
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public List<ListDichVuChiDinhCLSPTTT> ListDichVuChiDinh { get; set; }
        public string Hosting { get; set; }
        public string NhanVienChiDinh { get; set; }
        public long InChungChiDinh { get; set; }
        public bool KieuInChung { get; set; }
        public bool IsFromPhieuDieuTri { get; set; }
        public List<long> ListChonLoaiPhieuIn { get; set; }
    }
}
