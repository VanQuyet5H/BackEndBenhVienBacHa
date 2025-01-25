using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.CongTyBhtns
{
    public class CongTyBhtnViewModel : BaseViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public string Email { get; set; }

        public string MaSoThue { get; set; }

        public string DonVi { get; set; }

        public Enums.EnumHinhThucBaoHiem HinhThucBaoHiem { get; set; }

        public string HinhThucBaoHiemDisplay => HinhThucBaoHiem.GetDescription();

        public Enums.EnumPhamViBaoHiem PhamViBaoHiem { get; set; }

        public string PhamViBaoHiemDisplay => PhamViBaoHiem.GetDescription();

        public string GhiChu { get; set; }
    }
}
