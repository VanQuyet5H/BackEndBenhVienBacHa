using System;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruTaiNanThuongTichViewModel : BaseViewModel
    {
        public DateTime? ThoiGianXayRaTaiNan { get; set; }
        public DateTime? ThoiGianDenCapCuu { get; set; }
        public string ThongTinTaiNanThuongTich { get; set; }
    }
}
