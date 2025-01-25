
using System;

namespace Camino.Api.Models.QuaTang
{
    public class XuatKhoQuaTangMarketingViewModel : BaseViewModel
    {
        public string SoPhieu { get; set; }
        public long NguoiXuatId { get; set; }
        public string NhanVienXuat { get; set; }
        public long BenhNhanId { get; set; }
        public string NguoiNhan { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string GhiChu { get; set; }
        public long? KhoXuatId { get; set; }
        public string TenKhoXuat { get; set; }
    }
}
