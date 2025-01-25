using Camino.Core.Domain;

namespace Camino.Api.Models
{
    public class TaiLieuDinhKemViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long? KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public string MoTa { get; set; }
        public Enums.LoaiTapTin? LoaiTapTin { get; set; }
    }
}