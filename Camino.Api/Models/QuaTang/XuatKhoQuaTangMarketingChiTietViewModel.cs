using System;


namespace Camino.Api.Models.QuaTang
{
    public class XuatKhoQuaTangMarketingChiTietViewModel : BaseViewModel
    {
        public string SoPhieu { get; set; }
        public string NhanVienXuat { get; set; }
        public string NguoiNhan{ get; set; }
        public DateTime? NgayXuat { get; set; }
        public long? XuatKhoQuaTangId { get; set; }
        public long? QuaTangId { get; set; }
        public int? SoLuongXuat { get; set; }
    }
}
