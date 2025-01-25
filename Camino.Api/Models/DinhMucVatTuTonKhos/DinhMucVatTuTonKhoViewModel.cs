namespace Camino.Api.Models.DinhMucVatTuTonKhos
{
    public class DinhMucVatTuTonKhoViewModel : BaseViewModel
    {
        public long? VatTuBenhVienId { get; set; }

        public string TenVatTu { get; set; }

        public long? KhoId { get; set; }

        public string TenKhoVatTu { get; set; }

        public int? TonToiThieu { get; set; }

        public int? TonToiDa { get; set; }

        public int? SoNgayTruocKhiHetHan { get; set; }

        public string MoTa { get; set; }
    }
}
