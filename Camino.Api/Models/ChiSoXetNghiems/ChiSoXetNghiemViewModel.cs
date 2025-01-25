using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.ChiSoXetNghiems
{
    public class ChiSoXetNghiemViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string ChiSoBinhThuongNam { get; set; }
        public string ChiSoBinhThuongNu { get; set; }
        public EnumLoaiXetNghiem LoaiXetNghiem { get; set; }
        public string TenLoaiXetNghiem { get; set; }
        public string MoTa { get; set; }
        public bool? HieuLuc { get; set; }
    }
}
