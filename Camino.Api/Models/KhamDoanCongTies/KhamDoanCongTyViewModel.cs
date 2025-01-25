using Camino.Core.Domain;

namespace Camino.Api.Models.KhamDoanCongTies
{
    public class KhamDoanCongTyViewModel : BaseViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public string NguoiDaiDien { get; set; }

        public string NguoiLienHe { get; set; }

        public string MaSoThue { get; set; }

        public string SoTaiKhoanNganHang { get; set; }

        public Enums.EnumLoaiCongTy LoaiCongTy { get; set; }

        public bool CoHoatDong { get; set; }
    }
}
