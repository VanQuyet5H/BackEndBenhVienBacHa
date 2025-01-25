using Camino.Core.Domain.ValueObject.PhieuCongKhaiDichVuKyThuat;
using Camino.Core.Domain.ValueObject.TrichBienBanHoiChan;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.DieuTriNoiTru
{
    public partial interface IDieuTriNoiTruService
    {
        List<string> GetDataPhieuCongKhaiThuocVatTu(PhieuCongKhaiThuocVatTu phieuCongKhaiThuocVatTu);
        PhieuCongKhaiThuocVatTuObject GetThongTinPhieuCongKhaiThuocVatTu(long yeuCauTiepNhanId);
        Task<string> InPhieuCongKhaiThuocVatTu(XacNhanInTrichBienBanHoiChan xacNhanInTrichBienBanHoiChan);
    }
}
