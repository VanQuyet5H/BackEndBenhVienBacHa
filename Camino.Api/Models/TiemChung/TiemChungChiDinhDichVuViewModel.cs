using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungChiDinhDichVuViewModel
    {
        public TiemChungChiDinhDichVuViewModel()
        {
            YeuCauDichVuKyThuats = new List<YeuCauKhamTiemChungViewModel>();
        }

        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauKhamBenh { get; set; }
        public bool ChuyenHangDoiSangLamChiDinh { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay
        {
            get { return SoDuTaiKhoan.ApplyFormatMoneyVND(); }
        }

        public decimal SoDuTaiKhoanConLai { get; set; }
        public string SoDuTaiKhoanConLaiDisplay
        {
            get { return SoDuTaiKhoanConLai.ApplyFormatMoneyVND(); }
        }
        public bool IsVuotQuaSoDuTaiKhoan { get; set; }
        public bool? IsVuotQuaBaoLanhGoi { get; set; }
        public byte[] LastModified { get; set; }

        public List<YeuCauKhamTiemChungViewModel> YeuCauDichVuKyThuats { get; set; }
    }
}
